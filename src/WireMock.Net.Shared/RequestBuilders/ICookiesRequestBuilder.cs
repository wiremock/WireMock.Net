// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using WireMock.Matchers;

namespace WireMock.RequestBuilders;

/// <summary>
/// The CookieRequestBuilder interface.
/// </summary>
public interface ICookiesRequestBuilder : IParamsRequestBuilder
{
    /// <summary>
    /// WithCookie: matching based on name, pattern and matchBehaviour.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="pattern">The pattern.</param>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithCookie(string name, string pattern, MatchBehaviour matchBehaviour);

    /// <summary>
    /// WithCookie: matching based on name, pattern, ignoreCase and matchBehaviour.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="pattern">The pattern.</param>
    /// <param name="ignoreCase">Ignore the case from the pattern.</param>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithCookie(string name, string pattern, bool ignoreCase = true, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch);

    /// <summary>
    /// WithCookie: matching based on name, patterns and matchBehaviour.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="patterns">The patterns.</param>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithCookie(string name, string[] patterns, MatchBehaviour matchBehaviour);

    /// <summary>
    /// WithCookie: matching based on name, patterns, ignoreCase and matchBehaviour.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="patterns">The patterns.</param>
    /// <param name="ignoreCase">Ignore the case from the pattern.</param>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithCookie(string name, string[] patterns, bool ignoreCase = true, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch);

    /// <summary>
    /// WithCookie: matching based on name and IStringMatcher[].
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="matchers">The matchers.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithCookie(string name, params IStringMatcher[] matchers);

    /// <summary>
    /// WithCookie: matching based on name, ignoreCase and IStringMatcher[].
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="ignoreCase">Ignore the case from the cookie-keys.</param>
    /// <param name="matchers">The matchers.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithCookie(string name, bool ignoreCase, params IStringMatcher[] matchers);

    /// <summary>
    /// WithCookie: matching based on name, ignoreCase, matchBehaviour and IStringMatcher[].
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="ignoreCase">Ignore the case from the cookie-keys.</param>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="matchers">The matchers.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithCookie(string name, bool ignoreCase, MatchBehaviour matchBehaviour, params IStringMatcher[] matchers);

    /// <summary>
    /// WithCookie: matching based on functions.
    /// </summary>
    /// <param name="funcs">The cookies funcs.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithCookie(params Func<IDictionary<string, string>, bool>[] funcs);
}