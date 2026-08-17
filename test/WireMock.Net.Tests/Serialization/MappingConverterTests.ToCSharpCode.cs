// Copyright © WireMock.Net

using WireMock.Matchers.Request;
using WireMock.Net.Tests.VerifyExtensions;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Serialization;
using WireMock.Types;

namespace WireMock.Net.Tests.Serialization;

public partial class MappingConverterTests
{
    private static readonly VerifySettings VerifySettings = new();
    static MappingConverterTests()
    {
        VerifySettings.Init();
    }

    [Fact]
    public Task ToCSharpCode_With_Builder_And_AddStartIsTrue()
    {
        // Assign
        var mapping = CreateMapping();

        // Act
        var code = _sut.ToCSharpCode(mapping, new MappingConverterSettings
        {
            AddStart = true,
            ConverterType = MappingConverterType.Builder
        });

        // Assert
        code.Should().NotBeEmpty();

        // Verify
        return Verify(code, VerifySettings);
    }

    [Fact]
    public Task ToCSharpCode_With_Builder_And_AddStartIsFalse()
    {
        // Assign
        var mapping = CreateMapping();

        // Act
        var code = _sut.ToCSharpCode(mapping, new MappingConverterSettings
        {
            AddStart = false,
            ConverterType = MappingConverterType.Builder
        });

        // Assert
        code.Should().NotBeEmpty();

        // Verify
        return Verify(code, VerifySettings);
    }

    [Fact]
    public Task ToCSharpCode_With_Server_And_AddStartIsTrue()
    {
        // Assign
        var mapping = CreateMapping();

        // Act
        var code = _sut.ToCSharpCode(mapping, new MappingConverterSettings
        {
            AddStart = true,
            ConverterType = MappingConverterType.Server
        });

        // Assert
        code.Should().NotBeEmpty();

        // Verify
        return Verify(code, VerifySettings);
    }

    [Fact]
    public Task ToCSharpCode_With_Server_And_AddStartIsFalse()
    {
        // Assign
        var mapping = CreateMapping();

        // Act
        var code = _sut.ToCSharpCode(mapping, new MappingConverterSettings
        {
            AddStart = false,
            ConverterType = MappingConverterType.Server
        });

        // Assert
        code.Should().NotBeEmpty();

        // Verify
        return Verify(code, VerifySettings);
    }

    [Fact]
    public Task ToCSharpCode_With_ScenarioAndState_Emits_WhenStateIs_And_WillSetStateTo()
    {
        // Assign: a stateful mapping that gates on state "Begin" and moves to "End" after 3 hits.
        var mapping = CreateScenarioMapping("Ordering", executionConditionState: "Begin", nextState: "End", timesInSameState: 3);

        // Act
        var code = _sut.ToCSharpCode(mapping, new MappingConverterSettings
        {
            AddStart = false,
            ConverterType = MappingConverterType.Server
        });

        // Verify
        return Verify(code, VerifySettings);
    }

    [Fact]
    public Task ToCSharpCode_With_NextStateOnly_Emits_WillSetStateTo_Without_Times()
    {
        // Assign: a start-state mapping that only sets the next state (no execution condition, default times).
        var mapping = CreateScenarioMapping("Ordering", executionConditionState: null, nextState: "Begin", timesInSameState: null);

        // Act
        var code = _sut.ToCSharpCode(mapping, new MappingConverterSettings
        {
            AddStart = false,
            ConverterType = MappingConverterType.Server
        });

        // Verify
        return Verify(code, VerifySettings);
    }

    private Mapping CreateScenarioMapping(string scenario, string? executionConditionState, string? nextState, int? timesInSameState)
    {
        var request = Request.Create().UsingGet().WithPath("/test_path");
        var response = Response.Create().WithSuccess();

        return new Mapping
        (
            new Guid("8e7b9ab7-e18e-4502-8bc9-11e6679811cc"),
            _updatedAt,
            string.Empty,
            string.Empty,
            null,
            _settings,
            request,
            response,
            0,
            scenario,
            executionConditionState,
            nextState,
            timesInSameState,
            null,
            false,
            null,
            data: null
        );
    }

    private IMapping CreateMapping()
    {
        var guid = new Guid("8e7b9ab7-e18e-4502-8bc9-11e6679811cc");
        var request = Request.Create()
            .UsingGet()
            .WithEarlyMismatch(RequestMatcherType.Method)
            .WithPath("/test_path")
            .WithParam("q", "42")
            .WithClientIP("112.123.100.99")
            .WithHeader("h-key", "h-value")
            .WithCookie("c-key", "c-value")
            .WithBody("b");
        var response = Response.Create()
            .WithHeader("Keep-Alive", "test")
            .WithBody("bbb")
            .WithDelay(12345)
            .WithTransformer();

        return new Mapping
        (
            guid,
            _updatedAt,
            string.Empty,
            string.Empty,
            null,
            _settings,
            request,
            response,
            42,
            null,
            null,
            null,
            null,
            null,
            false,
            null,
            data: null
        ).WithProbability(0.3);
    }
}