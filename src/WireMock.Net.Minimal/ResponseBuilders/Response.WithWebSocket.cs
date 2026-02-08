// Copyright © WireMock.Net

using System;
using System.Threading.Tasks;
using Stef.Validation;
using WireMock.Models;

namespace WireMock.ResponseBuilders;

/// <summary>
/// WebSocket-specific response builder extensions
/// </summary>
public partial class Response
{
    /// <summary>
    /// Gets the WebSocket response for this HTTP response.
    /// </summary>
    public IWebSocketResponse? WebSocketResponse { get; set; }

    /// <summary>
    /// Set a WebSocket response using a chainable builder.
    /// </summary>
    public IResponseBuilder WithWebSocket(Func<IWebSocketResponseBuilder, IResponseBuilder> configureWebSocket)
    {
        Guard.NotNull(configureWebSocket);

        var builder = new WebSocketResponseBuilder(this);
        var result = configureWebSocket(builder);
        WebSocketResponse = builder.Build();

        return result;
    }

    /// <summary>
    /// Set a pre-built WebSocket response.
    /// </summary>
    public IResponseBuilder WithWebSocket(IWebSocketResponse webSocketResponse)
    {
        Guard.NotNull(webSocketResponse);

        WebSocketResponse = webSocketResponse;
        return this;
    }

    /// <summary>
    /// Set the subprotocol for WebSocket negotiation.
    /// </summary>
    public IResponseBuilder WithWebSocketSubprotocol(string subprotocol)
    {
        Guard.NotNullOrEmpty(subprotocol);

        if (WebSocketResponse == null)
        {
            WebSocketResponse = new WebSocketResponse();
        }

        ((WebSocketResponse)WebSocketResponse).Subprotocol = subprotocol;
        return this;
    }

    /// <summary>
    /// Add a callback for dynamic WebSocket responses.
    /// </summary>
    public IResponseBuilder WithWebSocketCallback(Func<IRequestMessage, Task<IWebSocketMessage[]>> callback)
    {
        Guard.NotNull(callback);

        WebSocketCallback = callback;
        return this;
    }

    /// <summary>
    /// Gets or sets the WebSocket callback function.
    /// </summary>
    public Func<IRequestMessage, Task<IWebSocketMessage[]>>? WebSocketCallback { get; set; }
}
