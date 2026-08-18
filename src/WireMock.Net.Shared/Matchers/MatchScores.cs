// Copyright © WireMock.Net

using WireMock.Types;

namespace WireMock.Matchers;

/// <summary>
/// MatchScores
/// </summary>
public static class MatchScores
{
    /// <summary>
    /// The tolerance
    /// </summary>
    public const double Tolerance = 0.000001;

    /// <summary>
    /// The default mismatch score
    /// </summary>
    public const double Mismatch = 0.0;

    /// <summary>
    /// The default perfect match score
    /// </summary>
    public const double Perfect = 1.0;

    /// <summary>
    /// The almost perfect match score
    /// </summary>
    public const double AlmostPerfect = 0.99;

    /// <summary>
    /// Is the value a perfect match?
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>true/false</returns>
    public static bool IsPerfect(double value)
    {
        return Math.Abs(value - Perfect) < Tolerance;
    }

    /// <summary>
    /// Convert a bool to the score.
    /// </summary>
    /// <param name="value">if set to <c>true</c> [value].</param>
    /// <returns>score</returns>
    public static double ToScore(bool value)
    {
        return value ? Perfect : Mismatch;
    }

    /// <summary>
    /// Calculates the score from multiple values.
    /// </summary>
    /// <param name="values">The values.</param>
    /// <param name="matchOperator">The <see cref="MatchOperator"/>.</param>
    /// <returns>average score</returns>
    public static double ToScore(IEnumerable<bool> values, MatchOperator matchOperator)
    {
        return ToScore(values.Select(ToScore).ToArray(), matchOperator);
    }

    /// <summary>
    /// Calculates the score from multiple values.
    /// </summary>
    /// <param name="values">The values.</param>
    /// <param name="matchOperator"></param>
    /// <returns>average score</returns>
    public static double ToScore(IEnumerable<double> values, MatchOperator matchOperator)
    {
        if (!values.Any())
        {
            return Mismatch;
        }

        return matchOperator switch
        {
            MatchOperator.Or => ToScore(values.Any(IsPerfect)),
            MatchOperator.And => ToScore(values.All(IsPerfect)),
            _ => values.Average()
        };
    }

    internal static double ToScore(WireMockList<string> values, IStringMatcher[] matchers, MatchOperator matchOperator = MatchOperator.And)
    {
        // Create a matrix of scores where each row corresponds to a value and each column corresponds to a matcher.
        var matrix = values
            .Select(value => matchers
                .Select(matcher => matcher.IsMatch(value).Score).ToArray()
            )
            .ToArray();

        if (matrix.Length == 0 || matchers.Length == 0)
        {
            return Mismatch;
        }

        // For each value, how well was it matched by its best matcher?
        var rowRange = Enumerable.Range(0, matchers.Length);
        var rowScore = matchOperator == MatchOperator.And ? matrix.Average(row => row.Max()) : matrix.Max(row => row.Max());

        // For each matcher, how well was it satisfied by its best value?
        var columnRange = Enumerable.Range(0, matchers.Length);
        var columnScore = matchOperator == MatchOperator.And ? columnRange.Average(column => matrix.Max(row => row[column])) : columnRange.Max(column => matrix.Max(row => row[column]));

        return matchOperator == MatchOperator.And ? Math.Min(rowScore, columnScore) : Math.Max(rowScore, columnScore);
    }
}