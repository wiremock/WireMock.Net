// Copyright © WireMock.Net

using System.Net.WebSockets;
using Microsoft.AspNetCore.Http;

namespace WireMock.WebSockets;

/// <summary>
/// WebSocket context interface for handling WebSocket connections
/// </summary>
public interface IWebSocketContext
{
    /// <summary>
    /// Unique connection identifier
    /// </summary>
    Guid ConnectionId { get; }

    /// <summary>
    /// The ASP.NET Core HttpContext
    /// </summary>
    HttpContext HttpContext { get; }

    /// <summary>
    /// The WebSocket instance
    /// </summary>
    WebSocket WebSocket { get; }

    /// <summary>
    /// The original request that initiated the WebSocket connection
    /// </summary>
    IRequestMessage RequestMessage { get; }

    /// <summary>
    /// The mapping that matched this WebSocket request
    /// </summary>
    IMapping Mapping { get; }

    /// <summary>
    /// Send text message to the client
    /// </summary>
    Task SendAsync(string text, CancellationToken cancellationToken = default);

    /// <summary>
    /// Send binary message to the client
    /// </summary>
    Task SendAsync(byte[] bytes, CancellationToken cancellationToken = default);

    /// <summary>
    /// Close the WebSocket connection
    /// </summary>
    Task CloseAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken = default);

    /// <summary>
    /// Abort the WebSocket connection to immediately close the connection without waiting for the close handshake
    /// </summary>
    void Abort(string? statusDescription = null);

    /// <summary>
    /// Broadcast text message to all connections in this mapping
    /// </summary>
    Task BroadcastAsync(string text, bool excludeSender = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Broadcast binary message to all connections in this mapping
    /// </summary>
    Task BroadcastAsync(byte[] bytes, bool excludeSender = false, CancellationToken cancellationToken = default);
}