// Copyright © WireMock.Net
using WireMock.Matchers.Request;

namespace WireMock.RequestBuilders;

/// <summary>
/// The BodyRequestBuilder interface.
/// </summary>
public interface IWebSocketRequestBuilder : IRequestMatcher
{
    /// <summary>
    /// Match WebSocket upgrade with optional protocols.
    /// </summary>
    IRequestBuilder WithWebSocketUpgrade(params string[] protocols);
}