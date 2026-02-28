// Copyright © WireMock.Net and mock4net by Alexandre Victoor

// This source file is based on mock4net by Alexandre Victoor which is licensed under the Apache 2.0 License.
// For more details see 'mock4net/LICENSE.txt' and 'mock4net/readme.md' in this project root.
using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.Types;
using Stef.Validation;

namespace WireMock.RequestBuilders;

public partial class Request
{
    /// <inheritdoc cref="IParamsRequestBuilder.WithParam(string, MatchBehaviour)"/>
    public IRequestBuilder WithParam(string key, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        return WithParam(key, false, matchBehaviour);
    }

    /// <inheritdoc cref="IParamsRequestBuilder.WithParam(string, bool, MatchBehaviour)"/>
    public IRequestBuilder WithParam(string key, bool ignoreCase, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        Guard.NotNull(key);

        _requestMatchers.Add(new RequestMessageParamMatcher(matchBehaviour, key, ignoreCase));
        return this;
    }

    /// <inheritdoc cref="IParamsRequestBuilder.WithParam(string, string[])"/>
    public IRequestBuilder WithParam(string key, params string[] values)
    {
        return WithParam(key, MatchBehaviour.AcceptOnMatch, false, values);
    }

    /// <inheritdoc cref="IParamsRequestBuilder.WithParam(string, bool, string[])"/>
    public IRequestBuilder WithParam(string key, bool ignoreCase, params string[] values)
    {
        return WithParam(key, MatchBehaviour.AcceptOnMatch, ignoreCase, values);
    }

    /// <inheritdoc cref="IParamsRequestBuilder.WithParam(string, IStringMatcher[])"/>
    public IRequestBuilder WithParam(string key, params IStringMatcher[] matchers)
    {
        return WithParam(key, MatchBehaviour.AcceptOnMatch, false, matchers);
    }

    /// <inheritdoc cref="IParamsRequestBuilder.WithParam(string, bool, IStringMatcher[])"/>
    public IRequestBuilder WithParam(string key, bool ignoreCase, params IStringMatcher[] matchers)
    {
        return WithParam(key, MatchBehaviour.AcceptOnMatch, ignoreCase, matchers);
    }

    /// <inheritdoc cref="IParamsRequestBuilder.WithParam(string, MatchBehaviour, string[])"/>
    public IRequestBuilder WithParam(string key, MatchBehaviour matchBehaviour, params string[] values)
    {
        return WithParam(key, matchBehaviour, false, values);
    }

    /// <inheritdoc cref="IParamsRequestBuilder.WithParam(string, MatchBehaviour, bool, string[])"/>
    public IRequestBuilder WithParam(string key, MatchBehaviour matchBehaviour, bool ignoreCase = false, params string[] values)
    {
        Guard.NotNull(key);

        _requestMatchers.Add(new RequestMessageParamMatcher(matchBehaviour, key, ignoreCase, values));
        return this;
    }

    /// <inheritdoc cref="IParamsRequestBuilder.WithParam(string, MatchBehaviour, IStringMatcher[])"/>
    public IRequestBuilder WithParam(string key, MatchBehaviour matchBehaviour, params IStringMatcher[] matchers)
    {
        return WithParam(key, matchBehaviour, false, matchers);
    }

    /// <inheritdoc cref="IParamsRequestBuilder.WithParam(string, MatchBehaviour, bool, IStringMatcher[])"/>
    public IRequestBuilder WithParam(string key, MatchBehaviour matchBehaviour, bool ignoreCase, params IStringMatcher[] matchers)
    {
        Guard.NotNull(key);

        _requestMatchers.Add(new RequestMessageParamMatcher(matchBehaviour, key, ignoreCase, matchers));
        return this;
    }

    /// <inheritdoc cref="IParamsRequestBuilder.WithParam(Func{IDictionary{string, WireMockList{string}}, bool}[])"/>
    public IRequestBuilder WithParam(params Func<IDictionary<string, WireMockList<string>>, bool>[] funcs)
    {
        Guard.NotNullOrEmpty(funcs);

        _requestMatchers.Add(new RequestMessageParamMatcher(funcs));
        return this;
    }
}