// Copyright © WireMock.Net

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
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
    public sealed class WireMockRouter
{
    private readonly WireMockServer _server;

    /// <summary>
    /// Initializes a new instance of the <see cref="WireMockRouter"/> class.
    /// </summary>
    /// <param name="server">The WireMock server instance.</param>
    public WireMockRouter(WireMockServer server)
    {
        _server = server;
    }

    /// <summary>
    /// Gets or initializes the collection of middleware for the router.
    /// </summary>
    public IReadOnlyCollection<WireMockMiddleware> MiddlewareCollection { get; init; } = [];

    /// <summary>
    /// Gets or initializes the default JSON serializer settings for the router.
    /// </summary>
    public JsonSerializerSettings? DefaultJsonSettings { get; init; }

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
    /// <param name="jsonSettings">Optional JSON serializer settings.</param>
    /// <returns>The current <see cref="WireMockRouter"/> instance.</returns>
    public WireMockRouter Map<TRequest>(
        string method,
        string pattern,
        Func<WireMockRequestInfo<TRequest>, object?> requestHandler,
        JsonSerializerSettings? jsonSettings = null)
    {
        object? CreateBody(IRequestMessage request) =>
            requestHandler(CreateRequestInfo<TRequest>(request, pattern, jsonSettings));

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
        IRequestMessage request, string pattern, JsonSerializerSettings? jsonSettings = null)
    {
        var requestInfo = CreateRequestInfo(request, pattern);
        return new WireMockRequestInfo<TRequest>(requestInfo.Request)
        {
            RouteArgs = requestInfo.RouteArgs,
            Body = requestInfo.Request.GetBodyAsJson<TRequest>(jsonSettings ?? DefaultJsonSettings),
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
