// Copyright © WireMock.Net

using System;
using System.Collections.Generic;

namespace WireMock.Handlers;

public interface IScenarioStateStore
{
    ScenarioState? Get(string name);

    IReadOnlyList<ScenarioState> GetAll();

    bool ContainsKey(string name);

    bool TryAdd(string name, ScenarioState scenarioState);

    ScenarioState AddOrUpdate(string name, Func<string, ScenarioState> addFactory, Func<string, ScenarioState, ScenarioState> updateFactory);

    ScenarioState? Update(string name, Action<ScenarioState> updateAction);

    bool TryRemove(string name);

    void Clear();
}
