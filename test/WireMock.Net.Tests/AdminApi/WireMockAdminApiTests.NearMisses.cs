// Copyright © WireMock.Net

using System.Net;
using System.Net.Http;
using RestEase;
using WireMock.Admin.Mappings;
using WireMock.Client;
using WireMock.Logging;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace WireMock.Net.Tests.AdminApi;

public partial class WireMockAdminApiTests
{
    private static readonly Guid OrdersPostMappingGuid = new("22222222-0000-0000-0000-000000000001");
    private static readonly Guid OrdersGetMappingGuid = new("22222222-0000-0000-0000-000000000002");

    [Fact]
    public async Task IWireMockAdminApi_CountRequestsAsync()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        using var server = StartServerWithOrdersMappings();
        using var httpClient = server.CreateClient();
        await httpClient.GetAsync("/orders", cancellationToken);
        await httpClient.GetAsync("/orders", cancellationToken);
        await httpClient.PostAsync("/orders", new StringContent("{}"), cancellationToken);

        var api = RestClient.For<IWireMockAdminApi>(server.Url!);

        // Act
        var getCount = await api.CountRequestsAsync(new RequestModel { Methods = ["GET"] }, cancellationToken);
        var deleteCount = await api.CountRequestsAsync(new RequestModel { Methods = ["DELETE"] }, cancellationToken);

