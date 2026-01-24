// Copyright © WireMock.Net

using System;
using System.Collections.Generic;

namespace WireMock.Matchers.Request;

/// <summary>
/// IRequestMatchResult
/// </summary>
public interface IRequestMatchResult : IComparable
{
    /// <summary>
    /// Gets the match percentage.
    /// </summary>
    /// <value>
    /// The match percentage.
    /// </value>
    double AverageTotalScore { get; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is perfect match.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is perfect match; otherwise, <c>false</c>.
    /// </value>
    bool IsPerfectMatch { get; }

    /// <summary>
    /// Gets the match details.
    /// </summary>
    IList<MatchDetail> MatchDetails { get; }

    /// <summary>
    /// Gets or sets the total number of matches.
    /// </summary>
    /// <value>
    /// The total number of matches.
    /// </value>
    int TotalNumber { get; }

    /// <summary>
    /// Gets or sets the match-score.
    /// </summary>
    /// <value>
    /// The match-score.
    /// </value>
    double TotalScore { get; }

    /// <summary>
    /// Adds the score.
    /// </summary>
    /// <param name="matcherType">The matcher Type.</param>
    /// <param name="score">The score.</param>
    /// <param name="exception">The exception [Optional].</param>
    /// <returns>The score.</returns>
    double AddScore(Type matcherType, double score, Exception? exception);

    /// <summary>
    /// Adds the score.
    /// </summary>
    /// <param name="matchDetail">The matchDetail.</param>
    /// <returns>The score.</returns>
    double AddMatchDetail(MatchDetail matchDetail);
}