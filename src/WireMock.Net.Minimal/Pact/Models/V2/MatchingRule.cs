// Copyright © WireMock.Net

#pragma warning disable CS1591
namespace WireMock.Pact.Models.V2;

public class MatchingRule
{
    /// <summary>
    /// type or regex
    /// </summary>
    public required string Match { get; set; }

    /// <summary>
    /// Used for Match = "type"
    /// </summary>
    public string? Min { get; set; }

    /// <summary>
    /// Used for Match = "type"
    /// </summary>
    public string? Max { get; set; }

    /// <summary>
    /// Used for Match = "regex"
    /// </summary>
    public string? Regex { get; set; }
}