// Copyright © WireMock.Net

using System;
using System.Linq;
using Stef.Validation;

namespace WireMock.Matchers.Request;

/// <summary>
/// The request HTTP Version matcher.
/// </summary>
public class RequestMessageHttpVersionMatcher : IRequestMatcher
{
    private const string  _name = nameof(RequestMessageHttpVersionMatcher);

    /// <summary>
    /// The matcher.
    /// </summary>
    public IStringMatcher? Matcher { get; }

    /// <summary>
    /// The func.
    /// </summary>
    public Func<string, bool>? MatcherOnStringFunc { get; }

    /// <summary>
    /// The <see cref="MatchBehaviour"/>
    /// </summary>
    public MatchBehaviour Behaviour { get; }

    /// <summary>
    /// The HTTP Version
    /// </summary>
    public string? HttpVersion { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageHttpVersionMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="httpVersion">The HTTP Version.</param>
    public RequestMessageHttpVersionMatcher(MatchBehaviour matchBehaviour, string httpVersion) :
        this(matchBehaviour, new ExactMatcher(matchBehaviour, httpVersion))
    {
        HttpVersion = httpVersion;
        Behaviour = matchBehaviour;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageClientIPMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="matcher">The matcher.</param>
    public RequestMessageHttpVersionMatcher(MatchBehaviour matchBehaviour, IStringMatcher matcher)
    {
        Matcher = Guard.NotNull(matcher);
        Behaviour = matchBehaviour;
        HttpVersion = matcher.GetPatterns().FirstOrDefault();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageClientIPMatcher"/> class.
    /// </summary>
    /// <param name="func">The function.</param>
    public RequestMessageHttpVersionMatcher(Func<string, bool> func)
    {
        MatcherOnStringFunc = Guard.NotNull(func);
    }

    /// <inheritdoc />
    public double GetMatchingScore(IRequestMessage requestMessage, IRequestMatchResult requestMatchResult)
    {
        var (score, exception) = GetMatchResult(requestMessage).Expand();
        return requestMatchResult.AddScore(GetType(), score, exception);
    }

    private MatchResult GetMatchResult(IRequestMessage requestMessage)
    {
        if (Matcher != null)
        {
            return Matcher.IsMatch(requestMessage.HttpVersion);
        }

        if (MatcherOnStringFunc != null)
        {
            return MatchResult.From($"{_name}:{nameof(MatcherOnStringFunc)}", MatchScores.ToScore(MatcherOnStringFunc(requestMessage.HttpVersion)));
        }

        return MatchResult.From(_name);
    }
}