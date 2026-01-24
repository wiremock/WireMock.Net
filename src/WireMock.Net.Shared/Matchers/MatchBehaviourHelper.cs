// Copyright © WireMock.Net

namespace WireMock.Matchers;

/// <summary>
/// MatchBehaviourHelper
/// </summary>
internal static class MatchBehaviourHelper
{
    /// <summary>
    /// Converts the specified match behaviour and match value to a new match value.
    /// 
    /// if AcceptOnMatch --> return match (default)
    /// if RejectOnMatch and match = 0.0 --> return 1.0
    /// if RejectOnMatch and match = 0.? --> return 0.0
    /// if RejectOnMatch and match = 1.0 --> return 0.0
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="match">The match.</param>
    /// <returns>match value</returns>
    internal static double Convert(MatchBehaviour matchBehaviour, double match)
    {
        if (matchBehaviour == MatchBehaviour.AcceptOnMatch)
        {
            return match;
        }

        return match <= MatchScores.Tolerance ? MatchScores.Perfect : MatchScores.Mismatch;
    }

    /// <summary>
    /// Converts the specified match behaviour and match result to a new match result value.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="result">The match result.</param>
    /// <returns>match result</returns>
    internal static MatchResult Convert(MatchBehaviour matchBehaviour, MatchResult result)
    {
        return matchBehaviour == MatchBehaviour.AcceptOnMatch ? result : MatchResult.From(result.Name, Convert(matchBehaviour, result.Score), result.Exception);
    }
}