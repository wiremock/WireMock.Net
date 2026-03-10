// Copyright © WireMock.Net

using WireMock.Handlers;
using Xunit;

namespace WireMock.Net.Tests.Handlers;

public class InMemoryScenarioStateStoreTests
{
    private readonly InMemoryScenarioStateStore _sut = new();

    [Fact]
    public void TryAdd_ShouldAddNewScenario()
    {
        var state = new ScenarioState { Name = "scenario1" };

        _sut.TryAdd("scenario1", state).Should().BeTrue();

        _sut.ContainsKey("scenario1").Should().BeTrue();
    }

    [Fact]
    public void TryAdd_ShouldReturnFalse_WhenScenarioAlreadyExists()
    {
        var state = new ScenarioState { Name = "scenario1" };
        _sut.TryAdd("scenario1", state);

        _sut.TryAdd("scenario1", new ScenarioState { Name = "scenario1" }).Should().BeFalse();
    }

    [Fact]
    public void Get_ShouldReturnScenarioState_WhenExists()
    {
        var state = new ScenarioState { Name = "scenario1", NextState = "state2" };
        _sut.TryAdd("scenario1", state);

        var result = _sut.Get("scenario1");

        result.Should().NotBeNull();
        result!.NextState.Should().Be("state2");
    }

    [Fact]
    public void Get_ShouldReturnNull_WhenNotExists()
    {
        _sut.Get("nonexistent").Should().BeNull();
    }

    [Fact]
    public void GetAll_ShouldReturnAllScenarios()
    {
        _sut.TryAdd("scenario1", new ScenarioState { Name = "scenario1" });
        _sut.TryAdd("scenario2", new ScenarioState { Name = "scenario2" });

        var result = _sut.GetAll();

        result.Should().HaveCount(2);
    }

    [Fact]
    public void GetAll_ShouldReturnEmpty_WhenNoScenarios()
    {
        _sut.GetAll().Should().BeEmpty();
    }

    [Fact]
    public void Update_ShouldModifyExistingScenario()
    {
        _sut.TryAdd("scenario1", new ScenarioState { Name = "scenario1", Counter = 0 });

        var result = _sut.Update("scenario1", s => { s.Counter = 5; s.NextState = "state2"; });

        result.Should().NotBeNull();
        result!.Counter.Should().Be(5);
        result.NextState.Should().Be("state2");
    }

    [Fact]
    public void Update_ShouldReturnNull_WhenNotExists()
    {
        _sut.Update("nonexistent", s => { s.Counter = 5; }).Should().BeNull();
    }

    [Fact]
    public void AddOrUpdate_ShouldAddNewScenario()
    {
        var result = _sut.AddOrUpdate(
            "scenario1",
            _ => new ScenarioState { Name = "scenario1", NextState = "added" },
            (_, current) => { current.NextState = "updated"; return current; }
        );

        result.NextState.Should().Be("added");
    }

    [Fact]
    public void AddOrUpdate_ShouldUpdateExistingScenario()
    {
        _sut.TryAdd("scenario1", new ScenarioState { Name = "scenario1", NextState = "initial" });

        var result = _sut.AddOrUpdate(
            "scenario1",
            _ => new ScenarioState { Name = "scenario1", NextState = "added" },
            (_, current) => { current.NextState = "updated"; return current; }
        );

        result.NextState.Should().Be("updated");
    }

    [Fact]
    public void TryRemove_ShouldRemoveExistingScenario()
    {
        _sut.TryAdd("scenario1", new ScenarioState { Name = "scenario1" });

        _sut.TryRemove("scenario1").Should().BeTrue();
        _sut.ContainsKey("scenario1").Should().BeFalse();
    }

    [Fact]
    public void TryRemove_ShouldReturnFalse_WhenNotExists()
    {
        _sut.TryRemove("nonexistent").Should().BeFalse();
    }

    [Fact]
    public void Clear_ShouldRemoveAllScenarios()
    {
        _sut.TryAdd("scenario1", new ScenarioState { Name = "scenario1" });
        _sut.TryAdd("scenario2", new ScenarioState { Name = "scenario2" });

        _sut.Clear();

        _sut.GetAll().Should().BeEmpty();
    }

    [Fact]
    public void ContainsKey_ShouldBeCaseInsensitive()
    {
        _sut.TryAdd("Scenario1", new ScenarioState { Name = "Scenario1" });

        _sut.ContainsKey("scenario1").Should().BeTrue();
        _sut.ContainsKey("SCENARIO1").Should().BeTrue();
    }

    [Fact]
    public void Get_ShouldBeCaseInsensitive()
    {
        _sut.TryAdd("Scenario1", new ScenarioState { Name = "Scenario1", NextState = "state2" });

        _sut.Get("scenario1")!.NextState.Should().Be("state2");
        _sut.Get("SCENARIO1")!.NextState.Should().Be("state2");
    }
}
