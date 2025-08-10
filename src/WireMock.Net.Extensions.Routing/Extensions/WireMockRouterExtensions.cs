// Copyright © WireMock.Net

using Newtonsoft.Json;
using WireMock.Net.Extensions.Routing.Models;

namespace WireMock.Net.Extensions.Routing.Extensions;

/// <summary>
/// Provides extension methods for mapping HTTP routes to handlers in <see cref="WireMockRouter"/>.
/// </summary>
public static class WireMockRouterExtensions
{
    /// <summary>
    /// Maps a GET request to a synchronous request handler.
    /// </summary>
    /// <param name="source">The router to extend.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <param name="requestHandler">The request handler function.</param>
    /// <returns>The current <see cref="WireMockRouter"/> instance.</returns>
    public static WireMockRouter MapGet(
        this WireMockRouter source,
        string pattern,
        Func<WireMockRequestInfo, object?> requestHandler) =>
        source.Map(HttpMethod.Get.Method, pattern, requestHandler);

    /// <summary>
    /// Maps a GET request to an asynchronous request handler.
    /// </summary>
    /// <param name="source">The router to extend.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <param name="requestHandler">The asynchronous request handler function.</param>
    /// <returns>The current <see cref="WireMockRouter"/> instance.</returns>
    public static WireMockRouter MapGet(
        this WireMockRouter source,
        string pattern,
        Func<WireMockRequestInfo, Task<object?>> requestHandler) =>
        source.Map(HttpMethod.Get.Method, pattern, requestHandler);

    /// <summary>
    /// Maps a POST request to a synchronous request handler.
    /// </summary>
    /// <param name="source">The router to extend.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <param name="requestHandler">The request handler function.</param>
    /// <returns>The current <see cref="WireMockRouter"/> instance.</returns>
    public static WireMockRouter MapPost(
        this WireMockRouter source,
        string pattern,
        Func<WireMockRequestInfo, object?> requestHandler) =>
        source.Map(HttpMethod.Post.Method, pattern, requestHandler);

    /// <summary>
    /// Maps a POST request to an asynchronous request handler.
    /// </summary>
    /// <param name="source">The router to extend.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <param name="requestHandler">The asynchronous request handler function.</param>
    /// <returns>The current <see cref="WireMockRouter"/> instance.</returns>
    public static WireMockRouter MapPost(
        this WireMockRouter source,
        string pattern,
        Func<WireMockRequestInfo, Task<object?>> requestHandler) =>
        source.Map(HttpMethod.Post.Method, pattern, requestHandler);

    /// <summary>
    /// Maps a POST request to a synchronous request handler with a typed body.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request body.</typeparam>
    /// <param name="source">The router to extend.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <param name="requestHandler">The request handler function.</param>
    /// <param name="jsonSettings">Optional JSON serializer settings.</param>
    /// <returns>The current <see cref="WireMockRouter"/> instance.</returns>
    public static WireMockRouter MapPost<TRequest>(
        this WireMockRouter source,
        string pattern,
        Func<WireMockRequestInfo<TRequest>, object?> requestHandler,
        JsonSerializerSettings? jsonSettings = null) =>
        source.Map(HttpMethod.Post.Method, pattern, requestHandler, jsonSettings);

    /// <summary>
    /// Maps a POST request to an asynchronous request handler with a typed body.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request body.</typeparam>
    /// <param name="source">The router to extend.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <param name="requestHandler">The asynchronous request handler function.</param>
    /// <param name="jsonSettings">Optional JSON serializer settings.</param>
    /// <returns>The current <see cref="WireMockRouter"/> instance.</returns>
    public static WireMockRouter MapPost<TRequest>(
        this WireMockRouter source,
        string pattern,
        Func<WireMockRequestInfo<TRequest>, Task<object?>> requestHandler,
        JsonSerializerSettings? jsonSettings = null) =>
        source.Map(HttpMethod.Post.Method, pattern, requestHandler, jsonSettings);

    /// <summary>
    /// Maps a PUT request to a synchronous request handler.
    /// </summary>
    /// <param name="source">The router to extend.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <param name="requestHandler">The request handler function.</param>
    /// <returns>The current <see cref="WireMockRouter"/> instance.</returns>
    public static WireMockRouter MapPut(
        this WireMockRouter source,
        string pattern,
        Func<WireMockRequestInfo, object?> requestHandler) =>
        source.Map(HttpMethod.Put.Method, pattern, requestHandler);

