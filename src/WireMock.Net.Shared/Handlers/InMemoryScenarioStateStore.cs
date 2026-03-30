// Copyright © WireMock.Net

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace WireMock.Handlers;

/// <summary>
/// Provides an in-memory implementation of the <see cref="IScenarioStateStore" /> interface for managing scenario state objects by name.
/// </summary>
public class InMemoryScenarioStateStore : IScenarioStateStore
{
    private readonly ConcurrentDictionary<string, ScenarioState> _scenarios = new(StringComparer.OrdinalIgnoreCase);

    /// <inheritdoc />
    public bool TryGet(string name, [NotNullWhen(true)] out ScenarioState? state)
    {
        return _scenarios.TryGetValue(name, out state);
    }

    /// <inheritdoc />
    public IReadOnlyList<ScenarioState> GetAll()
    {
        return _scenarios.Values.ToArray();
    }

    /// <inheritdoc />
    public bool ContainsKey(string name)
    {
        return _scenarios.ContainsKey(name);
    }

    /// <inheritdoc />
    public bool TryAdd(string name, ScenarioState scenarioState)
    {
        return _scenarios.TryAdd(name, scenarioState);
    }

    /// <inheritdoc />
    public ScenarioState AddOrUpdate(string name, Func<string, ScenarioState> addFactory, Func<string, ScenarioState, ScenarioState> updateFactory)
    {
        return _scenarios.AddOrUpdate(name, addFactory, updateFactory);
    }

    /// <inheritdoc />
    public ScenarioState? Update(string name, Action<ScenarioState> updateAction)
    {
        if (_scenarios.TryGetValue(name, out var state))
        {
            updateAction(state);
            return state;
        }

        return null;
    }

    /// <inheritdoc />
    public bool TryRemove(string name)
    {
        return _scenarios.TryRemove(name, out _);
    }

    /// <inheritdoc />
    public void Clear()
    {
        _scenarios.Clear();
    }
}