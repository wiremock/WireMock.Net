// Copyright © WireMock.Net

using RestEase;
using WireMock.Admin.Mappings;
using WireMock.Client;
using WireMock.Server;

namespace WireMock.Net.Tests.AdminApi;

public partial class WireMockAdminApiTests
{
    [Fact]
    public async Task IWireMockAdminApi_PostMappingAsync_WithIsEnabledFalse_DoesNotMatchRequests()
    {
        // Arrange
        var ct = TestContext.Current.CancellationToken;
        using var server = WireMockServer.StartWithAdminInterface();
        var api = RestClient.For<IWireMockAdminApi>(server.Urls[0]);
        var httpClient = server.CreateClient();

        var model = new MappingModel
        {
            Request = new RequestModel { Path = "/foo", Methods = ["GET"] },
            Response = new ResponseModel { Body = "hello", StatusCode = 200 },
            IsEnabled = false
        };

        // Act — POST the disabled mapping
        var postResult = await api.PostMappingAsync(model, ct);
        postResult.Should().NotBeNull();

        // Assert — request should not be matched (404)
        var response = await httpClient.GetAsync("/foo", ct);
        ((int)response.StatusCode).Should().Be(404);

        // Assert — mapping exists but IsEnabled is false
        server.Mappings.Where(m => !m.IsAdminInterface).Should().ContainSingle(m => m.IsEnabled == false);
    }

    [Fact]
    public async Task IWireMockAdminApi_DisableMappingAsync_PreventsMatching()
    {
        // Arrange
        var ct = TestContext.Current.CancellationToken;
        using var server = WireMockServer.StartWithAdminInterface();
        var api = RestClient.For<IWireMockAdminApi>(server.Urls[0]);
        var httpClient = server.CreateClient();

        var model = new MappingModel
        {
            Request = new RequestModel { Path = "/bar", Methods = ["GET"] },
            Response = new ResponseModel { Body = "world", StatusCode = 200 }
        };
        var postResult = await api.PostMappingAsync(model, ct);
        var guid = postResult.Guid!.Value;

        // Assert — mapping matches before disable
        var before = await httpClient.GetAsync("/bar", ct);
        ((int)before.StatusCode).Should().Be(200);

        // Act — disable
        var disableResult = await api.DisableMappingAsync(guid, ct);
        disableResult.Status.Should().Be("Mapping disabled");

        // Assert — no match after disable
        var after = await httpClient.GetAsync("/bar", ct);
        ((int)after.StatusCode).Should().Be(404);
    }

    [Fact]
    public async Task IWireMockAdminApi_EnableMappingAsync_ResumesMatching()
    {
        // Arrange
        var ct = TestContext.Current.CancellationToken;
        using var server = WireMockServer.StartWithAdminInterface();
        var api = RestClient.For<IWireMockAdminApi>(server.Urls[0]);
        var httpClient = server.CreateClient();

        var model = new MappingModel
        {
            Request = new RequestModel { Path = "/baz", Methods = ["GET"] },
            Response = new ResponseModel { Body = "re-enabled", StatusCode = 200 },
            IsEnabled = false
        };
        var postResult = await api.PostMappingAsync(model, ct);
        var guid = postResult.Guid!.Value;

        // Assert — no match while disabled
        var before = await httpClient.GetAsync("/baz", ct);
        ((int)before.StatusCode).Should().Be(404);

        // Act — enable
        var enableResult = await api.EnableMappingAsync(guid, ct);
        enableResult.Status.Should().Be("Mapping enabled");

        // Assert — mapping matches after enable
        var after = await httpClient.GetAsync("/baz", ct);
        ((int)after.StatusCode).Should().Be(200);
    }

    [Fact]
    public async Task IWireMockAdminApi_GetMappingAsync_ReturnsIsEnabledFalse_WhenDisabled()
    {
        // Arrange
        var ct = TestContext.Current.CancellationToken;
        using var server = WireMockServer.StartWithAdminInterface();
        var api = RestClient.For<IWireMockAdminApi>(server.Urls[0]);

        var disabledModel = new MappingModel
        {
            Request = new RequestModel { Path = "/check-disabled" },
            Response = new ResponseModel { Body = "x", StatusCode = 200 },
            IsEnabled = false
        };
        var enabledModel = new MappingModel
        {
            Request = new RequestModel { Path = "/check-enabled" },
            Response = new ResponseModel { Body = "y", StatusCode = 200 }
        };

        var disabledPost = await api.PostMappingAsync(disabledModel, ct);
        var enabledPost = await api.PostMappingAsync(enabledModel, ct);

        // Act
        var disabledGot = await api.GetMappingAsync(disabledPost.Guid!.Value, ct);
        var enabledGot = await api.GetMappingAsync(enabledPost.Guid!.Value, ct);

        // Assert — disabled mapping serializes IsEnabled = false
        disabledGot.IsEnabled.Should().BeFalse();

        // Assert — enabled mapping omits IsEnabled (null = default true)
        enabledGot.IsEnabled.Should().BeNull();
    }
}
