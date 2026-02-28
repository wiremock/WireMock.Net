// Copyright © WireMock.Net

namespace WireMock.Admin.Scenarios;

/// <summary>
/// ScenarioStateModel
/// </summary>
[FluentBuilder.AutoGenerateBuilder]
public class ScenarioStateModel
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the NextState.
    /// </summary>
    public string? NextState { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="ScenarioStateModel"/> is started.
    /// </summary>
    public bool Started { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="ScenarioStateModel"/> is finished.
    /// </summary>
    public bool Finished { get; set; }

    /// <summary>
    /// Gets or sets the state counter.
    /// </summary>
    public int Counter { get; set; }
}