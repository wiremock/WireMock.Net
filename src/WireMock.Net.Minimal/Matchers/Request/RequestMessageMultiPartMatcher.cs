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
    /// <summary>
    /// The name of this matcher.
    /// </summary>
    public const string Name = "MultiPartMatcher";

    private readonly IMimeKitUtils _mimeKitUtils = LoadMimeKitUtils();

    /// <summary>
    /// The matchers.
    /// </summary>
    public IMatcher[]? Matchers { get; }

    /// <summary>
    /// The <see cref="MatchOperator"/>
    /// </summary>
    public MatchOperator MatchOperator { get; } = MatchOperator.And;

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
        var matchDetail = MatchResult.From(Name).ToMatchDetail();
        Exception? exception = null;

        if (Matchers == null)
        {
            return requestMatchResult.AddMatchDetail(matchDetail);
        }

        if (!_mimeKitUtils.TryGetMimeMessage(requestMessage, out var message))
        {
            return requestMatchResult.AddMatchDetail(matchDetail);
        }

        double score = MatchScores.Mismatch;
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

        return requestMatchResult.AddMatchDetail(MatchResult.From(Name, score, exception).ToMatchDetail());
    }

    private static IMimeKitUtils LoadMimeKitUtils()
    {
        if (TypeLoader.TryLoadStaticInstance<IMimeKitUtils>(out var mimeKitUtils))
        {
            return mimeKitUtils;
        }

        throw new InvalidOperationException("MimeKit is required for RequestMessageMultiPartMatcher. Please install the WireMock.Net.MimePart package.");
    }
}