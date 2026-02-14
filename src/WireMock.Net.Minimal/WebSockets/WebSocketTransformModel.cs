// Copyright © WireMock.Net

namespace WireMock.WebSockets;

/// <summary>
/// Model for WebSocket message transformation
/// </summary>
internal struct WebSocketTransformModel
{
    /// <summary>
    /// The mapping that matched this WebSocket request
    /// </summary>
    public IMapping Mapping { get; set; }

    /// <summary>
    /// The original request that initiated the WebSocket connection
    /// </summary>
    public IRequestMessage Request { get; set; }

    /// <summary>
    /// The incoming WebSocket message
    /// </summary>
    public WebSocketMessage Message { get; set; }

    /// <summary>
    /// The message data as string
    /// </summary>
    public string? Data { get; set; }
}
