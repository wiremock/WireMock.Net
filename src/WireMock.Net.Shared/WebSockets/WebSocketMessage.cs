// Copyright © WireMock.Net

using System;
using System.Net.WebSockets;

namespace WireMock.WebSockets;

/// <summary>
/// Represents a WebSocket message
/// </summary>
public class WebSocketMessage
{
    /// <summary>
    /// The message type (Text or Binary)
    /// </summary>
    public WebSocketMessageType MessageType { get; set; }

    /// <summary>
    /// Text content (when MessageType is Text)
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// Binary content (when MessageType is Binary)
    /// </summary>
    public byte[]? Bytes { get; set; }

    /// <summary>
    /// Indicates whether this is the final message
    /// </summary>
    public bool EndOfMessage { get; set; }

    /// <summary>
    /// Timestamp when the message was received
    /// </summary>
    public DateTime Timestamp { get; set; }
}
