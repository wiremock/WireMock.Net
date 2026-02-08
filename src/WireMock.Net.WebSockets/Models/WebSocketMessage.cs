// Copyright © WireMock.Net

using System;
using System.Net.WebSockets;

namespace WireMock.WebSockets;

/// <summary>
/// Represents a WebSocket message.
/// </summary>
public class WebSocketMessage
{
    /// <summary>
    /// Gets or sets the message type.
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp when the message was created.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the message data.
    /// </summary>
    public object? Data { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this is a binary message.
    /// </summary>
    public bool IsBinary { get; set; }

    /// <summary>
    /// Gets or sets the raw message content (for binary messages).
    /// </summary>
    public byte[]? RawData { get; set; }

    /// <summary>
    /// Gets or sets the text content (for text messages).
    /// </summary>
    public string? TextData { get; set; }
}
