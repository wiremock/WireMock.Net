// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.Net.WebSockets;

namespace WireMock.WebSockets;

/// <summary>
/// Represents the context for a WebSocket handler.
/// </summary>
public class WebSocketHandlerContext
{
    /// <summary>
    /// Gets the WebSocket instance.
    /// </summary>
    public WebSocket WebSocket { get; init; } = null!;

    /// <summary>
    /// Gets the request message.
    /// </summary>
    public IRequestMessage RequestMessage { get; init; } = null!;

    /// <summary>
    /// Gets the request headers.
    /// </summary>
    public IDictionary<string, string[]> Headers { get; init; } = new Dictionary<string, string[]>();

    /// <summary>
    /// Gets the subprotocol negotiated for this connection.
    /// </summary>
    public string? SubProtocol { get; init; }

    /// <summary>
    /// Gets or sets user state associated with the connection.
    /// </summary>
    public IDictionary<string, object> UserState { get; init; } = new Dictionary<string, object>();
}
