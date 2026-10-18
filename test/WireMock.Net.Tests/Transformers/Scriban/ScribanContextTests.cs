// Copyright © WireMock.Net

using Moq;
using Scriban.Runtime;
using WireMock.Handlers;
using WireMock.Models;
using WireMock.Transformers;
using WireMock.Transformers.Scriban;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.Net.Tests.Transformers.Scriban;

public class ScribanContextTests
{
    private readonly IFileSystemHandler _fileSystemHandler = Mock.Of<IFileSystemHandler>();

    [Theory]
    [InlineData(TransformerType.Scriban)]
    [InlineData(TransformerType.ScribanDotLiquid)]
    public void ParseAndRender_WithCustomFunction_RendersFunctionResult(TransformerType transformerType)
    {
        // Arrange
        var customGlobals = new ScriptObject();
        customGlobals.Import("fake_name", new Func<string>(() => "Jane Doe"));
        var sut = new ScribanContext(_fileSystemHandler, transformerType, customGlobals);

        // Act
        var result = sut.ParseAndRender("Hello {{ fake_name }} at {{ request.Path }}", CreateModel());

        // Assert
        result.Should().Be("Hello Jane Doe at /foo/bar");
    }

    [Fact]
    public void ParseAndRender_Scriban_WithCustomFunctionWithArgument_RendersFunctionResult()
    {
        // Arrange
        var customGlobals = new ScriptObject();
        customGlobals.Import("greet", new Func<string, string>(name => $"Hello {name}"));
        var sut = new ScribanContext(_fileSystemHandler, TransformerType.Scriban, customGlobals);

        // Act
        var result = sut.ParseAndRender("{{ greet request.Method }}", CreateModel());

        // Assert
        result.Should().Be("Hello GET");
    }

    [Fact]
    public void ParseAndRender_ScribanDotLiquid_WithCustomFunctionAsFilter_RendersFunctionResult()
    {
        // Arrange
        var customGlobals = new ScriptObject();
        customGlobals.Import("greet", new Func<string, string>(name => $"Hello {name}"));
        var sut = new ScribanContext(_fileSystemHandler, TransformerType.ScribanDotLiquid, customGlobals);

        // Act
        var result = sut.ParseAndRender("{{ request.Method | greet }}", CreateModel());

        // Assert
        result.Should().Be("Hello GET");
    }

    [Theory]
    [InlineData(TransformerType.Scriban)]
    [InlineData(TransformerType.ScribanDotLiquid)]
    public void ParseAndRender_WithCustomGlobalNamedLikeModelMember_ModelTakesPrecedence(TransformerType transformerType)
    {
        // Arrange
        var customGlobals = new ScriptObject();
        customGlobals.SetValue("request", "custom", false);
        customGlobals.SetValue("other", "custom", false);
        var sut = new ScribanContext(_fileSystemHandler, transformerType, customGlobals);

        // Act
        var result = sut.ParseAndRender("{{ request.Path }} {{ other }}", CreateModel());

        // Assert
        result.Should().Be("/foo/bar custom");
    }

    [Theory]
    [InlineData(TransformerType.Scriban)]
    [InlineData(TransformerType.ScribanDotLiquid)]
    public void ParseAndRender_WithCustomGlobalNamedLikeBuiltin_CustomGlobalTakesPrecedence(TransformerType transformerType)
    {
        // Arrange
        var customGlobals = new ScriptObject();
        customGlobals.SetValue("date", "custom date", false);
        var sut = new ScribanContext(_fileSystemHandler, transformerType, customGlobals);

        // Act
        var result = sut.ParseAndRender("{{ date }}", CreateModel());

        // Assert
        result.Should().Be("custom date");
    }

    [Fact]
    public void ParseAndRender_WithTemplateAssignment_DoesNotMutateCustomGlobals()
    {
        // Arrange
        var customGlobals = new ScriptObject();
        customGlobals.SetValue("x", "custom", false);
        var sut = new ScribanContext(_fileSystemHandler, TransformerType.Scriban, customGlobals);

        // Act
        var first = sut.ParseAndRender("{{ x = 'assigned'; x }} {{ y = 1; y }}", CreateModel());
        var second = sut.ParseAndRender("{{ x }}", CreateModel());

        // Assert
        first.Should().Be("assigned 1");
        second.Should().Be("custom");
        customGlobals.Keys.Should().BeEquivalentTo("x");
        customGlobals["x"].Should().Be("custom");
    }

    [Theory]
    [InlineData(TransformerType.Scriban, "{{ request.Url }} {{ request.Path }} {{ request.Method }}")]
    [InlineData(TransformerType.Scriban, "{{ request.PathSegments[1] }} {{ request.Headers.Accept }}")]
    [InlineData(TransformerType.Scriban, "{{ for s in request.PathSegments }}[{{ s }}]{{ end }}")]
    [InlineData(TransformerType.Scriban, "{{ request.Body | string.upcase }} {{ data.Id }}")]
    [InlineData(TransformerType.Scriban, "{{ x = 5; x * 2 }}")]
    [InlineData(TransformerType.ScribanDotLiquid, "{{ request.Url }} {{ request.Path }} {{ request.Method }}")]
    [InlineData(TransformerType.ScribanDotLiquid, "{% for s in request.PathSegments %}[{{ s }}]{% endfor %}")]
    [InlineData(TransformerType.ScribanDotLiquid, "{{ request.Body | upcase }} {{ data.Id }}")]
    [InlineData(TransformerType.ScribanDotLiquid, "{% assign x = 5 %}{{ x | times: 2 }}")]
    public void ParseAndRender_WithEmptyCustomGlobals_RendersSameAsWithoutCustomGlobals(TransformerType transformerType, string template)
    {
        // Arrange
        var model = CreateModel();
        var withoutCustomGlobals = new ScribanContext(_fileSystemHandler, transformerType);
        var withCustomGlobals = new ScribanContext(_fileSystemHandler, transformerType, new ScriptObject());

        // Act
        var expected = withoutCustomGlobals.ParseAndRender(template, model);
        var result = withCustomGlobals.ParseAndRender(template, model);

        // Assert
        expected.Should().NotBeNullOrWhiteSpace();
        result.Should().Be(expected);
    }

    private static TransformModel CreateModel()
    {
        var body = new BodyData
        {
            BodyAsString = "hello",
            DetectedBodyType = BodyType.String
        };
        var headers = new Dictionary<string, string[]>
        {
            { "Accept", ["application/json"] }
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo/bar"), "GET", "::1", body, headers);

        return new TransformModel
        {
            mapping = Mock.Of<IMapping>(),
            request = request,
            data = new { Id = 42 }
        };
    }
}
