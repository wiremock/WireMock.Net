// Copyright © WireMock.Net

using System.Collections.Generic;
using WireMock.Types;

namespace WireMock.WebSockets;

/// <summary>
/// Represents a WebSocket connection request for matching purposes.
/// </summary>
public class WebSocketConnectRequest
{
    /// <summary>
    /// Gets the request path.
    /// </summary>
    public string Path { get; init; } = string.Empty;

    /// <summary>
    /// Gets the request headers.
    /// </summary>
    public IDictionary<string, WireMockList<string>> Headers { get; init; } = new Dictionary<string, WireMockList<string>>();

    /// <summary>
    /// Gets the requested subprotocols.
    /// </summary>
    public IList<string> SubProtocols { get; init; } = new List<string>();

    /// <summary>
    /// Gets the remote address (client IP).
    /// </summary>
    public string? RemoteAddress { get; init; }

    /// <summary>
    /// Gets the local address (server IP).
    /// </summary>
    public string? LocalAddress { get; init; }
}