    /// <summary>
    /// Maps a PUT request to an asynchronous request handler.
    /// </summary>
    /// <param name="source">The router to extend.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <param name="requestHandler">The asynchronous request handler function.</param>
    /// <returns>The current <see cref="WireMockRouter"/> instance.</returns>
    public static WireMockRouter MapPut(
        this WireMockRouter source,
        string pattern,
        Func<WireMockRequestInfo, Task<object?>> requestHandler) =>
        source.Map(HttpMethod.Put.Method, pattern, requestHandler);

    /// <summary>
    /// Maps a PUT request to a synchronous request handler with a typed body.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request body.</typeparam>
    /// <param name="source">The router to extend.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <param name="requestHandler">The request handler function.</param>
    /// <param name="jsonSettings">Optional JSON serializer settings.</param>
    /// <returns>The current <see cref="WireMockRouter"/> instance.</returns>
    public static WireMockRouter MapPut<TRequest>(
        this WireMockRouter source,
        string pattern,
        Func<WireMockRequestInfo<TRequest>, object?> requestHandler,
        JsonSerializerSettings? jsonSettings = null) =>
        source.Map(HttpMethod.Put.Method, pattern, requestHandler, jsonSettings);

    /// <summary>
    /// Maps a PUT request to an asynchronous request handler with a typed body.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request body.</typeparam>
    /// <param name="source">The router to extend.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <param name="requestHandler">The asynchronous request handler function.</param>
    /// <param name="jsonSettings">Optional JSON serializer settings.</param>
    /// <returns>The current <see cref="WireMockRouter"/> instance.</returns>
    public static WireMockRouter MapPut<TRequest>(
        this WireMockRouter source,
        string pattern,
        Func<WireMockRequestInfo<TRequest>, Task<object?>> requestHandler,
        JsonSerializerSettings? jsonSettings = null) =>
        source.Map(HttpMethod.Put.Method, pattern, requestHandler, jsonSettings);

    /// <summary>
    /// Maps a DELETE request to a synchronous request handler.
    /// </summary>
    /// <param name="source">The router to extend.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <param name="requestHandler">The request handler function.</param>
    /// <returns>The current <see cref="WireMockRouter"/> instance.</returns>
    public static WireMockRouter MapDelete(
        this WireMockRouter source,
        string pattern,
        Func<WireMockRequestInfo, object?> requestHandler) =>
        source.Map(HttpMethod.Delete.Method, pattern, requestHandler);

    /// <summary>
    /// Maps a DELETE request to an asynchronous request handler.
    /// </summary>
    /// <param name="source">The router to extend.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <param name="requestHandler">The asynchronous request handler function.</param>
    /// <returns>The current <see cref="WireMockRouter"/> instance.</returns>
    public static WireMockRouter MapDelete(
        this WireMockRouter source,
        string pattern,
        Func<WireMockRequestInfo, Task<object?>> requestHandler) =>
        source.Map(HttpMethod.Delete.Method, pattern, requestHandler);

    /// <summary>
    /// Maps a DELETE request to a synchronous request handler with a typed body.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request body.</typeparam>
    /// <param name="source">The router to extend.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <param name="requestHandler">The request handler function.</param>
    /// <param name="jsonSettings">Optional JSON serializer settings.</param>
    /// <returns>The current <see cref="WireMockRouter"/> instance.</returns>
    public static WireMockRouter MapDelete<TRequest>(
        this WireMockRouter source,
        string pattern,
        Func<WireMockRequestInfo<TRequest>, object?> requestHandler,
        JsonSerializerSettings? jsonSettings = null) =>
        source.Map(HttpMethod.Delete.Method, pattern, requestHandler, jsonSettings);

    /// <summary>
    /// Maps a DELETE request to an asynchronous request handler with a typed body.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request body.</typeparam>
    /// <param name="source">The router to extend.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <param name="requestHandler">The asynchronous request handler function.</param>
    /// <param name="jsonSettings">Optional JSON serializer settings.</param>
    /// <returns>The current <see cref="WireMockRouter"/> instance.</returns>
    public static WireMockRouter MapDelete<TRequest>(
        this WireMockRouter source,
        string pattern,
        Func<WireMockRequestInfo<TRequest>, Task<object?>> requestHandler,
        JsonSerializerSettings? jsonSettings = null) =>
        source.Map(HttpMethod.Delete.Method, pattern, requestHandler, jsonSettings);
}
