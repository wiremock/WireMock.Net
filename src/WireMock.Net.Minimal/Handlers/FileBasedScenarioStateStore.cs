// Copyright © WireMock.Net

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Stef.Validation;

namespace WireMock.Handlers;

/// <summary>
/// Provides a file-based implementation of <see cref="IScenarioStateStore" /> that persists scenario states to disk and allows concurrent access.
/// </summary>
public class FileBasedScenarioStateStore : IScenarioStateStore
{
    private readonly ConcurrentDictionary<string, ScenarioState> _scenarios = new(StringComparer.OrdinalIgnoreCase);
    private readonly string _scenariosFolder;
    private readonly object _lock = new();

    /// <summary>
    /// Initializes a new instance of the FileBasedScenarioStateStore class using the specified root folder as the base directory for scenario state storage.
    /// </summary>
    /// <param name="rootFolder">The root directory under which scenario state data will be stored. Must be a valid file system path.</param>
    public FileBasedScenarioStateStore(string rootFolder)
    {
        Guard.NotNullOrEmpty(rootFolder);

        _scenariosFolder = Path.Combine(rootFolder, "__admin", "scenarios");
        Directory.CreateDirectory(_scenariosFolder);
        LoadScenariosFromDisk();
    }

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
        if (_scenarios.TryAdd(name, scenarioState))
        {
            WriteScenarioToFile(name, scenarioState);
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public ScenarioState AddOrUpdate(string name, Func<string, ScenarioState> addFactory, Func<string, ScenarioState, ScenarioState> updateFactory)
    {
        lock (_lock)
        {
            var result = _scenarios.AddOrUpdate(name, addFactory, updateFactory);
            WriteScenarioToFile(name, result);
            return result;
        }
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    public bool TryRemove(string name)
    {
        if (_scenarios.TryRemove(name, out _))
        {
            DeleteScenarioFile(name);
            return true;
        }

        return false;
    }

    /// <inheritdoc />
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