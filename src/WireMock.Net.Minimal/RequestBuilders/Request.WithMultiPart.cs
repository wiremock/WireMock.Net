// Copyright © WireMock.Net

using WireMock.Matchers;
using WireMock.Matchers.Request;

namespace WireMock.RequestBuilders;

public partial class Request
{
    /// <inheritdoc />
    public IRequestBuilder WithMultiPart(IMatcher matcher)
    {
        _requestMatchers.Add(new RequestMessageMultiPartMatcher(matcher));
        return this;
    }

    /// <inheritdoc />
    public IRequestBuilder WithMultiPart(IMatcher[] matchers, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch, MatchOperator matchOperator = MatchOperator.And)
    {
        _requestMatchers.Add(new RequestMessageMultiPartMatcher(matchBehaviour, matchOperator, matchers));
        return this;
    }

    /// <inheritdoc />
    public IRequestBuilder WithMultiPart(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch, params IMatcher[] matchers)
    {
        _requestMatchers.Add(new RequestMessageMultiPartMatcher(matchBehaviour, MatchOperator.Or, matchers));
        return this;
    }
}