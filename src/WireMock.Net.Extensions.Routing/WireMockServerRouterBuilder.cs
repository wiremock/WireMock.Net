using System.Collections.Concurrent;
using Newtonsoft.Json;
using WireMock.Net.Extensions.Routing.Delegates;
using WireMock.Server;

namespace WireMock.Net.Extensions.Routing;

/// <summary>
/// Provides a builder for configuring and creating a <see cref="WireMockRouter"/> with middleware and JSON settings.
/// </summary>
public sealed class WireMockServerRouterBuilder
{
    private readonly WireMockServer _server;

    private readonly ConcurrentQueue<WireMockMiddleware> _middlewareCollection = new();

    private JsonSerializerSettings? _defaultJsonSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="WireMockServerRouterBuilder"/> class.
    /// </summary>
    /// <param name="server">The WireMock server instance.</param>
    public WireMockServerRouterBuilder(WireMockServer server)
    {
        _server = server;
    }

    /// <summary>
    /// Builds a <see cref="WireMockRouter"/> with the configured middleware and JSON settings.
    /// </summary>
    /// <returns>The configured <see cref="WireMockRouter"/>.</returns>
    public WireMockRouter Build() =>
        new(_server)
        {
            MiddlewareCollection = _middlewareCollection,
            DefaultJsonSettings = _defaultJsonSettings,
        };

    /// <summary>
    /// Adds a middleware to the router builder.
    /// </summary>
    /// <param name="middleware">The middleware to add.</param>
    /// <returns>The current <see cref="WireMockServerRouterBuilder"/> instance.</returns>
    public WireMockServerRouterBuilder Use(WireMockMiddleware middleware)
    {
        _middlewareCollection.Enqueue(middleware);
        return this;
    }

    /// <summary>
    /// Sets the default JSON serializer settings for the router.
    /// </summary>
    /// <param name="defaultJsonSettings">The default JSON serializer settings.</param>
    /// <returns>The current <see cref="WireMockServerRouterBuilder"/> instance.</returns>
    public WireMockServerRouterBuilder WithDefaultJsonSettings(
        JsonSerializerSettings? defaultJsonSettings)
    {
        _defaultJsonSettings = defaultJsonSettings;
        return this;
    }
}
