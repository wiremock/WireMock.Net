// Copyright © WireMock.Net

using System.Linq;
using Stef.Validation;
using WireMock.Types;

namespace WireMock.Matchers.Request;

/// <summary>
/// The request parameters matcher.
/// </summary>
public class RequestMessageParamMatcher : IRequestMatcher
{
    /// <summary>
    /// MatchBehaviour
    /// </summary>
    public MatchBehaviour MatchBehaviour { get; }

    /// <summary>
    /// The funcs
    /// </summary>
    public Func<IDictionary<string, WireMockList<string>>, bool>[]? Funcs { get; }

    /// <summary>
    /// The key
    /// </summary>
    public string Key { get; } = string.Empty;

    /// <summary>
    /// Defines if the key should be matched using case-ignore.
    /// </summary>
    public bool IgnoreCase { get; }

    /// <summary>
    /// The matchers.
    /// </summary>
    public IReadOnlyList<IStringMatcher>? Matchers { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageParamMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="key">The key.</param>
    /// <param name="ignoreCase">Defines if the key should be matched using case-ignore.</param>
    public RequestMessageParamMatcher(MatchBehaviour matchBehaviour, string key, bool ignoreCase) : this(matchBehaviour, key, ignoreCase, (IStringMatcher[]?)null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageParamMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="key">The key.</param>
    /// <param name="ignoreCase">Defines if the key should be matched using case-ignore.</param>
    /// <param name="values">The values.</param>
    public RequestMessageParamMatcher(MatchBehaviour matchBehaviour, string key, bool ignoreCase, params string[]? values) :
        this(matchBehaviour, key, ignoreCase, values?.Select(value => new ExactMatcher(matchBehaviour, ignoreCase, MatchOperator.And, value)).Cast<IStringMatcher>().ToArray())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageParamMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="key">The key.</param>
    /// <param name="ignoreCase">Defines if the key should be matched using case-ignore.</param>
    /// <param name="matchers">The matchers.</param>
    public RequestMessageParamMatcher(MatchBehaviour matchBehaviour, string key, bool ignoreCase, params IStringMatcher[]? matchers)
    {
        MatchBehaviour = matchBehaviour;
        Key = Guard.NotNull(key);
        IgnoreCase = ignoreCase;
        Matchers = matchers;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestMessageParamMatcher"/> class.
    /// </summary>
    /// <param name="funcs">The funcs.</param>
    public RequestMessageParamMatcher(params Func<IDictionary<string, WireMockList<string>>, bool>[] funcs)
    {
        Funcs = Guard.NotNull(funcs);
    }

    /// <inheritdoc />
    public double GetMatchingScore(IRequestMessage requestMessage, IRequestMatchResult requestMatchResult)
    {
        var score = GetMatchScore(requestMessage);
        return requestMatchResult.AddScore(GetType(), MatchBehaviourHelper.Convert(MatchBehaviour, score), null);
    }

    private double GetMatchScore(IRequestMessage requestMessage)
    {
        if (Funcs != null)
        {
            return MatchScores.ToScore(requestMessage.Query != null && Funcs.Any(f => f(requestMessage.Query)));
        }

        var valuesPresentInRequestMessage = ((RequestMessage)requestMessage).GetParameter(Key, IgnoreCase);
        if (valuesPresentInRequestMessage == null)
        {
            // Key is not present at all, just return Mismatch
            return MatchScores.Mismatch;
        }

        if (Matchers == null || !Matchers.Any())
        {
            // Matchers are null or not defined, and Key is present, just return Perfect.
            return MatchScores.Perfect;
        }

        // Return the score based on Matchers and valuesPresentInRequestMessage
        return CalculateScore(Matchers, valuesPresentInRequestMessage);
    }

    private static double CalculateScore(IReadOnlyList<IStringMatcher> matchers, WireMockList<string> valuesPresentInRequestMessage)
    {
        var total = new List<double>();

        // If the total patterns in all matchers > values in message, use the matcher as base
        if (matchers.Sum(m => m.GetPatterns().Length) > valuesPresentInRequestMessage.Count)
        {
            foreach (var matcher in matchers)
            {
                double score = 0d;
                foreach (string valuePresentInRequestMessage in valuesPresentInRequestMessage)
                {
                    score += matcher.IsMatch(valuePresentInRequestMessage).Score / matcher.GetPatterns().Length;
                }

                total.Add(score);
            }
        }
        else
        {
            foreach (string valuePresentInRequestMessage in valuesPresentInRequestMessage)
            {
                var score = matchers.Max(m => m.IsMatch(valuePresentInRequestMessage).Score);
                total.Add(score);
            }
        }

        return total.Any() ? MatchScores.ToScore(total, MatchOperator.Average) : 0;
    }
}