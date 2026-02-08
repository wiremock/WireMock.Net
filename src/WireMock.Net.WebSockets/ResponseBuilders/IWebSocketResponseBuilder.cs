// Copyright © WireMock.Net

using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using WireMock.ResponseBuilders;

namespace WireMock.WebSockets.ResponseBuilders;

/// <summary>
/// WebSocket-specific response builder interface.
/// </summary>
public interface IWebSocketResponseBuilder : IResponseBuilder
{
    /// <summary>
    /// Set a WebSocket handler function.
    /// </summary>
    /// <param name="handler">The handler function that receives the WebSocket and request context.</param>
    /// <returns>The response builder.</returns>
    IWebSocketResponseBuilder WithWebSocketHandler(Func<WebSocketHandlerContext, Task> handler);

    /// <summary>
    /// Set a WebSocket handler using the raw WebSocket object.
    /// </summary>
    /// <param name="handler">The handler function that receives the WebSocket.</param>
    /// <returns>The response builder.</returns>
    IWebSocketResponseBuilder WithWebSocketHandler(Func<WebSocket, Task> handler);

    /// <summary>
    /// Set a message-based handler for processing WebSocket messages.
    /// </summary>
    /// <param name="handler">The handler function that processes messages and returns responses.</param>
    /// <returns>The response builder.</returns>
    IWebSocketResponseBuilder WithWebSocketMessageHandler(Func<WebSocketMessage, Task<WebSocketMessage?>> handler);

    /// <summary>
    /// Set the keep-alive interval for the WebSocket connection.
    /// </summary>
    /// <param name="interval">The keep-alive interval.</param>
    /// <returns>The response builder.</returns>
    IWebSocketResponseBuilder WithWebSocketKeepAlive(TimeSpan interval);

    /// <summary>
    /// Set the connection timeout.
    /// </summary>
    /// <param name="timeout">The connection timeout.</param>
    /// <returns>The response builder.</returns>
    IWebSocketResponseBuilder WithWebSocketTimeout(TimeSpan timeout);

    /// <summary>
    /// Send a specific message over the WebSocket.
    /// </summary>
    /// <param name="message">The message to send.</param>
    /// <returns>The response builder.</returns>
    IWebSocketResponseBuilder WithWebSocketMessage(WebSocketMessage message);
}
