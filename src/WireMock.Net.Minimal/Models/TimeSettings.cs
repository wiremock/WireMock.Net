// Copyright © WireMock.Net

namespace WireMock.Models;

/// <summary>
/// TimeSettingsModel: Start, End and TTL
/// </summary>
public class TimeSettings : ITimeSettings
{
    /// <inheritdoc />
    public DateTime? Start { get; set; }

    /// <inheritdoc />
    public DateTime? End { get; set; }

    /// <inheritdoc />
    public int? TTL { get; set; }
}