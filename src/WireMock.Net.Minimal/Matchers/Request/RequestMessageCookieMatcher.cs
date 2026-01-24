// Copyright © WireMock.Net

using Stef.Validation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WireMock.Matchers.Request;

/// <summary>
/// The request cookie matcher.
/// </summary>
/// <inheritdoc cref="IRequestMatcher"/>
public class RequestMessageCookieMatcher : IRequestMatcher
{
    private const string _name = nameof(RequestMessageCookieMatcher);

    /// <summary>
    /// MatchBehaviour
    /// </summary>
    public MatchBehaviour MatchBehaviour { get; }

    /// <summary>
    /// IgnoreCase
    /// </summary>
    public bool IgnoreCase { get; }

    /// <summary>
    /// The functions
    /// </summary>
    public Func<IDictionary<string, string>, bool>[]? Funcs { get; }

    /// <summary>
    /// The name
    /// </summary>
    public string Name { get; }

    /// <value>
    /// The matchers.
    /// </value>
    public IStringMatcher[]? Matchers { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageCookieMatcher"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="pattern">The pattern.</param>
    /// <param name="ignoreCase">Ignore the case from the pattern.</param>
    /// <param name="matchBehaviour">The match behaviour.</param>
    public RequestMessageCookieMatcher(MatchBehaviour matchBehaviour, string name, string pattern, bool ignoreCase)
    {
        MatchBehaviour = matchBehaviour;
        IgnoreCase = ignoreCase;
        Name = Guard.NotNull(name);
        Matchers = new IStringMatcher[] { new WildcardMatcher(matchBehaviour, Guard.NotNull(pattern), ignoreCase) };
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageCookieMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="name">The name.</param>
    /// <param name="patterns">The patterns.</param>
    /// <param name="ignoreCase">Ignore the case from the pattern.</param>
    public RequestMessageCookieMatcher(MatchBehaviour matchBehaviour, string name, bool ignoreCase, params string[] patterns) :
        this(matchBehaviour, name, ignoreCase, patterns.Select(pattern => new WildcardMatcher(matchBehaviour, pattern, ignoreCase)).Cast<IStringMatcher>().ToArray())
    {
        Guard.NotNull(patterns);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageCookieMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="name">The name.</param>
    /// <param name="matchers">The matchers.</param>
    /// <param name="ignoreCase">Ignore the case from the pattern.</param>
    public RequestMessageCookieMatcher(MatchBehaviour matchBehaviour, string name, bool ignoreCase, params IStringMatcher[] matchers)
    {
        MatchBehaviour = matchBehaviour;
        Name = Guard.NotNull(name);
        Matchers = Guard.NotNull(matchers);
        IgnoreCase = ignoreCase;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageCookieMatcher"/> class.
    /// </summary>
    /// <param name="funcs">The funcs.</param>
    public RequestMessageCookieMatcher(params Func<IDictionary<string, string>, bool>[] funcs)
    {
        Guard.NotNull(funcs);

        Funcs = funcs;
        Name = string.Empty; // Not used when Func, but set to a non-null valid value.
    }

    /// <inheritdoc />
    public double GetMatchingScore(IRequestMessage requestMessage, IRequestMatchResult requestMatchResult)
    {
        var (score, exception) = GetMatchResult(requestMessage).Expand();
        return requestMatchResult.AddScore(GetType(), score, exception);
    }

    private MatchResult GetMatchResult(IRequestMessage requestMessage)
    {
        if (requestMessage.Cookies == null)
        {
            return MatchResult.From(_name, MatchBehaviourHelper.Convert(MatchBehaviour, MatchScores.Mismatch));
        }

        // Check if we want to use IgnoreCase to compare the Cookie-Name and Cookie-Value
        var cookies = !IgnoreCase ? requestMessage.Cookies : new Dictionary<string, string>(requestMessage.Cookies, StringComparer.OrdinalIgnoreCase);

        if (Funcs != null)
        {
            return MatchResult.From(_name, MatchScores.ToScore(Funcs.Any(f => f(cookies))));
        }

        if (Matchers == null)
        {
            return MatchResult.From(_name);
        }

        if (!cookies.ContainsKey(Name))
        {
            return MatchResult.From(_name, MatchBehaviourHelper.Convert(MatchBehaviour, MatchScores.Mismatch));
        }

        return MatchResult.From(_name, Matchers.Max(m => m.IsMatch(cookies[Name]))?.Score ?? MatchScores.Mismatch);
    }
}