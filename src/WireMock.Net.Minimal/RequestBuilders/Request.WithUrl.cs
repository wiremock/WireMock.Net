// Copyright © WireMock.Net

using Stef.Validation;
using WireMock.Matchers;
using WireMock.Matchers.Request;

namespace WireMock.RequestBuilders;

public partial class Request
{
    /// <inheritdoc />
    public IRequestBuilder WithUrl(params IStringMatcher[] matchers)
    {
        return WithUrl(MatchOperator.Or, matchers);
    }

    /// <inheritdoc />
    public IRequestBuilder WithUrl(MatchOperator matchOperator, params IStringMatcher[] matchers)
    {
        Guard.NotNullOrEmpty(matchers);

        _requestMatchers.Add(new RequestMessageUrlMatcher(MatchBehaviour.AcceptOnMatch, matchOperator, matchers));
        return this;
    }

    /// <inheritdoc />
    public IRequestBuilder WithUrl(params string[] urls)
    {
        return WithUrl(MatchOperator.Or, urls);
    }

    /// <inheritdoc />
    public IRequestBuilder WithUrl(MatchOperator matchOperator, params string[] urls)
    {
        Guard.NotNullOrEmpty(urls);

        _requestMatchers.Add(new RequestMessageUrlMatcher(MatchBehaviour.AcceptOnMatch, matchOperator, urls));
        return this;
    }

    /// <inheritdoc cref="IUrlAndPathRequestBuilder.WithUrl(Func{string, bool}[])"/>
    public IRequestBuilder WithUrl(params Func<string, bool>[] funcs)
    {
        Guard.NotNullOrEmpty(funcs);

        _requestMatchers.Add(new RequestMessageUrlMatcher(funcs));
        return this;
    }
}