// Copyright © WireMock.Net

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Net.WebSockets;
using Newtonsoft.Json;

namespace WireMock.WebSockets;

/// <summary>
/// Registry for managing WebSocket connections per mapping
/// </summary>
internal class WebSocketConnectionRegistry
{
    private readonly ConcurrentDictionary<Guid, WireMockWebSocketContext> _connections = new();

    /// <summary>
    /// Add a connection to the registry
    /// </summary>
    public void AddConnection(WireMockWebSocketContext context)
    {
        _connections.TryAdd(context.ConnectionId, context);
    }

    /// <summary>
    /// Remove a connection from the registry
    /// </summary>
    public void RemoveConnection(Guid connectionId)
    {
        _connections.TryRemove(connectionId, out _);
    }

    /// <summary>
    /// Get all connections
    /// </summary>
    public IReadOnlyCollection<WireMockWebSocketContext> GetConnections()
    {
        return _connections.Values.ToList();
    }

    /// <summary>
    /// Try to get a specific connection
    /// </summary>
    public bool TryGetConnection(Guid connectionId, [NotNullWhen(true)] out WireMockWebSocketContext? connection)
    {
        return _connections.TryGetValue(connectionId, out connection);
    }

    /// <summary>
    /// Broadcast text to all connections
    /// </summary>
    public async Task BroadcastTextAsync(string text, CancellationToken cancellationToken = default)
    {
        var tasks = _connections.Values
            .Where(c => c.WebSocket.State == WebSocketState.Open)
            .Select(c => c.SendAsync(text, cancellationToken));

        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Broadcast JSON to all connections
    /// </summary>
    public async Task BroadcastJsonAsync(object data, CancellationToken cancellationToken = default)
    {
        var json = JsonConvert.SerializeObject(data);
        await BroadcastTextAsync(json, cancellationToken);
    }
}