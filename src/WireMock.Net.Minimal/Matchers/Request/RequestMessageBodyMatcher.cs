// Copyright © WireMock.Net

using System.Linq;
using Stef.Validation;
using WireMock.Matchers.Helpers;
using WireMock.Util;

namespace WireMock.Matchers.Request;

/// <summary>
/// The request body matcher.
/// </summary>
public class RequestMessageBodyMatcher : IRequestMatcher
{
    /// <summary>
    /// The body function
    /// </summary>
    public Func<string?, bool>? MatchOnBodyAsStringFunc { get; }

    /// <summary>
    /// The body data function for byte[]
    /// </summary>
    public Func<byte[]?, bool>? MatchOnBodyAsBytesFunc { get; }

    /// <summary>
    /// The body data function for json
    /// </summary>
    public Func<object?, bool>? MatchOnBodyAsJsonFunc { get; }

    /// <summary>
    /// The body data function for BodyData
    /// </summary>
    public Func<IBodyData?, bool>? MatchOnBodyAsBodyDataFunc { get; }

    /// <summary>
    /// The body data function for FormUrlEncoded
    /// </summary>
    public Func<IDictionary<string, string>?, bool>? MatchOnBodyAsFormUrlEncodedFunc { get; }

    /// <summary>
    /// The matchers.
    /// </summary>
    public IMatcher[]? Matchers { get; }

    /// <summary>
    /// The <see cref="MatchOperator"/>
    /// </summary>
    public MatchOperator MatchOperator { get; } = MatchOperator.Or;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageBodyMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="body">The body.</param>
    public RequestMessageBodyMatcher(MatchBehaviour matchBehaviour, string body) :
        this(new[] { new WildcardMatcher(matchBehaviour, body) }.Cast<IMatcher>().ToArray())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageBodyMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="body">The body.</param>
    public RequestMessageBodyMatcher(MatchBehaviour matchBehaviour, byte[] body) :
        this(new[] { new ExactObjectMatcher(matchBehaviour, body) }.Cast<IMatcher>().ToArray())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageBodyMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="body">The body.</param>
    public RequestMessageBodyMatcher(MatchBehaviour matchBehaviour, object body) :
        this(new[] { new ExactObjectMatcher(matchBehaviour, body) }.Cast<IMatcher>().ToArray())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageBodyMatcher"/> class.
    /// </summary>
    /// <param name="func">The function.</param>
    public RequestMessageBodyMatcher(Func<string?, bool> func)
    {
        MatchOnBodyAsStringFunc = Guard.NotNull(func);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageBodyMatcher"/> class.
    /// </summary>
    /// <param name="func">The function.</param>
    public RequestMessageBodyMatcher(Func<byte[]?, bool> func)
    {
        MatchOnBodyAsBytesFunc = Guard.NotNull(func);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageBodyMatcher"/> class.
    /// </summary>
    /// <param name="func">The function.</param>
    public RequestMessageBodyMatcher(Func<object?, bool> func)
    {
        MatchOnBodyAsJsonFunc = Guard.NotNull(func);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageBodyMatcher"/> class.
    /// </summary>
    /// <param name="func">The function.</param>
    public RequestMessageBodyMatcher(Func<IBodyData?, bool> func)
    {
        MatchOnBodyAsBodyDataFunc = Guard.NotNull(func);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageBodyMatcher"/> class.
    /// </summary>
    /// <param name="func">The function.</param>
    public RequestMessageBodyMatcher(Func<IDictionary<string, string>?, bool> func)
    {
        MatchOnBodyAsFormUrlEncodedFunc = Guard.NotNull(func);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageBodyMatcher"/> class.
    /// </summary>
    /// <param name="matchers">The matchers.</param>
    public RequestMessageBodyMatcher(params IMatcher[] matchers)
    {
        Matchers = Guard.NotNull(matchers);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageBodyMatcher"/> class.
    /// </summary>
    /// <param name="matchers">The matchers.</param>
    /// <param name="matchOperator">The <see cref="MatchOperator"/> to use.</param>
    public RequestMessageBodyMatcher(MatchOperator matchOperator, params IMatcher[] matchers)
    {
        Matchers = Guard.NotNull(matchers);
        MatchOperator = matchOperator;
    }

    /// <inheritdoc />
    public double GetMatchingScore(IRequestMessage requestMessage, IRequestMatchResult requestMatchResult)
    {
        var (score, exception) = CalculateMatchResult(requestMessage).Expand();
        return requestMatchResult.AddScore(GetType(), score, exception);
    }

    private MatchResult CalculateMatchResult(IRequestMessage requestMessage)
    {
        if (Matchers != null && Matchers.Any())
        {
            var results = Matchers.Select(matcher => BodyDataMatchScoreCalculator.CalculateMatchScore(requestMessage.BodyData, matcher)).ToArray();
            return MatchResult.From(nameof(RequestMessageBodyMatcher), results, MatchOperator);
        }

        if (MatchOnBodyAsStringFunc != null)
        {
            return MatchResult.From($"{nameof(RequestMessageBodyMatcher)}:{nameof(MatchOnBodyAsStringFunc)}", MatchScores.ToScore(MatchOnBodyAsStringFunc(requestMessage.BodyData?.BodyAsString)));
        }

        if (MatchOnBodyAsFormUrlEncodedFunc != null)
        {
            return MatchResult.From($"{nameof(RequestMessageBodyMatcher)}:{nameof(MatchOnBodyAsFormUrlEncodedFunc)}", MatchScores.ToScore(MatchOnBodyAsFormUrlEncodedFunc(requestMessage.BodyData?.BodyAsFormUrlEncoded)));
        }

        if (MatchOnBodyAsJsonFunc != null)
        {
            return MatchResult.From($"{nameof(RequestMessageBodyMatcher)}:{nameof(MatchOnBodyAsJsonFunc)}", MatchScores.ToScore(MatchOnBodyAsJsonFunc(requestMessage.BodyData?.BodyAsJson)));
        }

        if (MatchOnBodyAsBytesFunc != null)
        {
            return MatchResult.From($"{nameof(RequestMessageBodyMatcher)}:{nameof(MatchOnBodyAsBytesFunc)}", MatchScores.ToScore(MatchOnBodyAsBytesFunc(requestMessage.BodyData?.BodyAsBytes)));
        }

        if (MatchOnBodyAsBodyDataFunc != null)
        {
            return MatchResult.From($"{nameof(RequestMessageBodyMatcher)}:{nameof(MatchOnBodyAsBodyDataFunc)}", MatchScores.ToScore(MatchOnBodyAsBodyDataFunc(requestMessage.BodyData)));
        }

        return MatchResult.From(nameof(RequestMessageBodyMatcher));
    }
}