// Copyright © WireMock.Net

namespace WireMock.Admin.Mappings;

/// <summary>
/// Cookie Model
/// </summary>
[FluentBuilder.AutoGenerateBuilder]
public class CookieModel
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the matchers.
    /// </summary>
    public IList<MatcherModel>? Matchers { get; set; }

    /// <summary>
    /// Gets or sets the ignore case for the Cookie Name.
    /// </summary>
    public bool? IgnoreCase { get; set; }

    /// <summary>
    /// Gets or sets the Reject on match for the Cookie Name.
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