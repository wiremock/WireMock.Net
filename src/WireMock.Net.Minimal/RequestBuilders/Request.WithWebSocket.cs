// Copyright © WireMock.Net

using WireMock.Matchers;
using WireMock.Matchers.Request;

namespace WireMock.RequestBuilders;

public partial class Request
{
    /// <inheritdoc />
    public bool IsWebSocket { get; private set; }

    /// <inheritdoc />
    public IRequestBuilder WithWebSocketUpgrade(params string[] protocols)
    {
        _requestMatchers.Add(new RequestMessageHeaderMatcher(
            MatchBehaviour.AcceptOnMatch,
            MatchOperator.Or,
            "Upgrade",
            true,
            new ExactMatcher(true, "websocket")
        ));

        _requestMatchers.Add(new RequestMessageHeaderMatcher(
            MatchBehaviour.AcceptOnMatch,
            MatchOperator.Or,
            "Connection",
            true,
            new WildcardMatcher("*Upgrade*", true)
        ));

        if (protocols.Length > 0)
        {
            _requestMatchers.Add(new RequestMessageHeaderMatcher(
                MatchBehaviour.AcceptOnMatch,
                MatchOperator.Or,
                "Sec-WebSocket-Protocol",
                true,
                protocols.Select(p => new ExactMatcher(true, p)).ToArray()
            ));
        }

        IsWebSocket = true;

        return this;
    }
}