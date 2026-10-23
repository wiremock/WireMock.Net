// Copyright © WireMock.Net

using System.Net;
using WireMock.Models;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace WireMock.Net.Tests;

public partial class WireMockServerTests
{
    private static readonly Guid NearMissPostGuid = new("11111111-0000-0000-0000-000000000001");
    private static readonly Guid NearMissGetGuid = new("11111111-0000-0000-0000-000000000002");
    private static readonly Guid NearMissOtherGuid = new("11111111-0000-0000-0000-000000000003");

    [Fact]
    public async Task WireMockServer_FindUnmatchedLogEntries_ShouldOnlyReturnRequestsWithoutMatchingMapping()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        using var server = StartServerWithNearMissMappings();
        using var client = server.CreateClient();

        await client.GetAsync("/orders", cancellationToken);
        await client.GetAsync("/order", cancellationToken);

        // Act
        var unmatched = server.FindUnmatchedLogEntries();

        // Assert
        unmatched.Should().ContainSingle();
        unmatched[0].RequestMessage!.Path.Should().Be("/order");
        unmatched[0].ResponseMessage!.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task WireMockServer_FindNearMisses_ShouldReturnClosestMappingsFirst()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        using var server = StartServerWithNearMissMappings();
        using var client = server.CreateClient();

        // Path matches the POST and GET mappings, but the method is wrong for both.
        await client.PutAsync("/orders", new StringContent("{}"), cancellationToken);
        var requestMessage = server.LogEntries.Single().RequestMessage!;

        // Act
        var nearMisses = server.FindNearMisses(requestMessage);

