// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
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
        string statusDescription = "Closed by server")
    {
        foreach (var registry in _options.WebSocketRegistries.Values)
        {
            if (registry.TryGetConnection(connectionId, out var connection))
            {
                await connection.CloseAsync(closeStatus, statusDescription);
                return;
            }
        }
    }

    /// <summary>
    /// Broadcast a message to all WebSocket connections in a specific mapping
    /// </summary>
    [PublicAPI]
    public async Task BroadcastToWebSocketsAsync(Guid mappingGuid, string text)
    {
        if (_options.WebSocketRegistries.TryGetValue(mappingGuid, out var registry))
        {
            await registry.BroadcastTextAsync(text);
        }
    }

    /// <summary>
    /// Broadcast a message to all WebSocket connections
    /// </summary>
    [PublicAPI]
    public async Task BroadcastToAllWebSocketsAsync(string text)
    {
        foreach (var registry in _options.WebSocketRegistries.Values)
        {
            await registry.BroadcastTextAsync(text);
        }
    }
}