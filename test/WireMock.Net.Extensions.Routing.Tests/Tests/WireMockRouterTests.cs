using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using WireMock.Net.Extensions.Routing.Extensions;
using WireMock.Server;

namespace WireMock.Net.Extensions.Routing.Tests.Tests;

public sealed class WireMockRouterTests
{
    private const string DefaultUrlPattern = "/test";

    private readonly WireMockServer _server;
    private readonly WireMockRouter _sut;

    public WireMockRouterTests()
    {
        _server = WireMockServer.Start();
        _sut = new WireMockRouter(_server);
    }

    [Fact]
    public async Task Map_ShouldReturnResultFromHandler_ForGetMethod()
    {
        const int handlerResult = 5;
        _sut.Map(HttpMethod.Get.ToString(), DefaultUrlPattern, _ => handlerResult);
        using var client = _server.CreateClient();

        var result = await client.GetFromJsonAsync<int>(DefaultUrlPattern);

        result.Should().Be(handlerResult);
    }

    [Fact]
    public async Task Map_ShouldReturnResultFromAsyncHandler_ForGetMethod()
    {
        const int handlerResult = 5;
        _sut.Map(HttpMethod.Get.ToString(), DefaultUrlPattern, _ => Task.FromResult(handlerResult));
        using var client = _server.CreateClient();

        var result = await client.GetFromJsonAsync<int>(DefaultUrlPattern);

        result.Should().Be(handlerResult);
    }

    [Fact]
    public async Task Map_ShouldReturnResultFromAsyncHandlerWithAwait_ForGetMethod()
    {
        const int handlerResult = 5;
        _sut.Map(
            HttpMethod.Get.ToString(),
            DefaultUrlPattern,
            async _ => await Task.FromResult(handlerResult));
        using var client = _server.CreateClient();

        var result = await client.GetFromJsonAsync<int>(DefaultUrlPattern);

        result.Should().Be(handlerResult);
    }

    [Fact]
    public async Task Map_ShouldReturnResultFromAsyncHandlerWithDelayAndAwait_ForGetMethod()
    {
        const int handlerResult = 5;

        async Task<object?> HandleRequestAsync()
        {
            await Task.Delay(1);
            return handlerResult;
        }

        _sut.Map(HttpMethod.Get.ToString(), DefaultUrlPattern, _ => HandleRequestAsync());
        using var client = _server.CreateClient();

        var result = await client.GetFromJsonAsync<int>(DefaultUrlPattern);

        result.Should().Be(handlerResult);
    }

    [Fact]
    public async Task MapGet_ShouldReturnResultFromAsyncHandlerWithDelayAwait()
    {
        const int handlerResult = 5;

        async Task<object?> HandleRequestAsync()
        {
            await Task.Delay(1);
            return handlerResult;
        }

        _sut.MapGet(DefaultUrlPattern, _ => HandleRequestAsync());
        using var client = _server.CreateClient();

        var result = await client.GetFromJsonAsync<int>(DefaultUrlPattern);

        result.Should().Be(handlerResult);
    }
}
