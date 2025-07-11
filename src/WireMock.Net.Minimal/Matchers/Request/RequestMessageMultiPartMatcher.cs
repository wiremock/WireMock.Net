// Copyright © WireMock.Net

using System;
using System.Linq;
using Stef.Validation;
using WireMock.Util;

namespace WireMock.Matchers.Request;

/// <summary>
/// The request body MultiPart matcher.
/// </summary>
public class RequestMessageMultiPartMatcher : IRequestMatcher
{
    private static readonly IMimeKitUtils MimeKitUtils = TypeLoader.LoadStaticInstance<IMimeKitUtils>();

    /// <summary>
    /// The matchers.
    /// </summary>
    public IMatcher[]? Matchers { get; }

    /// <summary>
    /// The <see cref="MatchOperator"/>
    /// </summary>
    public MatchOperator MatchOperator { get; } = MatchOperator.Or;

    /// <summary>
    /// The <see cref="MatchBehaviour"/>
    /// </summary>
    public MatchBehaviour MatchBehaviour { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageMultiPartMatcher"/> class.
    /// </summary>
    /// <param name="matchers">The matchers.</param>
    public RequestMessageMultiPartMatcher(params IMatcher[] matchers)
    {
        Matchers = Guard.NotNull(matchers);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageMultiPartMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="matchOperator">The <see cref="MatchOperator"/> to use.</param>
    /// <param name="matchers">The matchers.</param>
    public RequestMessageMultiPartMatcher(MatchBehaviour matchBehaviour, MatchOperator matchOperator, params IMatcher[] matchers)
    {
        Matchers = Guard.NotNull(matchers);
        MatchBehaviour = matchBehaviour;
        MatchOperator = matchOperator;
    }

    /// <inheritdoc />
    public double GetMatchingScore(IRequestMessage requestMessage, IRequestMatchResult requestMatchResult)
    {
        var score = MatchScores.Mismatch;
        Exception? exception = null;

        if (Matchers?.Any() != true)
        {
            return requestMatchResult.AddScore(GetType(), score, null);
        }

        if (!MimeKitUtils.TryGetMimeMessage(requestMessage, out var message))
        {
            return requestMatchResult.AddScore(GetType(), score, null);
        }

        try
        {
            foreach (var mimePartMatcher in Matchers.OfType<IMimePartMatcher>().ToArray())
            {
                score = MatchScores.Mismatch;

                foreach (var mimeBodyPart in message.BodyParts)
                {
                    var matchResult = mimePartMatcher.IsMatch(mimeBodyPart);
                    if (matchResult.IsPerfect())
                    {
                        score = MatchScores.Perfect;
                        break;
                    }
                }

                if ((MatchOperator == MatchOperator.Or && MatchScores.IsPerfect(score)) || (MatchOperator == MatchOperator.And && !MatchScores.IsPerfect(score)))
                {
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        return requestMatchResult.AddScore(GetType(), score, exception);
    }
}