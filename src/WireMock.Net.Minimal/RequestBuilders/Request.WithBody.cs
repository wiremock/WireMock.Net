// Copyright © WireMock.Net and mock4net by Alexandre Victoor

// This source file is based on mock4net by Alexandre Victoor which is licensed under the Apache 2.0 License.
// For more details see 'mock4net/LICENSE.txt' and 'mock4net/readme.md' in this project root.
using System;
using System.Collections.Generic;
using Stef.Validation;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.Util;

namespace WireMock.RequestBuilders;

public partial class Request
{
    /// <inheritdoc />
    public IRequestBuilder WithBody(string body, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        _requestMatchers.Add(new RequestMessageBodyMatcher(matchBehaviour, body));
        return this;
    }

    /// <inheritdoc />
    public IRequestBuilder WithBody(byte[] body, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        _requestMatchers.Add(new RequestMessageBodyMatcher(matchBehaviour, body));
        return this;
    }

    /// <inheritdoc />
    public IRequestBuilder WithBody(object body, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        _requestMatchers.Add(new RequestMessageBodyMatcher(matchBehaviour, body));
        return this;
    }

    /// <inheritdoc />
    public IRequestBuilder WithBodyAsJson(object body, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        var matcher = body as IMatcher ?? new JsonMatcher(matchBehaviour, body);
        return WithBody([matcher]);
    }

    /// <inheritdoc />
    public IRequestBuilder WithBody(IMatcher matcher)
    {
        return WithBody([matcher]);
    }

    /// <inheritdoc />
    public IRequestBuilder WithBody(IMatcher[] matchers, MatchOperator matchOperator = MatchOperator.Or)
    {
        Guard.NotNull(matchers);

        _requestMatchers.Add(new RequestMessageBodyMatcher(matchOperator, matchers));
        return this;
    }

    /// <inheritdoc />
    public IRequestBuilder WithBody(Func<string?, bool> func)
    {
        Guard.NotNull(func);

        _requestMatchers.Add(new RequestMessageBodyMatcher(func));
        return this;
    }

    /// <inheritdoc />
    public IRequestBuilder WithBody(Func<byte[]?, bool> func)
    {
        Guard.NotNull(func);

        _requestMatchers.Add(new RequestMessageBodyMatcher(func));
        return this;
    }

    /// <inheritdoc />
    public IRequestBuilder WithBody(Func<object?, bool> func)
    {
        Guard.NotNull(func);

        _requestMatchers.Add(new RequestMessageBodyMatcher(func));
        return this;
    }

    /// <inheritdoc />
    public IRequestBuilder WithBodyType<T>(Func<T?, bool> func)
    {
        Guard.NotNull(func);

        _requestMatchers.Add(new RequestMessageBodyMatcher<T>(func));
        return this;
    }

    /// <inheritdoc />
    public IRequestBuilder WithBody(Func<IBodyData?, bool> func)
    {
        Guard.NotNull(func);

        _requestMatchers.Add(new RequestMessageBodyMatcher(func));
        return this;
    }

    /// <inheritdoc />
    public IRequestBuilder WithBody(Func<IDictionary<string, string>?, bool> func)
    {
        _requestMatchers.Add(new RequestMessageBodyMatcher(Guard.NotNull(func)));
        return this;
    }
}