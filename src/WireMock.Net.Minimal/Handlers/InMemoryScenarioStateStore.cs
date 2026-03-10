// Copyright © WireMock.Net

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace WireMock.Handlers;

public class InMemoryScenarioStateStore : IScenarioStateStore
{
    private readonly ConcurrentDictionary<string, ScenarioState> _scenarios = new(StringComparer.OrdinalIgnoreCase);

    public ScenarioState? Get(string name)
    {
        return _scenarios.TryGetValue(name, out var state) ? state : null;
    }

    public IReadOnlyList<ScenarioState> GetAll()
    {
        return _scenarios.Values.ToArray();
    }

    public bool ContainsKey(string name)
    {
        return _scenarios.ContainsKey(name);
    }

    public bool TryAdd(string name, ScenarioState scenarioState)
    {
        return _scenarios.TryAdd(name, scenarioState);
    }

    public ScenarioState AddOrUpdate(string name, Func<string, ScenarioState> addFactory, Func<string, ScenarioState, ScenarioState> updateFactory)
    {
        return _scenarios.AddOrUpdate(name, addFactory, updateFactory);
    }

    public ScenarioState? Update(string name, Action<ScenarioState> updateAction)
    {
        if (_scenarios.TryGetValue(name, out var state))
        {
            updateAction(state);
            return state;
        }

        return null;
    }

    public bool TryRemove(string name)
    {
        return _scenarios.TryRemove(name, out _);
    }

    public void Clear()
    {
        _scenarios.Clear();
    }
}
