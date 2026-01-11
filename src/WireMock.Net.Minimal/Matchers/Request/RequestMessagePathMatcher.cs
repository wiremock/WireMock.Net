// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.Linq;
using AnyOfTypes;
using Stef.Validation;
using WireMock.Models;

namespace WireMock.Matchers.Request;

/// <summary>
/// The request path matcher.
/// </summary>
public class RequestMessagePathMatcher : IRequestMatcher
{
    /// <summary>
    /// The matchers
    /// </summary>
    public IReadOnlyList<IStringMatcher>? Matchers { get; }

    /// <summary>
    /// The path functions
    /// </summary>
    public Func<string, bool>[]? Funcs { get; }

    /// <summary>
    /// The <see cref="MatchBehaviour"/>
    /// </summary>
    public MatchBehaviour Behaviour { get; }

    /// <summary>
    /// The <see cref="MatchOperator"/>
    /// </summary>
    public MatchOperator MatchOperator { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessagePathMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="matchOperator">The <see cref="MatchOperator"/> to use.</param>
    /// <param name="paths">The paths.</param>
    public RequestMessagePathMatcher(
        MatchBehaviour matchBehaviour,
        MatchOperator matchOperator,
        params string[] paths) :
        this(matchBehaviour, matchOperator, paths
            .Select(path => new WildcardMatcher(matchBehaviour, new AnyOf<string, StringPattern>[] { path }, false, matchOperator))
            .Cast<IStringMatcher>().ToArray())
    {
        Behaviour = matchBehaviour;
        MatchOperator = matchOperator;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessagePathMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="matchOperator">The <see cref="MatchOperator"/> to use.</param>
    /// <param name="matchers">The matchers.</param>
    public RequestMessagePathMatcher(MatchBehaviour matchBehaviour, MatchOperator matchOperator, params IStringMatcher[] matchers)
    {
        Matchers = Guard.NotNull(matchers);
        Behaviour = matchBehaviour;
        MatchOperator = matchOperator;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessagePathMatcher"/> class.
    /// </summary>
    /// <param name="funcs">The path functions.</param>
    public RequestMessagePathMatcher(params Func<string, bool>[] funcs)
    {
        Funcs = Guard.NotNull(funcs);
    }

    /// <inheritdoc />
    public double GetMatchingScore(IRequestMessage requestMessage, IRequestMatchResult requestMatchResult)
    {
        var (score, exception) = GetMatchResult(requestMessage).Expand();
        return requestMatchResult.AddScore(GetType(), score, exception);
    }

    private MatchResult GetMatchResult(IRequestMessage requestMessage)
    {
        if (Matchers != null)
        {
            var results = Matchers.Select(m => m.IsMatch(requestMessage.Path)).ToArray();
            return MatchResult.From(nameof(RequestMessagePathMatcher), results, MatchOperator);
        }

        if (Funcs != null)
        {
            var results = Funcs.Select(func => func(requestMessage.Path)).ToArray();
            var score = MatchScores.ToScore(results, MatchOperator);
            return MatchResult.From(nameof(RequestMessagePathMatcher), score);
        }

        return MatchResult.From(nameof(RequestMessagePathMatcher));
    }
}