// Copyright © WireMock.Net

using Stef.Validation;
using WireMock.Matchers;
using WireMock.Matchers.Request;

namespace WireMock.RequestBuilders;

public partial class Request
{
    /// <inheritdoc />
    public IRequestBuilder WithClientIP(params IStringMatcher[] matchers)
    {
        return WithClientIP(MatchOperator.Or, matchers);
    }

    /// <inheritdoc />
    public IRequestBuilder WithClientIP(MatchOperator matchOperator, params IStringMatcher[] matchers)
    {
        Guard.NotNullOrEmpty(matchers);

        _requestMatchers.Add(new RequestMessageClientIPMatcher(MatchBehaviour.AcceptOnMatch, MatchOperator.Or, matchers));
        return this;
    }

    /// <inheritdoc />
    public IRequestBuilder WithClientIP(params string[] clientIPs)
    {
        return WithClientIP(MatchOperator.Or, clientIPs);
    }

    /// <inheritdoc />
    public IRequestBuilder WithClientIP(MatchOperator matchOperator, params string[] clientIPs)
    {
        Guard.NotNullOrEmpty(clientIPs);

        _requestMatchers.Add(new RequestMessageClientIPMatcher(MatchBehaviour.AcceptOnMatch, matchOperator, clientIPs));
        return this;
    }

    /// <inheritdoc />
    public IRequestBuilder WithClientIP(params Func<string, bool>[] funcs)
    {
        Guard.NotNullOrEmpty(funcs);

        _requestMatchers.Add(new RequestMessageClientIPMatcher(funcs));
        return this;
    }
}