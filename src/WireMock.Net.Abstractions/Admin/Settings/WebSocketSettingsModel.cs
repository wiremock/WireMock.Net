// Copyright © WireMock.Net

namespace WireMock.Admin.Settings;

/// <summary>
/// WebSocket Settings Model
/// </summary>
[FluentBuilder.AutoGenerateBuilder]
public class WebSocketSettingsModel
{
    /// <summary>
    /// Maximum number of concurrent WebSocket connections (default: 100)
    /// </summary>
    public int MaxConnections { get; set; } = 100;

    /// <summary>
    /// Default receive buffer size in bytes (default: 4096)
    /// </summary>
    public int ReceiveBufferSize { get; set; } = 4096;

    /// <summary>
    /// Default keep-alive interval in seconds (default: 30)
    /// </summary>
    public int KeepAliveIntervalSeconds { get; set; } = 30;

    /// <summary>
    /// Maximum message size in bytes (default: 1048576 - 1 MB)
    /// </summary>
    public int MaxMessageSize { get; set; } = 1048576;

    /// <summary>
    /// Enable WebSocket compression (default: true)
    /// </summary>
    public bool EnableCompression { get; set; } = true;

    /// <summary>
    /// Default close timeout in minutes (default: 10)
    /// </summary>
    public int CloseTimeoutMinutes { get; set; } = 10;
}
