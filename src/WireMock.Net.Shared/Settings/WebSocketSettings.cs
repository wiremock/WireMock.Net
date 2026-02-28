// Copyright © WireMock.Net

using WireMock.Constants;

namespace WireMock.Settings;

/// <summary>
/// WebSocket-specific settings
/// </summary>
public class WebSocketSettings
{
    /// <summary>
    /// Maximum number of concurrent WebSocket connections (default: 100)
    /// </summary>
    public int MaxConnections { get; set; } = 100;

    /// <summary>
    /// Default keep-alive interval (default: 30 seconds)
    /// </summary>
    public int KeepAliveIntervalSeconds { get; set; } = WebSocketConstants.DefaultKeepAliveIntervalSeconds;
}