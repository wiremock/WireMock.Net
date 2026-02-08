// Copyright © WireMock.Net

namespace WireMock.Models;

/// <summary>
/// Represents a single WebSocket message
/// </summary>
public interface IWebSocketMessage
{
    /// <summary>
    /// Gets the delay in milliseconds before sending this message
    /// </summary>
    int DelayMs { get; }

    /// <summary>
    /// Gets the message body as string (for text frames)
    /// </summary>
    string? BodyAsString { get; }

    /// <summary>
    /// Gets the message body as bytes (for binary frames)
    /// </summary>
    byte[]? BodyAsBytes { get; }

    /// <summary>
    /// Gets a value indicating whether this is a text frame (vs binary)
    /// </summary>
    bool IsText { get; }

    /// <summary>
    /// Gets the unique identifier for this message
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Gets the correlation ID for request/response correlation
    /// </summary>
    string? CorrelationId { get; }
}
