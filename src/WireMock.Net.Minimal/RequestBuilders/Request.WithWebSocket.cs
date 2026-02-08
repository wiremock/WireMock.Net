// Copyright © WireMock.Net

using Stef.Validation;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.Types;

namespace WireMock.RequestBuilders;

/// <summary>
/// WebSocket-specific request matching extensions
/// </summary>
public partial class Request
{
    /// <summary>
    /// Match WebSocket connection requests (checks for upgrade headers).
    /// This automatically matches the HTTP upgrade headers required for WebSocket handshake.
    /// </summary>
    /// <returns>The request builder for chaining</returns>
    public IRequestBuilder WithWebSocket()
    {
        Add(new RequestMessageHeaderMatcher(MatchBehaviour.AcceptOnMatch, "Upgrade", "websocket", ignoreCase: true));
        Add(new RequestMessageHeaderMatcher(MatchBehaviour.AcceptOnMatch, "Connection", "*Upgrade*", ignoreCase: true));

        return this;
    }

    /// <summary>
    /// Convenience method: match WebSocket connection by path and automatically add upgrade headers.
    /// Equivalent to: WithPath(path).WithWebSocket()
    /// </summary>
    /// <param name="path">WebSocket path (e.g., "/ws", "/api/chat")</param>
    /// <returns>The request builder for chaining</returns>
    public IRequestBuilder WithWebSocketPath(string path)
    {
        Guard.NotNullOrWhiteSpace(path);

        WithPath(path);
        return WithWebSocket();
    }

    /// <summary>
    /// Match specific WebSocket subprotocol in Sec-WebSocket-Protocol header.
    /// Used for protocol versioning or multiple protocol support.
    /// </summary>
    /// <param name="subprotocol">Subprotocol name (e.g., "chat", "superchat", "v1")</param>
    /// <returns>The request builder for chaining</returns>
    public IRequestBuilder WithWebSocketSubprotocol(string subprotocol)
    {
        Guard.NotNullOrWhiteSpace(subprotocol);

        Add(new RequestMessageHeaderMatcher(MatchBehaviour.AcceptOnMatch, "Sec-WebSocket-Protocol", subprotocol, ignoreCase: false));

        return this;
    }

    /// <summary>
    /// Match WebSocket with specific version (typically 13 per RFC 6455).
    /// </summary>
    /// <param name="version">WebSocket version number</param>
    /// <returns>The request builder for chaining</returns>
    public IRequestBuilder WithWebSocketVersion(string version = "13")
    {
        Guard.NotNullOrWhiteSpace(version);

        Add(new RequestMessageHeaderMatcher(MatchBehaviour.AcceptOnMatch, "Sec-WebSocket-Version", version, ignoreCase: false));

        return this;
    }

    /// <summary>
    /// Match WebSocket with client origin (CORS validation).
    /// </summary>
    /// <param name="origin">Origin URL</param>
    /// <returns>The request builder for chaining</returns>
    public IRequestBuilder WithWebSocketOrigin(string origin)
    {
        Guard.NotNullOrWhiteSpace(origin);

        Add(new RequestMessageHeaderMatcher(MatchBehaviour.AcceptOnMatch, "Origin", origin, ignoreCase: false));

        return this;
    }
}
