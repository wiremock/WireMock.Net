// Copyright © WireMock.Net

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Net.WebSockets;
using static System.Net.Mime.MediaTypeNames;

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
    public async Task BroadcastAsync(string text, Guid? excludeConnectionId, CancellationToken cancellationToken = default)
    {
        var tasks = Filter(excludeConnectionId).Select(c => c.SendAsync(text, cancellationToken));
        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Broadcast binary to all connections
    /// </summary>
    public async Task BroadcastAsync(byte[] bytes, Guid? excludeConnectionId, CancellationToken cancellationToken = default)
    {
        var tasks = Filter(excludeConnectionId).Select(c => c.SendAsync(bytes, cancellationToken));
        await Task.WhenAll(tasks);
    }

    private IEnumerable<WireMockWebSocketContext> Filter(Guid? excludeConnectionId)
    {
        return _connections.Values
            .Where(c =>c.WebSocket.State == WebSocketState.Open && (!excludeConnectionId.HasValue || c.ConnectionId != excludeConnectionId));
    }
}