        // Assert
        getCount.Count.Should().Be(2);
        deleteCount.Count.Should().Be(0);
    }

    [Fact]
    public async Task IWireMockAdminApi_GetUnmatchedRequestsAsync()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        using var server = StartServerWithOrdersMappings();
        using var httpClient = server.CreateClient();
        await httpClient.GetAsync("/orders", cancellationToken);
        await httpClient.GetAsync("/unknown", cancellationToken);

        var api = RestClient.For<IWireMockAdminApi>(server.Url!);

        // Act
        var requests = await api.GetUnmatchedRequestsAsync(cancellationToken);

        // Assert
        requests.Should().ContainSingle();
        requests[0].Request!.Path.Should().Be("/unknown");
        requests[0].MappingGuid.Should().BeNull();
    }

    [Fact]
    public async Task IWireMockAdminApi_GetNearMissesForUnmatchedRequestsAsync()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        using var server = StartServerWithOrdersMappings();
        using var httpClient = server.CreateClient();
        await httpClient.PutAsync("/orders", new StringContent("{}"), cancellationToken);
        var logEntryGuid = server.LogEntries.Single().Guid;

        var api = RestClient.For<IWireMockAdminApi>(server.Url!);

        // Act
        var nearMisses = await api.GetNearMissesForUnmatchedRequestsAsync(cancellationToken: cancellationToken);

        // Assert
        nearMisses.Should().HaveCount(2);
        nearMisses.Select(nm => nm.MappingGuid).Should().BeEquivalentTo([OrdersPostMappingGuid, OrdersGetMappingGuid]);
        nearMisses.Should().AllSatisfy(nm =>
        {
            nm.LogEntryGuid.Should().Be(logEntryGuid);
            nm.Request!.Method.Should().Be("PUT");
            nm.Request.Path.Should().Be("/orders");
            nm.RequestMatchResult!.IsPerfectMatch.Should().BeFalse();
            nm.RequestMatchResult.AverageTotalScore.Should().BeApproximately(0.5, 0.001);
        });
        nearMisses.Select(nm => nm.MappingTitle).Should().Contain("Create order");
    }

    [Fact]
    public async Task IWireMockAdminApi_GetNearMissesForUnmatchedRequestsAsync_WithMaxResults()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        using var server = StartServerWithOrdersMappings();
        using var httpClient = server.CreateClient();
        await httpClient.PutAsync("/orders", new StringContent("{}"), cancellationToken);

        var api = RestClient.For<IWireMockAdminApi>(server.Url!);

        // Act
        var nearMisses = await api.GetNearMissesForUnmatchedRequestsAsync(1, cancellationToken);

        // Assert
        nearMisses.Should().ContainSingle();
    }

    [Fact]
    public async Task IWireMockAdminApi_GetNearMissesForUnmatchedRequestsAsync_WithInvalidMaxResults_ReturnsBadRequest()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        using var server = StartServerWithOrdersMappings();
        using var httpClient = server.CreateClient();

        // Act
        var response = await httpClient.GetAsync("/__admin/requests/unmatched/near-misses?maxResults=0", cancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task IWireMockAdminApi_GetNearMissesForRequestAsync_ShouldExcludeMatchedMapping()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        using var server = StartServerWithOrdersMappings();
        using var httpClient = server.CreateClient();
        await httpClient.GetAsync("/orders", cancellationToken);
        var logEntry = server.LogEntries.Single();

        var api = RestClient.For<IWireMockAdminApi>(server.Url!);

        // Act
        var nearMisses = await api.GetNearMissesForRequestAsync(logEntry.Guid, cancellationToken: cancellationToken);

        // Assert
        logEntry.MappingGuid.Should().Be(OrdersGetMappingGuid);
        nearMisses.Should().ContainSingle();
        nearMisses[0].MappingGuid.Should().Be(OrdersPostMappingGuid);
        nearMisses[0].LogEntryGuid.Should().Be(logEntry.Guid);
    }

    [Fact]
    public async Task IWireMockAdminApi_GetNearMissesForRequestAsync_UnknownRequest_ReturnsNotFound()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        using var server = StartServerWithOrdersMappings();

        var api = RestClient.For<IWireMockAdminApi>(server.Url!);

        // Act
        Func<Task> act = () => api.GetNearMissesForRequestAsync(Guid.NewGuid(), cancellationToken: cancellationToken);

        // Assert
        (await act.Should().ThrowAsync<ApiException>()).Which.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task IWireMockAdminApi_GetNearMissesForRequestAsync_WithMultiplePerfectMatches_ShouldNotReturnPerfectMatches()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        using var server = StartServerWithOrdersMappings();
        server
            .Given(Request.Create().WithPath("/orders").UsingGet())
            .AtPriority(100)
            .WithTitle("Get orders (fallback)")
            .RespondWith(Response.Create().WithStatusCode(HttpStatusCode.OK));
        using var httpClient = server.CreateClient();
        await httpClient.GetAsync("/orders", cancellationToken);
        var logEntry = server.LogEntries.Single();

        var api = RestClient.For<IWireMockAdminApi>(server.Url!);

        // Act
        var nearMisses = await api.GetNearMissesForRequestAsync(logEntry.Guid, cancellationToken: cancellationToken);

        // Assert
        nearMisses.Should().ContainSingle();
        nearMisses[0].MappingGuid.Should().Be(OrdersPostMappingGuid);
    }

    [Fact]
    public async Task IWireMockAdminApi_CountRequestsAsync_ShouldNotCountAdminRequests()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        using var server = StartServerWithOrdersMappings();
        using var httpClient = server.CreateClient();
        await httpClient.GetAsync("/orders", cancellationToken);
        await httpClient.GetAsync("/__admin/mappings", cancellationToken);

        var api = RestClient.For<IWireMockAdminApi>(server.Url!);

        // Act
        var count = await api.CountRequestsAsync(new RequestModel { Methods = ["GET"] }, cancellationToken);

        // Assert
        count.Count.Should().Be(1);
    }

    private static WireMockServer StartServerWithOrdersMappings()
    {
        var server = WireMockServer.Start(new WireMockServerSettings
        {
            StartAdminInterface = true,
            Logger = new WireMockNullLogger()
        });

        server
            .Given(Request.Create().WithPath("/orders").UsingPost())
            .WithGuid(OrdersPostMappingGuid)
            .WithTitle("Create order")
            .RespondWith(Response.Create().WithStatusCode(HttpStatusCode.Created));

        server
            .Given(Request.Create().WithPath("/orders").UsingGet())
            .WithGuid(OrdersGetMappingGuid)
            .WithTitle("Get orders")
            .RespondWith(Response.Create().WithStatusCode(HttpStatusCode.OK));

        return server;
    }
}
