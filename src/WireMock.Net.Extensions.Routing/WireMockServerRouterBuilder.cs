// Copyright © WireMock.Net

using System.Collections.Concurrent;
using JsonConverter.Abstractions;
using WireMock.Net.Extensions.Routing.Delegates;
using WireMock.Server;

namespace WireMock.Net.Extensions.Routing;

/// <summary>
/// Provides a builder for configuring and creating a <see cref="WireMockRouter"/> with middleware and JSON settings.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="WireMockServerRouterBuilder"/> class.
/// </remarks>
/// <param name="server">The WireMock server instance.</param>
public sealed class WireMockServerRouterBuilder(WireMockServer server)
{
    private readonly WireMockServer _server = server;

    private readonly ConcurrentQueue<WireMockMiddleware> _middlewareCollection = new();

    private IJsonConverter? _defaultJsonConverter;

    private JsonConverterOptions? _defaultJsonOptions;

    /// <summary>
    /// Builds a <see cref="WireMockRouter"/> with the configured middleware and JSON settings.
    /// </summary>
    /// <returns>The configured <see cref="WireMockRouter"/>.</returns>
    public WireMockRouter Build() =>
        new(_server)
        {
            MiddlewareCollection = _middlewareCollection,
            DefaultJsonConverter = _defaultJsonConverter,
            DefaultJsonOptions = _defaultJsonOptions,
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
    /// Sets the default <see cref="IJsonConverter"/>.
    /// </summary>
    /// <param name="defaultJsonConverter">the default <see cref="IJsonConverter"/></param>
    /// <returns>The current <see cref="WireMockServerRouterBuilder"/> instance.</returns>
    public WireMockServerRouterBuilder WithDefaultJsonConverter(
        IJsonConverter? defaultJsonConverter)
    {
        _defaultJsonConverter = defaultJsonConverter;
        return this;
    }

    /// <summary>
    /// Sets the default <see cref="JsonConverterOptions"/> [optional].
    /// </summary>
    /// <param name="defaultJsonOptions">the default <see cref="JsonConverterOptions"/> [optional]</param>
    /// <returns>The current <see cref="WireMockServerRouterBuilder"/> instance.</returns>
    public WireMockServerRouterBuilder WithDefaultJsonOptions(
        JsonConverterOptions? defaultJsonOptions)
    {
        _defaultJsonOptions = defaultJsonOptions;
        return this;
    }
}
