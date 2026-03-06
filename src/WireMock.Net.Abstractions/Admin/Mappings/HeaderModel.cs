// Copyright © WireMock.Net

namespace WireMock.Admin.Mappings;

/// <summary>
/// Header Model
/// </summary>
[FluentBuilder.AutoGenerateBuilder]
public class HeaderModel
{
    /// <summary>
    /// Gets or sets the name (key).
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the matchers.
    /// </summary>
    public IList<MatcherModel>? Matchers { get; set; }

    /// <summary>
    /// Gets or sets the ignore case for the Header Key.
    /// </summary>
    public bool? IgnoreCase { get; set; }

    /// <summary>
    /// Gets or sets the Reject on match for the Header Key.
    /// </summary>
    public bool? RejectOnMatch { get; set; }

    /// <summary>
    /// The Operator to use when matchers are defined. [Optional]
    /// - null      = Same as "or".
    /// - "or"      = Only one pattern should match.
    /// - "and"     = All patterns should match.
    /// - "average" = The average value from all patterns.
    /// </summary>
    public string? MatchOperator { get; set; }
}