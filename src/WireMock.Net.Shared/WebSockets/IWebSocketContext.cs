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
    Task CloseAsync(WebSocketCloseStatus closeStatus, string statusDescription);

    /// <summary>
    /// Broadcast text message to all connections in this mapping
    /// </summary>
    Task BroadcastTextAsync(string text, CancellationToken cancellationToken = default);
}