        // Assert
        nearMisses.Should().HaveCount(2);
        nearMisses.Select(nm => nm.MappingGuid).Should().BeEquivalentTo([NearMissPostGuid, NearMissGetGuid]);
        nearMisses.Should().AllSatisfy(nm =>
        {
            nm.LogEntryGuid.Should().BeNull();
            nm.RequestMessage.Should().BeSameAs(requestMessage);
            nm.RequestMatchResult.IsPerfectMatch.Should().BeFalse();
            nm.RequestMatchResult.MatchDetails.Should().Contain(md => md.Name == "MethodMatcher" && md.Score < 1.0);
        });
    }

    [Fact]
    public async Task WireMockServer_FindNearMisses_ShouldRespectMaxResults()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        using var server = StartServerWithNearMissMappings();
        using var client = server.CreateClient();

        await client.PutAsync("/orders", new StringContent("{}"), cancellationToken);

        // Act
        var nearMisses = server.FindNearMisses(server.LogEntries.Single().RequestMessage!, maxResults: 1);

        // Assert
        nearMisses.Should().ContainSingle();
    }

    [Fact]
    public async Task WireMockServer_FindNearMisses_ShouldIgnoreDisabledMappings()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        using var server = StartServerWithNearMissMappings();
        server.Mappings.Single(m => m.Guid == NearMissPostGuid).IsDisabled = true;
        using var client = server.CreateClient();

        await client.PutAsync("/orders", new StringContent("{}"), cancellationToken);

        // Act
        var nearMisses = server.FindNearMisses(server.LogEntries.Single().RequestMessage!);

        // Assert
        nearMisses.Select(nm => nm.MappingGuid).Should().NotContain(NearMissPostGuid);
    }

    [Fact]
    public void WireMockServer_FindNearMisses_WithInvalidMaxResults_ShouldThrow()
    {
        // Arrange
        using var server = WireMockServer.Start();
        var requestMessage = new RequestMessage(new UrlDetails("http://localhost/orders"), "GET", "::1");

        // Act
        Action act = () => server.FindNearMisses(requestMessage, maxResults: 0);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public async Task WireMockServer_FindNearMissesForUnmatchedRequests_ShouldReturnNearMissesPerUnmatchedRequest()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        using var server = StartServerWithNearMissMappings();
        using var client = server.CreateClient();

        await client.GetAsync("/orders", cancellationToken);
        await client.PutAsync("/orders", new StringContent("{}"), cancellationToken);
        await client.DeleteAsync("/customers", cancellationToken);
        var unmatchedGuids = server.FindUnmatchedLogEntries().Select(le => le.Guid).ToArray();

        // Act
        var nearMisses = server.FindNearMissesForUnmatchedRequests(maxResultsPerRequest: 1);

        // Assert
        unmatchedGuids.Should().HaveCount(2);
        nearMisses.Should().HaveCount(2);
        nearMisses.Select(nm => nm.LogEntryGuid!.Value).Should().BeEquivalentTo(unmatchedGuids);
        nearMisses.Single(nm => nm.RequestMessage.Path == "/customers").MappingGuid.Should().Be(NearMissOtherGuid);
    }

    [Fact]
    public async Task WireMockServer_FindNearMisses_ShouldNotReturnPerfectMatches()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        using var server = StartServerWithNearMissMappings();
        server
            .Given(Request.Create().WithPath("/orders").UsingGet())
            .AtPriority(100)
            .WithTitle("Get orders (fallback)")
            .RespondWith(Response.Create().WithStatusCode(HttpStatusCode.OK));
        using var client = server.CreateClient();

        await client.GetAsync("/orders", cancellationToken);

        // Act
        var nearMisses = server.FindNearMisses(server.LogEntries.Single().RequestMessage!);

        // Assert
        nearMisses.Should().ContainSingle();
        nearMisses[0].MappingGuid.Should().Be(NearMissPostGuid);
    }

    [Fact]
    public async Task WireMockServer_FindNearMisses_ForLogEntry_ShouldSetLogEntryGuid()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        using var server = StartServerWithNearMissMappings();
        using var client = server.CreateClient();

        await client.PutAsync("/orders", new StringContent("{}"), cancellationToken);
        var logEntry = server.LogEntries.Single();

        // Act
        var nearMisses = server.FindNearMisses(logEntry, maxResults: 1);

        // Assert
        nearMisses.Should().ContainSingle();
        nearMisses[0].LogEntryGuid.Should().Be(logEntry.Guid);
        nearMisses[0].RequestMessage.Should().BeSameAs(logEntry.RequestMessage);
    }

    [Fact]
    public async Task WireMockServer_FindUnmatchedLogEntries_WithAllowPartialMapping_ShouldIncludeRequestsServedByAPartialMatch()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        using var server = StartServerWithNearMissMappings();
        server.AllowPartialMapping();
        using var client = server.CreateClient();

        await client.GetAsync("/orders", cancellationToken);
        await client.PutAsync("/orders", new StringContent("{}"), cancellationToken);

        // Act
        var unmatched = server.FindUnmatchedLogEntries();
        var nearMisses = server.FindNearMissesForUnmatchedRequests();

        // Assert
        unmatched.Should().ContainSingle();
        unmatched[0].RequestMessage!.Method.Should().Be("PUT");
        unmatched[0].MappingGuid.Should().NotBeNull("the request was served by a partial match");
        nearMisses.Select(nm => nm.MappingGuid).Should().Contain(unmatched[0].MappingGuid!.Value);
    }

    private static WireMockServer StartServerWithNearMissMappings()
    {
        var server = WireMockServer.Start();

        server
            .Given(Request.Create().WithPath("/orders").UsingPost())
            .WithGuid(NearMissPostGuid)
            .WithTitle("Create order")
            .RespondWith(Response.Create().WithStatusCode(HttpStatusCode.Created));

        server
            .Given(Request.Create().WithPath("/orders").UsingGet())
            .WithGuid(NearMissGetGuid)
            .WithTitle("Get orders")
            .RespondWith(Response.Create().WithStatusCode(HttpStatusCode.OK));

        server
            .Given(Request.Create().WithPath("/customers").UsingDelete().WithHeader("X-Api-Key", "secret"))
            .WithGuid(NearMissOtherGuid)
            .WithTitle("Delete customers")
            .RespondWith(Response.Create().WithStatusCode(HttpStatusCode.NoContent));

        return server;
    }
}
