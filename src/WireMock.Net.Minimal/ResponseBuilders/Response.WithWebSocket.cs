// Copyright © WireMock.Net

using WireMock.Settings;
using WireMock.WebSockets;

namespace WireMock.ResponseBuilders;

public partial class Response
{
    /// <summary>
    /// Internal property to store WebSocket builder configuration
    /// </summary>
    internal WebSocketBuilder? WebSocketBuilder { get; set; }

    /// <summary>
    /// Configure WebSocket response behavior
    /// </summary>
    public IResponseBuilder WithWebSocket(Action<IWebSocketBuilder> configure)
    {
        var builder = new WebSocketBuilder(this);
        configure(builder);

        WebSocketBuilder = builder;
        
        return this;
    }

    /// <summary>
    /// Proxy WebSocket to another server
    /// </summary>
    public IResponseBuilder WithWebSocketProxy(string targetUrl)
    {
        return WithWebSocketProxy(new ProxyAndRecordSettings { Url = targetUrl });
    }

    /// <summary>
    /// Proxy WebSocket to another server with settings
    /// </summary>
    public IResponseBuilder WithWebSocketProxy(ProxyAndRecordSettings settings)
    {
        var builder = new WebSocketBuilder(this);
        builder.WithProxy(settings);

        WebSocketBuilder = builder;
        
        return this;
    }
}