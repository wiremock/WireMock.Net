// Copyright © WireMock.Net

using WireMock.RequestBuilders;

namespace WireMock.WebSockets.RequestBuilders;

/// <summary>
/// WebSocket-specific request builder interface.
/// </summary>
public interface IWebSocketRequestBuilder : IRequestBuilder
{
    /// <summary>
    /// Match WebSocket requests to a specific path.
    /// </summary>
    /// <param name="path">The path to match.</param>
    /// <returns>The request builder.</returns>
    IWebSocketRequestBuilder WithWebSocketPath(string path);

    /// <summary>
    /// Match WebSocket requests with specific subprotocols.
    /// </summary>
    /// <param name="subProtocols">The acceptable subprotocols.</param>
    /// <returns>The request builder.</returns>
    IWebSocketRequestBuilder WithWebSocketSubprotocol(params string[] subProtocols);

    /// <summary>
    /// Match WebSocket requests based on custom headers.
    /// </summary>
    /// <param name="headers">The header key-value pairs to match.</param>
    /// <returns>The request builder.</returns>
    IWebSocketRequestBuilder WithCustomHandshakeHeaders(params (string Key, string Value)[] headers);
}
