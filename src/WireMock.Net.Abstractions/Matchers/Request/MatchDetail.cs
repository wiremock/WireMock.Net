// Copyright © WireMock.Net

using System;

namespace WireMock.Matchers.Request;

/// <summary>
/// MatchDetail
/// </summary>
public class MatchDetail
{
    /// <summary>
    /// Gets or sets the type of the matcher.
    /// </summary>
    public required Type MatcherType { get; set; }

    /// <summary>
    /// Gets or sets the score between 0.0 and 1.0
    /// </summary>
    public double Score { get; set; }

    /// <summary>
    /// The exception in case the Matcher throws exception.
    /// [Optional]
    /// </summary>
    public Exception? Exception { get; set; }
}