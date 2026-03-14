// Copyright © WireMock.Net

namespace WireMock;

/// <summary>
/// The ScenarioState
/// </summary>
public class ScenarioState
{
    /// <summary>
    /// Gets or sets the Name (from the Scenario).
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Gets or sets the NextState.
    /// </summary>
    public string? NextState { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="ScenarioState"/> is started.
    /// </summary>
    public bool Started { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="ScenarioState"/> is finished.
    /// </summary>
    public bool Finished { get; set; }

    /// <summary>
    /// Gets or sets the state counter.
    /// </summary>
    public int Counter { get; set; }
}
