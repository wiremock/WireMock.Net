// Copyright © WireMock.Net

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace WireMock.Handlers;

public class FileBasedScenarioStateStore : IScenarioStateStore
{
    private readonly ConcurrentDictionary<string, ScenarioState> _scenarios = new(StringComparer.OrdinalIgnoreCase);
    private readonly string _scenariosFolder;
    private readonly object _lock = new();

    public FileBasedScenarioStateStore(string rootFolder)
    {
        _scenariosFolder = Path.Combine(rootFolder, "__admin", "scenarios");
        Directory.CreateDirectory(_scenariosFolder);
        LoadScenariosFromDisk();
    }

    public bool TryGet(string name, [NotNullWhen(true)] out ScenarioState? state)
    {
        return _scenarios.TryGetValue(name, out state);
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
        if (_scenarios.TryAdd(name, scenarioState))
        {
            WriteScenarioToFile(name, scenarioState);
            return true;
        }

        return false;
    }

    public ScenarioState AddOrUpdate(string name, Func<string, ScenarioState> addFactory, Func<string, ScenarioState, ScenarioState> updateFactory)
    {
        lock (_lock)
        {
            var result = _scenarios.AddOrUpdate(name, addFactory, updateFactory);
            WriteScenarioToFile(name, result);
            return result;
        }
    }

    public ScenarioState? Update(string name, Action<ScenarioState> updateAction)
    {
        lock (_lock)
        {
            if (_scenarios.TryGetValue(name, out var state))
            {
                updateAction(state);
                WriteScenarioToFile(name, state);
                return state;
            }

            return null;
        }
    }

    public bool TryRemove(string name)
    {
        if (_scenarios.TryRemove(name, out _))
        {
            DeleteScenarioFile(name);
            return true;
        }

        return false;
    }

    public void Clear()
    {
        _scenarios.Clear();

        foreach (var file in Directory.GetFiles(_scenariosFolder, "*.json"))
        {
            File.Delete(file);
        }
    }

    private string GetScenarioFilePath(string name)
    {
        var sanitized = string.Concat(name.Select(c => Path.GetInvalidFileNameChars().Contains(c) ? '_' : c));
        return Path.Combine(_scenariosFolder, sanitized + ".json");
    }

    private void WriteScenarioToFile(string name, ScenarioState state)
    {
        var json = JsonConvert.SerializeObject(state, Formatting.Indented);
        File.WriteAllText(GetScenarioFilePath(name), json);
    }

    private void DeleteScenarioFile(string name)
    {
        var path = GetScenarioFilePath(name);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    private void LoadScenariosFromDisk()
    {
        foreach (var file in Directory.GetFiles(_scenariosFolder, "*.json"))
        {
            var json = File.ReadAllText(file);
            var state = JsonConvert.DeserializeObject<ScenarioState>(json);
            if (state != null)
            {
                _scenarios.TryAdd(state.Name, state);
            }
        }
    }
}
