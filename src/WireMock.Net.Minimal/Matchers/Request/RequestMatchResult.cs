// Copyright © WireMock.Net

using System.Linq;

namespace WireMock.Matchers.Request;

/// <summary>
/// RequestMatchResult
/// </summary>
public class RequestMatchResult : IRequestMatchResult
{
    /// <inheritdoc />
    public double TotalScore => MatchDetails.Sum(md => md.Score);

    /// <inheritdoc />
    public int TotalNumber => MatchDetails.Count;

    /// <inheritdoc />
    public bool IsPerfectMatch => Math.Abs(TotalScore - TotalNumber) < MatchScores.Tolerance;

    /// <inheritdoc />
    public double AverageTotalScore => TotalNumber == 0 ? MatchScores.Mismatch : TotalScore / TotalNumber;

    /// <inheritdoc />
    public IList<MatchDetail> MatchDetails { get; } = [];

    /// <inheritdoc />
    public double AddScore(Type matcherType, double score, Exception? exception)
    {
        return AddMatchDetail(new MatchDetail
        {
            Name = matcherType.Name.Replace("RequestMessage", string.Empty),
            MatcherType = matcherType,
            Score = score,
            Exception = exception
        });
    }

    /// <inheritdoc />
    public double AddMatchDetail(MatchDetail matchDetail)
    {
        MatchDetails.Add(matchDetail);

        return matchDetail.Score;
    }

    /// <summary>
    /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
    /// </summary>
    /// <param name="obj">An object to compare with this instance.</param>
    /// <returns>
    /// A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance precedes <paramref name="obj" /> in the sort order. Zero This instance occurs in the same position in the sort order as <paramref name="obj" />. Greater than zero This instance follows <paramref name="obj" /> in the sort order.
    /// </returns>
    public int CompareTo(object? obj)
    {
        if (obj == null)
        {
            return -1;
        }

        var compareObj = (RequestMatchResult)obj;

        var averageTotalScoreResult = compareObj.AverageTotalScore.CompareTo(AverageTotalScore);

        // In case the score is equal, prefer the one with the most matchers.
        return averageTotalScoreResult == 0 ? compareObj.TotalNumber.CompareTo(TotalNumber) : averageTotalScoreResult;
    }
}