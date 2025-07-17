// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using WireMock.Matchers;
using WireMock.Types;

namespace WireMock.RequestBuilders;

/// <summary>
/// The ParamsRequestBuilder interface.
/// </summary>
public interface IParamsRequestBuilder : IBodyRequestBuilder
{
    /// <summary>
    /// WithParam: matching on key only.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="matchBehaviour">The match behaviour (optional).</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithParam(string key, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch);

    /// <summary>
    /// WithParam: matching on key only.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="ignoreCase">Defines if the key should be matched using case-ignore.</param>
    /// <param name="matchBehaviour">The match behaviour (optional).</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithParam(string key, bool ignoreCase, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch);

    /// <summary>
    /// WithParam: matching on key and values.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="values">The values.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithParam(string key, params string[] values);

    /// <summary>
    /// WithParam: matching on key and values.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="ignoreCase">Defines if the key should be matched using case-ignore.</param>
    /// <param name="values">The values.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithParam(string key, bool ignoreCase, params string[] values);

    /// <summary>
    /// WithParam: matching on key and matchers.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="matchers">The matchers.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithParam(string key, params IStringMatcher[] matchers);

    /// <summary>
    /// WithParam: matching on key and matchers.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="ignoreCase">Defines if the key should be matched using case-ignore.</param>
    /// <param name="matchers">The matchers.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithParam(string key, bool ignoreCase, params IStringMatcher[] matchers);

    /// <summary>
    /// WithParam: matching on key, values and matchBehaviour.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="values">The values.</param>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithParam(string key, MatchBehaviour matchBehaviour, params string[] values);

    /// <summary>
    /// WithParam: matching on key, values and matchBehaviour.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="ignoreCase">Defines if the key should be matched using case-ignore.</param>
    /// <param name="values">The values.</param>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithParam(string key, MatchBehaviour matchBehaviour, bool ignoreCase = false, params string[] values);

    /// <summary>
    /// WithParam: matching on key, matchers and matchBehaviour.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="matchers">The matchers.</param>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithParam(string key, MatchBehaviour matchBehaviour, params IStringMatcher[] matchers);

    /// <summary>
    /// WithParam: matching on key, matchers and matchBehaviour.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="ignoreCase">Defines if the key should be matched using case-ignore.</param>
    /// <param name="matchers">The matchers.</param>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithParam(string key, MatchBehaviour matchBehaviour, bool ignoreCase = false, params IStringMatcher[] matchers);

    /// <summary>
    /// WithParam: matching on functions.
    /// </summary>
    /// <param name="funcs">The funcs.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithParam(params Func<IDictionary<string, WireMockList<string>>, bool>[] funcs);
}