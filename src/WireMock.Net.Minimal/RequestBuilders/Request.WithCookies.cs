// Copyright © WireMock.Net

using WireMock.Matchers;
using WireMock.Matchers.Request;
using Stef.Validation;

namespace WireMock.RequestBuilders;

public partial class Request
{
    /// <inheritdoc cref="ICookiesRequestBuilder.WithCookie(string, string, MatchBehaviour)"/>
    public IRequestBuilder WithCookie(string name, string pattern, MatchBehaviour matchBehaviour)
    {
        return WithCookie(name, pattern, true, matchBehaviour);
    }

    /// <inheritdoc cref="ICookiesRequestBuilder.WithCookie(string, string, bool, MatchBehaviour)"/>
    public IRequestBuilder WithCookie(string name, string pattern, bool ignoreCase = true, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        Guard.NotNull(name);
        Guard.NotNull(pattern);

        _requestMatchers.Add(new RequestMessageCookieMatcher(matchBehaviour, name, pattern, ignoreCase));
        return this;
    }

    /// <inheritdoc cref="ICookiesRequestBuilder.WithCookie(string, string[], MatchBehaviour)"/>
    public IRequestBuilder WithCookie(string name, string[] patterns, MatchBehaviour matchBehaviour)
    {
        return WithCookie(name, patterns, true, matchBehaviour);
    }

    /// <inheritdoc cref="ICookiesRequestBuilder.WithCookie(string, string[], bool, MatchBehaviour)"/>
    public IRequestBuilder WithCookie(string name, string[] patterns, bool ignoreCase = true, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        Guard.NotNull(name);
        Guard.NotNull(patterns);

        _requestMatchers.Add(new RequestMessageCookieMatcher(matchBehaviour, name, ignoreCase, patterns));
        return this;
    }

    /// <inheritdoc cref="ICookiesRequestBuilder.WithCookie(string, IStringMatcher[])"/>
    public IRequestBuilder WithCookie(string name, params IStringMatcher[] matchers)
    {
        Guard.NotNull(name);
        Guard.NotNullOrEmpty(matchers);

        _requestMatchers.Add(new RequestMessageCookieMatcher(MatchBehaviour.AcceptOnMatch, name, false, matchers));
        return this;
    }

    /// <inheritdoc cref="ICookiesRequestBuilder.WithCookie(string, bool, IStringMatcher[])"/>
    public IRequestBuilder WithCookie(string name, bool ignoreCase, params IStringMatcher[] matchers)
    {
        Guard.NotNull(name);
        Guard.NotNullOrEmpty(matchers);

        _requestMatchers.Add(new RequestMessageCookieMatcher(MatchBehaviour.AcceptOnMatch, name, ignoreCase, matchers));
        return this;
    }

    /// <inheritdoc cref="ICookiesRequestBuilder.WithCookie(string, IStringMatcher[])"/>
    public IRequestBuilder WithCookie(string name, bool ignoreCase, MatchBehaviour matchBehaviour, params IStringMatcher[] matchers)
    {
        Guard.NotNull(name);
        Guard.NotNullOrEmpty(matchers);

        _requestMatchers.Add(new RequestMessageCookieMatcher(matchBehaviour, name, ignoreCase, matchers));
        return this;
    }

    /// <inheritdoc cref="ICookiesRequestBuilder.WithCookie(Func{IDictionary{string, string}, bool}[])"/>
    public IRequestBuilder WithCookie(params Func<IDictionary<string, string>, bool>[] funcs)
    {
        Guard.NotNullOrEmpty(funcs);

        _requestMatchers.Add(new RequestMessageCookieMatcher(funcs));
        return this;
    }
}