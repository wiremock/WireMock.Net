// Copyright © WireMock.Net

using System;
using WireMock.ResponseProviders;
using WireMock.Settings;
using WireMock.WebSockets;

namespace WireMock.ResponseBuilders;

/// <summary>
/// The WebSocketResponseBuilder interface.
/// </summary>
public interface IWebSocketResponseBuilder : IResponseProvider
{
    /// <summary>
    /// Configure WebSocket response behavior
    /// </summary>
    IResponseBuilder WithWebSocket(Action<IWebSocketBuilder> configure);

    /// <summary>
    /// Proxy WebSocket to another server
    /// </summary>
    IResponseBuilder WithWebSocketProxy(string targetUrl);

    /// <summary>
    /// Proxy WebSocket to another server with settings
    /// </summary>
    IResponseBuilder WithWebSocketProxy(ProxyAndRecordSettings settings);
}