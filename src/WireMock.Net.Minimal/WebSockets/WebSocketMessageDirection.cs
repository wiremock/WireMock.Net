// Copyright © WireMock.Net

namespace WireMock.WebSockets;

/// <summary>
/// Represents the direction of a WebSocket message.
/// </summary>
internal enum WebSocketMessageDirection
{
    /// <summary>
    /// Message received from the client.
    /// </summary>
    Receive,

    /// <summary>
    /// Message sent to the client.
    /// </summary>
    Send
}