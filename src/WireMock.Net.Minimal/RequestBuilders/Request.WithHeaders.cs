// Copyright © WireMock.Net and mock4net by Alexandre Victoor

// This source file is based on mock4net by Alexandre Victoor which is licensed under the Apache 2.0 License.
// For more details see 'mock4net/LICENSE.txt' and 'mock4net/readme.md' in this project root.
using WireMock.Matchers;
using WireMock.Matchers.Request;
using Stef.Validation;

namespace WireMock.RequestBuilders;

public partial class Request
{
    /// <inheritdoc cref="IHeadersRequestBuilder.WithHeader(string, string, MatchBehaviour)"/>
    public IRequestBuilder WithHeader(string name, string pattern, MatchBehaviour matchBehaviour)
    {
        return WithHeader(name, pattern, true, matchBehaviour);
    }

    /// <inheritdoc cref="IHeadersRequestBuilder.WithHeader(string, string, bool, MatchBehaviour)"/>
    public IRequestBuilder WithHeader(string name, string pattern, bool ignoreCase = true, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        Guard.NotNull(name);
        Guard.NotNull(pattern);

        _requestMatchers.Add(new RequestMessageHeaderMatcher(matchBehaviour, name, pattern, ignoreCase));
        return this;
    }

    /// <inheritdoc />
    public IRequestBuilder WithHeader(string name, string[] patterns, MatchBehaviour matchBehaviour, MatchOperator matchOperator = MatchOperator.Or)
    {
        return WithHeader(name, patterns, true, matchBehaviour, matchOperator);
    }

    /// <inheritdoc />
    public IRequestBuilder WithHeader(string name, string[] patterns, bool ignoreCase = true, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch, MatchOperator matchOperator = MatchOperator.Or)
    {
        Guard.NotNull(name);
        Guard.NotNull(patterns);

        _requestMatchers.Add(new RequestMessageHeaderMatcher(matchBehaviour, matchOperator, name, ignoreCase, patterns));
        return this;
    }

    /// <inheritdoc />
    public IRequestBuilder WithHeader(string name, params IStringMatcher[] matchers)
    {
        Guard.NotNull(name);
        Guard.NotNullOrEmpty(matchers);

        _requestMatchers.Add(new RequestMessageHeaderMatcher(MatchBehaviour.AcceptOnMatch, MatchOperator.Or, name, false, matchers));
        return this;
    }

    /// <inheritdoc />
    public IRequestBuilder WithHeader(string name, bool ignoreCase, params IStringMatcher[] matchers)
    {
        Guard.NotNull(name);
        Guard.NotNullOrEmpty(matchers);

        _requestMatchers.Add(new RequestMessageHeaderMatcher(MatchBehaviour.AcceptOnMatch, MatchOperator.Or, name, ignoreCase, matchers));
        return this;
    }

    /// <inheritdoc />
    public IRequestBuilder WithHeader(string name, bool ignoreCase, MatchBehaviour matchBehaviour, MatchOperator matchOperator, params IStringMatcher[] matchers)
    {
        Guard.NotNull(name);
        Guard.NotNullOrEmpty(matchers);

        _requestMatchers.Add(new RequestMessageHeaderMatcher(matchBehaviour, matchOperator, name, ignoreCase, matchers));
        return this;
    }

    /// <inheritdoc />
    public IRequestBuilder WithHeader(params Func<IDictionary<string, string[]>, bool>[] funcs)
    {
        Guard.NotNullOrEmpty(funcs);

        _requestMatchers.Add(new RequestMessageHeaderMatcher(funcs));
        return this;
    }
}