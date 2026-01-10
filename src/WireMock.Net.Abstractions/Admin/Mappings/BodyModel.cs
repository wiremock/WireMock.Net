// Copyright © WireMock.Net

namespace WireMock.Admin.Mappings;

/// <summary>
/// Body Model
/// </summary>
[FluentBuilder.AutoGenerateBuilder]
public class BodyModel
{
    /// <summary>
    /// The name of the body matcher.
    /// Currently only "MultiPartMatcher" is supported.
    /// </summary>
    public string? MatcherName { get; set; }

    /// <summary>
    /// Gets or sets the matcher.
    /// </summary>
    public MatcherModel? Matcher { get; set; }

    /// <summary>
    /// Gets or sets the matchers.
    /// </summary>
    public MatcherModel[]? Matchers { get; set; }

    /// <summary>
    /// The Operator to use when matchers are defined. [Optional]
    /// - null      = Same as "or".
    /// - "or"      = Only one pattern should match.
    /// - "and"     = All patterns should match.
    /// - "average" = The average value from all patterns.
    /// </summary>
    public string? MatchOperator { get; set; }
}