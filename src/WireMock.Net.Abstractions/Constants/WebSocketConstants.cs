// Copyright © WireMock.Net

namespace WireMock.Constants;

/// <summary>
/// WebSocket constants
/// </summary>
public static class WebSocketConstants
{
    /// <summary>
    /// Default receive buffer size for WebSocket messages (4 KB)
    /// </summary>
    public const int DefaultReceiveBufferSize = 4096;

    /// <summary>
    /// Default keep-alive interval in seconds
    /// </summary>
    public const int DefaultKeepAliveIntervalSeconds = 30;

    /// <summary>
    /// Default close timeout in minutes
    /// </summary>
    public const int DefaultCloseTimeoutMinutes = 10;

    /// <summary>
    /// Minimum buffer size for WebSocket operations (1 KB)
    /// </summary>
    public const int MinimumBufferSize = 1024;

    /// <summary>
    /// Default maximum message size (1 MB)
    /// </summary>
    public const int DefaultMaxMessageSize = 1024 * 1024;

    /// <summary>
    /// Proxy forward buffer size (4 KB)
    /// </summary>
    public const int ProxyForwardBufferSize = 4096;
}
