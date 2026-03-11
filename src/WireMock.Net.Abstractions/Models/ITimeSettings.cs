// Copyright © WireMock.Net

namespace WireMock.Models;

/// <summary>
/// TimeSettings: Start, End and TTL
/// </summary>
public interface ITimeSettings
{
    /// <summary>
    /// Gets or sets the DateTime from which this mapping should be used. In case this is not defined, it's used (default behavior).
    /// </summary>
    DateTime? Start { get; set; }

    /// <summary>
    /// Gets or sets the DateTime from until this mapping should be used. In case this is not defined, it's used forever (default behavior).
    /// </summary>
    DateTime? End { get; set; }

    /// <summary>
    /// Gets or sets the TTL (Time To Live) in seconds for this mapping. In case this is not defined, it's used (default behavior).
    /// </summary>
    int? TTL { get; set; }
}