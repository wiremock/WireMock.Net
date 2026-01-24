// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.Linq;
using AnyOfTypes;
using Stef.Validation;
using WireMock.Models;

namespace WireMock.Matchers.Request;

/// <summary>
/// The request url matcher.
/// </summary>
public class RequestMessageUrlMatcher : IRequestMatcher
{
    /// <summary>
    /// The matchers
    /// </summary>
    public IReadOnlyList<IStringMatcher>? Matchers { get; }

    /// <summary>
    /// The url functions
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
    /// Initializes a new instance of the <see cref="RequestMessageUrlMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="matchOperator">The <see cref="MatchOperator"/> to use.</param>
    /// <param name="urls">The urls.</param>
    public RequestMessageUrlMatcher(
        MatchBehaviour matchBehaviour,
        MatchOperator matchOperator,
        params string[] urls) :
        this(matchBehaviour, matchOperator, urls
            .Select(url => new WildcardMatcher(matchBehaviour, new AnyOf<string, StringPattern>[] { url }, false, matchOperator))
            .Cast<IStringMatcher>().ToArray())
    {
        Behaviour = matchBehaviour;
        MatchOperator = matchOperator;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageUrlMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="matchOperator">The <see cref="MatchOperator"/> to use. (default = "Or")</param>
    /// <param name="matchers">The matchers.</param>
    public RequestMessageUrlMatcher(MatchBehaviour matchBehaviour, MatchOperator matchOperator, params IStringMatcher[] matchers)
    {
        Matchers = Guard.NotNull(matchers);
        Behaviour = matchBehaviour;
        MatchOperator = matchOperator;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageUrlMatcher"/> class.
    /// </summary>
    /// <param name="funcs">The url functions.</param>
    public RequestMessageUrlMatcher(params Func<string, bool>[] funcs)
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
            var results = Matchers.Select(m => m.IsMatch(requestMessage.Url)).ToArray();
            return MatchResult.From(nameof(RequestMessageUrlMatcher), results, MatchOperator);
        }

        if (Funcs != null)
        {
            var results = Funcs.Select(func => func(requestMessage.Url)).ToArray();
            var score = MatchScores.ToScore(results, MatchOperator);
            return MatchResult.From(nameof(RequestMessageUrlMatcher), score);
        }

        return MatchResult.From(nameof(RequestMessageUrlMatcher));
    }
}