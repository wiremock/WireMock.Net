// Copyright © WireMock.Net

using System.Collections.Generic;
using WireMock.Matchers.Request;

namespace WireMock.Admin.Requests;

/// <summary>
/// LogRequestMatchModel
/// </summary>
public class LogRequestMatchModel
{
    /// <summary>
    /// Gets or sets the match-score.
    /// </summary>
    /// <value>
    /// The match-score.
    /// </value>
    public double TotalScore { get; set; }

    /// <summary>
    /// Gets or sets the total number of matches.
    /// </summary>
    /// <value>
    /// The total number of matches.
    /// </value>
    public int TotalNumber { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is perfect match.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is perfect match; otherwise, <c>false</c>.
    /// </value>
    public bool IsPerfectMatch { get; set; }

    /// <summary>
    /// Gets the match percentage.
    /// </summary>
    /// <value>
    /// The match percentage.
    /// </value>
    public double AverageTotalScore { get; set; }

    /// <summary>
    /// Gets the match details.
    /// </summary>
    /// <value>
    /// The match details.
    /// </value>
    public IList<MatchDetail> MatchDetails { get; set; } = [];
}