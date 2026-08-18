// Copyright © WireMock.Net

using WireMock.Matchers;
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

    [Fact]
    public Task ToCSharpCode_WithPath_RegexMatcher_MultiplePatterns()
    {
        // Assign
        var mapping = CreateMappingWithRequest(Request.Create()
            .UsingGet()
            .WithPath(new RegexMatcher(MatchBehaviour.AcceptOnMatch, ["/a", "/b"])));

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
    public Task ToCSharpCode_WithPath_SimMetricsMatcher_MultiplePatterns()
    {
        // Assign
        var mapping = CreateMappingWithRequest(Request.Create()
            .UsingGet()
            .WithPath(new SimMetricsMatcher(["/a", "/b"])));

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
    public Task ToCSharpCode_WithPath_ContentTypeMatcher_MultiplePatterns()
    {
        // Assign
        var mapping = CreateMappingWithRequest(Request.Create()
            .UsingGet()
            .WithPath(new ContentTypeMatcher(["text/a", "text/b"])));

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
    public Task ToCSharpCode_WithPath_FormUrlEncodedMatcher_MultiplePatterns()
    {
        // Assign
        var mapping = CreateMappingWithRequest(Request.Create()
            .UsingGet()
            .WithPath(new FormUrlEncodedMatcher(["a=1", "b=2"])));

        // Act
        var code = _sut.ToCSharpCode(mapping, new MappingConverterSettings
        {
            AddStart = false,
            ConverterType = MappingConverterType.Server
        });

        // Verify
        return Verify(code, VerifySettings);
    }

    private Mapping CreateMappingWithRequest(IRequestBuilder requestBuilder)
    {
        return new Mapping
        (
            guid: new Guid("8e7b9ab7-e18e-4502-8bc9-11e6679811cc"),
            updatedAt: _updatedAt,
            title: string.Empty,
            description: string.Empty,
            path: null,
            settings: _settings,
            requestMatcher: (IRequestMatcher)requestBuilder,
            provider: Response.Create().WithSuccess(),
            priority: 0,
            scenario: null,
            executionConditionState: null,
            nextState: null,
            stateTimes: null,
            webhooks: null,
            useWebhooksFireAndForget: false,
            timeSettings: null,
            data: null
        );
    }

    private Mapping CreateScenarioMapping(string scenario, string? executionConditionState, string? nextState, int? timesInSameState)
    {
        var request = Request.Create().UsingGet().WithPath("/test_path");
        var response = Response.Create().WithSuccess();

        return new Mapping
        (
            guid: new Guid("8e7b9ab7-e18e-4502-8bc9-11e6679811cc"),
            updatedAt: _updatedAt,
            title: string.Empty,
            description: string.Empty,
            path: null,
            settings: _settings,
            requestMatcher: request,
            provider: response,
            priority: 0,
            scenario: scenario,
            executionConditionState: executionConditionState,
            nextState: nextState,
            stateTimes: timesInSameState,
            webhooks: null,
            useWebhooksFireAndForget: false,
            timeSettings: null,
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
            guid: guid,
            updatedAt: _updatedAt,
            title: string.Empty,
            description: string.Empty,
            path: null,
            settings: _settings,
            requestMatcher: request,
            provider: response,
            priority: 42,
            scenario: null,
            executionConditionState: null,
            nextState: null,
            stateTimes: null,
            webhooks: null,
            useWebhooksFireAndForget: false,
            timeSettings: null,
            data: null
        ).WithProbability(0.3);
    }
}