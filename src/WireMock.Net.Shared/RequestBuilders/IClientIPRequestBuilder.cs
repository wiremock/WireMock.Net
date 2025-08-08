// Copyright © WireMock.Net

using System;
using WireMock.Matchers;

namespace WireMock.RequestBuilders;

/// <summary>
/// The IClientIPRequestBuilder interface.
/// </summary>
public interface IClientIPRequestBuilder : IUrlAndPathRequestBuilder
{
    /// <summary>
    /// WithClientIP: add clientIP matching based on IStringMatchers.
    /// </summary>
    /// <param name="matchers">The matchers.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithClientIP(params IStringMatcher[] matchers);

    /// <summary>
    /// WithClientIP: add clientIP matching based on MatchOperator and IStringMatchers.
    /// </summary>
    /// <param name="matchOperator">The <see cref="MatchOperator"/> to use.</param>
    /// <param name="matchers">The matchers.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithClientIP(MatchOperator matchOperator, params IStringMatcher[] matchers);

    /// <summary>
    /// WithClientIP: add clientIP matching based on clientIPs.
    /// </summary>
    /// <param name="clientIPs">The clientIPs.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithClientIP(params string[] clientIPs);

    /// <summary>
    /// WithClientIP: add clientIP matching based on clientIPs , matchBehaviour and MatchOperator.
    /// </summary>
    /// <param name="matchOperator">The <see cref="MatchOperator"/> to use.</param>
    /// <param name="clientIPs">The clientIPs.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithClientIP(MatchOperator matchOperator, params string[] clientIPs);

    /// <summary>
    /// WithClientIP: add clientIP matching based on functions.
    /// </summary>
    /// <param name="funcs">The clientIP funcs.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithClientIP(params Func<string, bool>[] funcs);
}