// Copyright © WireMock.Net

using System.Net.WebSockets;
using JetBrains.Annotations;
using WireMock.WebSockets;

namespace WireMock.Server;

public partial class WireMockServer
{
    /// <summary>
    /// Get all active WebSocket connections
    /// </summary>
    [PublicAPI]
    public IReadOnlyCollection<WireMockWebSocketContext> GetWebSocketConnections()
    {
        return _options.WebSocketRegistries.Values
            .SelectMany(r => r.GetConnections())
            .ToList();
    }

    /// <summary>
    /// Get WebSocket connections for a specific mapping
    /// </summary>
    [PublicAPI]
    public IReadOnlyCollection<WireMockWebSocketContext> GetWebSocketConnections(Guid mappingGuid)
    {
        return _options.WebSocketRegistries.TryGetValue(mappingGuid, out var registry) ? registry.GetConnections() : [];
    }

    /// <summary>
    /// Close a specific WebSocket connection
    /// </summary>
    [PublicAPI]
    public async Task CloseWebSocketConnectionAsync(
        Guid connectionId,
        WebSocketCloseStatus closeStatus = WebSocketCloseStatus.NormalClosure,
        string statusDescription = "Closed by server",
        CancellationToken cancellationToken = default)
    {
        foreach (var registry in _options.WebSocketRegistries.Values)
        {
            if (registry.TryGetConnection(connectionId, out var connection) && !cancellationToken.IsCancellationRequested)
            {
                await connection.CloseAsync(closeStatus, statusDescription, cancellationToken);
                return;
            }
        }
    }

    /// <summary>
    /// Broadcast a text message to all WebSocket connections in a specific mapping
    /// </summary>
    [PublicAPI]
    public async Task BroadcastToWebSocketsAsync(Guid mappingGuid, string text, Guid? excludeConnectionId = null, CancellationToken cancellationToken = default)
    {
        if (_options.WebSocketRegistries.TryGetValue(mappingGuid, out var registry))
        {
            await registry.BroadcastAsync(text, excludeConnectionId, cancellationToken);
        }
    }

    /// <summary>
    /// Broadcast a text message to all WebSocket connections
    /// </summary>
    [PublicAPI]
    public async Task BroadcastToAllWebSocketsAsync(string text, Guid? excludeConnectionId = null, CancellationToken cancellationToken = default)
    {
        foreach (var registry in _options.WebSocketRegistries.Values)
        {
            await registry.BroadcastAsync(text, excludeConnectionId, cancellationToken);
        }
    }

    /// <summary>
    /// Broadcast a binary message to all WebSocket connections in a specific mapping
    /// </summary>
    [PublicAPI]
    public async Task BroadcastToWebSocketsAsync(Guid mappingGuid, byte[] bytes, Guid? excludeConnectionId = null, CancellationToken cancellationToken = default)
    {
        if (_options.WebSocketRegistries.TryGetValue(mappingGuid, out var registry))
        {
            await registry.BroadcastAsync(bytes, excludeConnectionId, cancellationToken);
        }
    }

    /// <summary>
    /// Broadcast a binary message to all WebSocket connections
    /// </summary>
    [PublicAPI]
    public async Task BroadcastToAllWebSocketsAsync(byte[] bytes, Guid? excludeConnectionId = null, CancellationToken cancellationToken = default)
    {
        foreach (var registry in _options.WebSocketRegistries.Values)
        {
            await registry.BroadcastAsync(bytes, excludeConnectionId, cancellationToken);
        }
    }
}