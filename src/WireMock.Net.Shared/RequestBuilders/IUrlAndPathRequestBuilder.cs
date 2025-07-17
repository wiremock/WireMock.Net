// Copyright © WireMock.Net

using System;
using WireMock.Matchers;

namespace WireMock.RequestBuilders;

/// <summary>
/// IUrlAndPathRequestBuilder
/// </summary>
public interface IUrlAndPathRequestBuilder : IMethodRequestBuilder
{
    /// <summary>
    /// WithPath: add path matching based on IStringMatchers.
    /// </summary>
    /// <param name="matchers">The matchers.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithPath(params IStringMatcher[] matchers);

    /// <summary>
    /// WithPath: add path matching based on MatchOperator and IStringMatchers.
    /// </summary>
    /// <param name="matchOperator">The <see cref="MatchOperator"/> to use.</param>
    /// <param name="matchers">The matchers.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithPath(MatchOperator matchOperator, params IStringMatcher[] matchers);

    /// <summary>
    /// WithPath: add path matching based on paths.
    /// </summary>
    /// <param name="paths">The paths.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithPath(params string[] paths);

    /// <summary>
    /// WithPath: add path matching based on paths , matchBehaviour and MatchOperator.
    /// </summary>
    /// <param name="matchOperator">The <see cref="MatchOperator"/> to use.</param>
    /// <param name="paths">The paths.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithPath(MatchOperator matchOperator, params string[] paths);

    /// <summary>
    /// WithPath: add path matching based on functions.
    /// </summary>
    /// <param name="funcs">The path funcs.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithPath(params Func<string, bool>[] funcs);

    /// <summary>
    /// WithUrl: add url matching based on IStringMatcher[].
    /// </summary>
    /// <param name="matchers">The matchers.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithUrl(params IStringMatcher[] matchers);

    /// <summary>
    /// WithUrl: add url matching based on MatchOperator and IStringMatchers.
    /// </summary>
    /// <param name="matchOperator">The <see cref="MatchOperator"/> to use.</param>
    /// <param name="matchers">The matchers.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithUrl(MatchOperator matchOperator, params IStringMatcher[] matchers);

    /// <summary>
    /// WithUrl: add url matching based on urls.
    /// </summary>
    /// <param name="urls">The urls.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithUrl(params string[] urls);

    /// <summary>
    /// WithUrl: add url matching based on urls.
    /// </summary>
    /// <param name="matchOperator">The <see cref="MatchOperator"/> to use.</param>
    /// <param name="urls">The urls.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithUrl(MatchOperator matchOperator, params string[] urls);

    /// <summary>
    /// WithUrl: add url matching based on functions.
    /// </summary>
    /// <param name="funcs">The url functions.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithUrl(params Func<string, bool>[] funcs);
}