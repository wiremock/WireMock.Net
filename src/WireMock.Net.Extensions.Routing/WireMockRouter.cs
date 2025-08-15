// Copyright © WireMock.Net

using JsonConverter.Abstractions;
using JsonConverter.Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using WireMock.Matchers;
using WireMock.Net.Extensions.Routing.Delegates;
using WireMock.Net.Extensions.Routing.Extensions;
using WireMock.Net.Extensions.Routing.Models;
using WireMock.Net.Extensions.Routing.Utils;
using WireMock.Server;

namespace WireMock.Net.Extensions.Routing;

/// <summary>
/// Provides routing and request mapping functionality for WireMock.Net,
/// mimicking ASP.NET Core Minimal APIs routing style.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="WireMockRouter"/> class.
/// </remarks>
/// <param name="server">The WireMock.Net server instance.</param>
public sealed class WireMockRouter(WireMockServer server)
{
    private readonly WireMockServer _server = server;

    /// <summary>
    /// Gets or initializes the collection of middleware for the router.
    /// </summary>
    public IReadOnlyCollection<WireMockMiddleware> MiddlewareCollection { get; init; } = [];

    /// <summary>
    /// Gets or initializes the default <see cref="IJsonConverter"/> [optional].
    /// </summary>
    public IJsonConverter? DefaultJsonConverter { get; init; }

    /// <summary>
    /// Gets or initializes the default <see cref="JsonConverterOptions"/> [optional].
    /// </summary>
    public JsonConverterOptions? DefaultJsonOptions { get; init; }

    /// <summary>
    /// Maps a route to a synchronous request handler.
    /// </summary>
    /// <param name="method">The HTTP method.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <param name="requestHandler">The request handler function.</param>
    /// <returns>The current <see cref="WireMockRouter"/> instance.</returns>
    public WireMockRouter Map(
        string method, string pattern, Func<WireMockRequestInfo, object?> requestHandler)
    {
        object? CreateResponse(IRequestMessage request) =>
            requestHandler(CreateRequestInfo(request, pattern));

        return Map(method, pattern, CreateResponse);
    }

    /// <summary>
    /// Maps a route to an asynchronous request handler.
    /// </summary>
    /// <param name="method">The HTTP method.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <param name="requestHandler">The asynchronous request handler function.</param>
    /// <returns>The current <see cref="WireMockRouter"/> instance.</returns>
    public WireMockRouter Map(
        string method, string pattern, Func<WireMockRequestInfo, Task<object?>> requestHandler)
    {
        Task<object?> CreateResponseAsync(IRequestMessage request) =>
            requestHandler(CreateRequestInfo(request, pattern));

        return Map(method, pattern, CreateResponseAsync);
    }

    /// <summary>
    /// Maps a route to a request handler with a typed body.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request body.</typeparam>
    /// <param name="method">The HTTP method.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <param name="requestHandler">The request handler function.</param>
    /// <param name="jsonConverter">The <see cref="IJsonConverter"/> [optional]. Default value is NewtonsoftJsonConverter.</param>
    /// <param name="jsonOptions">The <see cref="JsonConverterOptions"/> [optional].</param>
    /// <returns>The current <see cref="WireMockRouter"/> instance.</returns>
    public WireMockRouter Map<TRequest>(
        string method,
        string pattern,
        Func<WireMockRequestInfo<TRequest>, object?> requestHandler,
        IJsonConverter? jsonConverter = null,
        JsonConverterOptions? jsonOptions = null)
    {
        object? CreateBody(IRequestMessage request) =>
            requestHandler(CreateRequestInfo<TRequest>(request, pattern, jsonConverter, jsonOptions));

        return Map(method, pattern, CreateBody);
    }

    private static WireMockRequestInfo CreateRequestInfo(IRequestMessage request, string pattern) =>
        new(request)
        {
            RouteArgs = RoutePattern.GetArgs(pattern, request.Path),
        };

    private static WireMockHttpRequestHandler CreateHttpRequestHandler(
        Func<IRequestMessage, object?> requestHandler) =>
        request => CreateResponseMessageAsync(requestHandler(request));

    private static async Task<ResponseMessage> CreateResponseMessageAsync(object? response)
    {
        var awaitedResponse = response is Task task
            ? await task.ToGenericTaskAsync()
            : response;
        var result = awaitedResponse as IResult ?? Results.Ok(awaitedResponse);
        var httpContext = CreateHttpContext();
        await result.ExecuteAsync(httpContext);
        return await httpContext.Response.ToResponseMessageAsync();
    }

    private static HttpContext CreateHttpContext() =>
        new DefaultHttpContext
        {
            RequestServices = new ServiceCollection().AddLogging().BuildServiceProvider(),
            Response = { Body = new MemoryStream() },
        };

    private WireMockRequestInfo<TRequest> CreateRequestInfo<TRequest>(
        IRequestMessage request,
        string pattern,
        IJsonConverter? jsonConverter = null,
        JsonConverterOptions? jsonOptions = null)
    {
        var requestInfo = CreateRequestInfo(request, pattern);
        var establishedJsonConverter =
            jsonConverter ?? DefaultJsonConverter ?? new NewtonsoftJsonConverter();
        var establishedJsonOptions = jsonOptions ?? DefaultJsonOptions;
        return new WireMockRequestInfo<TRequest>(requestInfo.Request)
        {
            RouteArgs = requestInfo.RouteArgs,
            Body = requestInfo.Request.GetBodyAsJson<TRequest>(
                establishedJsonConverter, establishedJsonOptions),
        };
    }

    private WireMockRouter Map(
        string method, string pattern, Func<IRequestMessage, object?> requestHandler)
    {
        var matcher = new RegexMatcher(RoutePattern.ToRegex(pattern), ignoreCase: true);
        var httpRequestHandler =
            CreateHttpRequestHandler(requestHandler).UseMiddlewareCollection(MiddlewareCollection);
        _server.Map(method, matcher, httpRequestHandler);
        return this;
    }
}
