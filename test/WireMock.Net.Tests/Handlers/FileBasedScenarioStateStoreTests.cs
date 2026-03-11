// Copyright © WireMock.Net

using System;
using System.IO;
using Newtonsoft.Json;
using WireMock.Handlers;
using Xunit;

namespace WireMock.Net.Tests.Handlers;

public class FileBasedScenarioStateStoreTests : IDisposable
{
    private readonly string _tempFolder;
    private readonly string _scenariosFolder;

    public FileBasedScenarioStateStoreTests()
    {
        _tempFolder = Path.Combine(Path.GetTempPath(), "WireMock_Tests_" + Guid.NewGuid().ToString("N"));
        _scenariosFolder = Path.Combine(_tempFolder, "__admin", "scenarios");
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempFolder))
        {
            Directory.Delete(_tempFolder, true);
        }
    }

    private FileBasedScenarioStateStore CreateSut() => new(_tempFolder);

    // --- Mirror tests from InMemoryScenarioStateStoreTests ---

    [Fact]
    public void TryAdd_ShouldAddNewScenario()
    {
        var sut = CreateSut();
        var state = new ScenarioState { Name = "scenario1" };

        sut.TryAdd("scenario1", state).Should().BeTrue();

        sut.ContainsKey("scenario1").Should().BeTrue();
    }

    [Fact]
    public void TryAdd_ShouldReturnFalse_WhenScenarioAlreadyExists()
    {
        var sut = CreateSut();
        var state = new ScenarioState { Name = "scenario1" };
        sut.TryAdd("scenario1", state);

        sut.TryAdd("scenario1", new ScenarioState { Name = "scenario1" }).Should().BeFalse();
    }

    [Fact]
    public void TryGet_ShouldReturnTrue_WhenExists()
    {
        var sut = CreateSut();
        var state = new ScenarioState { Name = "scenario1", NextState = "state2" };
        sut.TryAdd("scenario1", state);

        sut.TryGet("scenario1", out var result).Should().BeTrue();

        result.Should().NotBeNull();
        result!.NextState.Should().Be("state2");
    }

    [Fact]
    public void TryGet_ShouldReturnFalse_WhenNotExists()
    {
        var sut = CreateSut();
        sut.TryGet("nonexistent", out var result).Should().BeFalse();
        result.Should().BeNull();
    }

    [Fact]
    public void GetAll_ShouldReturnAllScenarios()
    {
        var sut = CreateSut();
        sut.TryAdd("scenario1", new ScenarioState { Name = "scenario1" });
        sut.TryAdd("scenario2", new ScenarioState { Name = "scenario2" });

        var result = sut.GetAll();

        result.Should().HaveCount(2);
    }

    [Fact]
    public void GetAll_ShouldReturnEmpty_WhenNoScenarios()
    {
        var sut = CreateSut();
        sut.GetAll().Should().BeEmpty();
    }

    [Fact]
    public void Update_ShouldModifyExistingScenario()
    {
        var sut = CreateSut();
        sut.TryAdd("scenario1", new ScenarioState { Name = "scenario1", Counter = 0 });

        var result = sut.Update("scenario1", s => { s.Counter = 5; s.NextState = "state2"; });

        result.Should().NotBeNull();
        result!.Counter.Should().Be(5);
        result.NextState.Should().Be("state2");
    }

    [Fact]
    public void Update_ShouldReturnNull_WhenNotExists()
    {
        var sut = CreateSut();
        sut.Update("nonexistent", s => { s.Counter = 5; }).Should().BeNull();
    }

    [Fact]
    public void AddOrUpdate_ShouldAddNewScenario()
    {
        var sut = CreateSut();
        var result = sut.AddOrUpdate(
            "scenario1",
            _ => new ScenarioState { Name = "scenario1", NextState = "added" },
            (_, current) => { current.NextState = "updated"; return current; }
        );

        result.NextState.Should().Be("added");
    }

    [Fact]
    public void AddOrUpdate_ShouldUpdateExistingScenario()
    {
        var sut = CreateSut();
        sut.TryAdd("scenario1", new ScenarioState { Name = "scenario1", NextState = "initial" });

        var result = sut.AddOrUpdate(
            "scenario1",
            _ => new ScenarioState { Name = "scenario1", NextState = "added" },
            (_, current) => { current.NextState = "updated"; return current; }
        );

        result.NextState.Should().Be("updated");
    }

    [Fact]
    public void TryRemove_ShouldRemoveExistingScenario()
    {
        var sut = CreateSut();
        sut.TryAdd("scenario1", new ScenarioState { Name = "scenario1" });

        sut.TryRemove("scenario1").Should().BeTrue();
        sut.ContainsKey("scenario1").Should().BeFalse();
    }

    [Fact]
    public void TryRemove_ShouldReturnFalse_WhenNotExists()
    {
        var sut = CreateSut();
        sut.TryRemove("nonexistent").Should().BeFalse();
    }

    [Fact]
    public void Clear_ShouldRemoveAllScenarios()
    {
        var sut = CreateSut();
        sut.TryAdd("scenario1", new ScenarioState { Name = "scenario1" });
        sut.TryAdd("scenario2", new ScenarioState { Name = "scenario2" });

        sut.Clear();

        sut.GetAll().Should().BeEmpty();
    }

    [Fact]
    public void ContainsKey_ShouldBeCaseInsensitive()
    {
        var sut = CreateSut();
        sut.TryAdd("Scenario1", new ScenarioState { Name = "Scenario1" });

        sut.ContainsKey("scenario1").Should().BeTrue();
        sut.ContainsKey("SCENARIO1").Should().BeTrue();
    }

    [Fact]
    public void TryGet_ShouldBeCaseInsensitive()
    {
        var sut = CreateSut();
        sut.TryAdd("Scenario1", new ScenarioState { Name = "Scenario1", NextState = "state2" });

        sut.TryGet("scenario1", out var result1).Should().BeTrue();
        result1!.NextState.Should().Be("state2");

        sut.TryGet("SCENARIO1", out var result2).Should().BeTrue();
        result2!.NextState.Should().Be("state2");
    }

    // --- File-persistence-specific tests ---

    [Fact]
    public void TryAdd_ShouldCreateJsonFileOnDisk()
    {
        var sut = CreateSut();
        sut.TryAdd("scenario1", new ScenarioState { Name = "scenario1", NextState = "state2" });

        var filePath = Path.Combine(_scenariosFolder, "scenario1.json");
        File.Exists(filePath).Should().BeTrue();

        var json = File.ReadAllText(filePath);
        var deserialized = JsonConvert.DeserializeObject<ScenarioState>(json);
        deserialized!.Name.Should().Be("scenario1");
        deserialized.NextState.Should().Be("state2");
    }

    [Fact]
    public void TryRemove_ShouldDeleteJsonFileFromDisk()
    {
        var sut = CreateSut();
        sut.TryAdd("scenario1", new ScenarioState { Name = "scenario1" });

        var filePath = Path.Combine(_scenariosFolder, "scenario1.json");
        File.Exists(filePath).Should().BeTrue();

        sut.TryRemove("scenario1");

        File.Exists(filePath).Should().BeFalse();
    }

    [Fact]
    public void Clear_ShouldDeleteAllJsonFilesFromDisk()
    {
        var sut = CreateSut();
        sut.TryAdd("scenario1", new ScenarioState { Name = "scenario1" });
        sut.TryAdd("scenario2", new ScenarioState { Name = "scenario2" });

        Directory.GetFiles(_scenariosFolder, "*.json").Should().HaveCount(2);

        sut.Clear();

        Directory.GetFiles(_scenariosFolder, "*.json").Should().BeEmpty();
    }

    [Fact]
    public void Constructor_ShouldLoadExistingScenariosFromDisk()
    {
        // Pre-write JSON files before constructing the store
        Directory.CreateDirectory(_scenariosFolder);
        var state1 = new ScenarioState { Name = "scenario1", NextState = "loaded1" };
        var state2 = new ScenarioState { Name = "scenario2", NextState = "loaded2", Counter = 3 };
        File.WriteAllText(Path.Combine(_scenariosFolder, "scenario1.json"), JsonConvert.SerializeObject(state1));
        File.WriteAllText(Path.Combine(_scenariosFolder, "scenario2.json"), JsonConvert.SerializeObject(state2));

        var sut = CreateSut();

        sut.GetAll().Should().HaveCount(2);
        sut.TryGet("scenario1", out var loaded1).Should().BeTrue();
        loaded1!.NextState.Should().Be("loaded1");
        sut.TryGet("scenario2", out var loaded2).Should().BeTrue();
        loaded2!.Counter.Should().Be(3);
    }

    [Fact]
    public void Update_ShouldPersistChangesToDisk()
    {
        var sut = CreateSut();
        sut.TryAdd("scenario1", new ScenarioState { Name = "scenario1", Counter = 0 });

        sut.Update("scenario1", s => { s.Counter = 10; s.NextState = "persisted"; });

        var filePath = Path.Combine(_scenariosFolder, "scenario1.json");
        var json = File.ReadAllText(filePath);
        var deserialized = JsonConvert.DeserializeObject<ScenarioState>(json);
        deserialized!.Counter.Should().Be(10);
        deserialized.NextState.Should().Be("persisted");
    }
}
