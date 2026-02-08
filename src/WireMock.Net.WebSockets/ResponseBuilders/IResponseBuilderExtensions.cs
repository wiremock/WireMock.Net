// Copyright © WireMock.Net

using System;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Stef.Validation;

namespace WireMock.ResponseBuilders;

/// <summary>
/// IResponseBuilderExtensions extensions for WebSockets.
/// </summary>
// ReSharper disable once InconsistentNaming
public static class IResponseBuilderExtensions
{
    private static readonly ConditionalWeakTable<IResponseBuilder, WebSocketConfiguration> WebSocketConfigs = new();

    /// <summary>
    /// Set a WebSocket handler function.
    /// </summary>
    /// <param name="responseBuilder">The response builder.</param>
    /// <param name="handler">The handler function that receives the WebSocket and request context.</param>
    /// <returns>The response builder.</returns>
    public static IResponseBuilder WithWebSocketHandler(this IResponseBuilder responseBuilder, Func<WebSocketHandlerContext, Task> handler)
    {
        Guard.NotNull(responseBuilder);
        Guard.NotNull(handler);

        var config = GetOrCreateConfig(responseBuilder);
        config.Handler = handler;
        return responseBuilder;
    }

    /// <summary>
    /// Set a WebSocket handler using the raw WebSocket object.
    /// </summary>
    /// <param name="responseBuilder">The response builder.</param>
    /// <param name="handler">The handler function that receives the WebSocket.</param>
    /// <returns>The response builder.</returns>
    public static IResponseBuilder WithWebSocketHandler(this IResponseBuilder responseBuilder, Func<WebSocket, Task> handler)
    {
        Guard.NotNull(responseBuilder);
        Guard.NotNull(handler);

        var config = GetOrCreateConfig(responseBuilder);
        // Wrap the WebSocket handler to accept the context
        config.Handler = ctx => handler(ctx.WebSocket);
        return responseBuilder;
    }

    /// <summary>
    /// Set a message-based handler for processing WebSocket messages.
    /// </summary>
    /// <param name="responseBuilder">The response builder.</param>
    /// <param name="handler">The handler function that processes messages and returns responses.</param>
    /// <returns>The response builder.</returns>
    public static IResponseBuilder WithWebSocketMessageHandler(this IResponseBuilder responseBuilder, Func<WebSocketMessage, Task<WebSocketMessage?>> handler)
    {
        Guard.NotNull(responseBuilder);
        Guard.NotNull(handler);

        var config = GetOrCreateConfig(responseBuilder);
        config.MessageHandler = handler;
        return responseBuilder;
    }

    /// <summary>
    /// Set the keep-alive interval for the WebSocket connection.
    /// </summary>
    /// <param name="responseBuilder">The response builder.</param>
    /// <param name="interval">The keep-alive interval.</param>
    /// <returns>The response builder.</returns>
    public static IResponseBuilder WithWebSocketKeepAlive(this IResponseBuilder responseBuilder, TimeSpan interval)
    {
        Guard.NotNull(responseBuilder);

        var config = GetOrCreateConfig(responseBuilder);
        config.KeepAliveInterval = interval;
        return responseBuilder;
    }

    /// <summary>
    /// Set the connection timeout.
    /// </summary>
    /// <param name="responseBuilder">The response builder.</param>
    /// <param name="timeout">The connection timeout.</param>
    /// <returns>The response builder.</returns>
    public static IResponseBuilder WithWebSocketTimeout(this IResponseBuilder responseBuilder, TimeSpan timeout)
    {
        Guard.NotNull(responseBuilder);

        var config = GetOrCreateConfig(responseBuilder);
        config.Timeout = timeout;
        return responseBuilder;
    }

    /// <summary>
    /// Send a specific message over the WebSocket.
    /// </summary>
    /// <param name="responseBuilder">The response builder.</param>
    /// <param name="message">The message to send.</param>
    /// <returns>The response builder.</returns>
    public static IResponseBuilder WithWebSocketMessage(this IResponseBuilder responseBuilder, WebSocketMessage message)
    {
        Guard.NotNull(responseBuilder);
        Guard.NotNull(message);

        var config = GetOrCreateConfig(responseBuilder);
        // Create a handler that sends the specified message
        config.Handler = async ctx =>
        {
            var data = message.IsBinary && message.RawData != null
                ? message.RawData
                : System.Text.Encoding.UTF8.GetBytes(message.TextData ?? string.Empty);

            await ctx.WebSocket.SendAsync(
                new ArraySegment<byte>(data),
                message.IsBinary ? WebSocketMessageType.Binary : WebSocketMessageType.Text,
                true,
                System.Threading.CancellationToken.None).ConfigureAwait(false);
        };

        return responseBuilder;
    }

    /// <summary>
    /// Get the WebSocket configuration for a response builder.
    /// </summary>
    /// <param name="responseBuilder">The response builder.</param>
    /// <returns>The WebSocket configuration, or null if not configured.</returns>
    internal static WebSocketConfiguration? GetWebSocketConfiguration(this IResponseBuilder responseBuilder)
    {
        Guard.NotNull(responseBuilder);
        return WebSocketConfigs.TryGetValue(responseBuilder, out var config) ? config : null;
    }

    private static WebSocketConfiguration GetOrCreateConfig(IResponseBuilder responseBuilder)
    {
        if (WebSocketConfigs.TryGetValue(responseBuilder, out var existing))
        {
            return existing;
        }

        var config = new WebSocketConfiguration();
        WebSocketConfigs.Add(responseBuilder, config);
        return config;
    }

    /// <summary>
    /// Internal configuration holder for WebSocket settings.
    /// </summary>
    internal class WebSocketConfiguration
    {
        public Func<WebSocketHandlerContext, Task>? Handler { get; set; }

        public Func<WebSocketMessage, Task<WebSocketMessage?>>? MessageHandler { get; set; }

        public TimeSpan? KeepAliveInterval { get; set; }

        public TimeSpan? Timeout { get; set; }
    }
}