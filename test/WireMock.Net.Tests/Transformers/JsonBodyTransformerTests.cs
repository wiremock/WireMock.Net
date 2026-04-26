// Copyright © WireMock.Net

using System.Text;
using System.Text.Json.Nodes;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WireMock.Handlers;
using WireMock.Settings;
using WireMock.Transformers;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.Net.Tests.Transformers;

public class JsonBodyTransformerTests
{
    public static TheoryData<JsonBodyTransformerTestContext> Transformers
    {
        get
        {
            return
            [
                new JsonBodyTransformerTestContext(
                    () => new NewtonsoftJsonBodyTransformer(new WireMockServerSettings()),
                    JObject.Parse,
                    body => ((JToken)body).ToString(Formatting.None)),

                new JsonBodyTransformerTestContext(
                    () => new SystemTextJsonBodyTransformer(),
                    json => JsonNode.Parse(json)!,
                    body => ((JsonNode)body).ToJsonString())
            ];
        }
    }

    [Theory]
    [MemberData(nameof(Transformers))]
    public void TransformBodyAsJson_Replaces_String_Value_And_Preserves_Original(JsonBodyTransformerTestContext testContext)
    {
        // Arrange
        var transformer = testContext.CreateTransformer();
        var originalJson = testContext.ParseJson("{\"value\":\"{{number}}\"}");
        var bodyData = new BodyData
        {
            Encoding = Encoding.UTF8,
            DetectedBodyType = BodyType.Json,
            DetectedBodyTypeFromContentType = BodyType.Json,
            ProtoBufMessageType = "My.Message",
            BodyAsJson = originalJson
        };

        var transformerContext = new FakeTransformerContext(
            text => text,
            text => text == "{{number}}" ? "123" : text);

        // Act
        var result = transformer.TransformBodyAsJson(transformerContext, ReplaceNodeOptions.EvaluateAndTryToConvert, new { }, bodyData);

        // Assert
        result.Encoding.Should().Be(Encoding.UTF8);
        result.DetectedBodyType.Should().Be(BodyType.Json);
        result.DetectedBodyTypeFromContentType.Should().Be(BodyType.Json);
        result.ProtoBufMessageType.Should().Be("My.Message");
        result.BodyAsJson.Should().NotBeNull();
        testContext.SerializeJson(result.BodyAsJson).Should().Be("{\"value\":123}");
        testContext.SerializeJson(originalJson).Should().Be("{\"value\":\"{{number}}\"}");
    }

    [Theory]
    [MemberData(nameof(Transformers))]
    public void TransformBodyAsJson_With_String_Body_Replaces_Single_Node_With_Object(JsonBodyTransformerTestContext testContext)
    {
        // Arrange
        var transformer = testContext.CreateTransformer();
        var bodyData = new BodyData
        {
            DetectedBodyType = BodyType.Json,
            BodyAsJson = "{{json}}"
        };

        var transformerContext = new FakeTransformerContext(
            text => text == "{{json}}" ? "{\"name\":\"test\"}" : text,
            text => text);

        // Act
        var result = transformer.TransformBodyAsJson(transformerContext, ReplaceNodeOptions.EvaluateAndTryToConvert, new { }, bodyData);

        // Assert
        result.BodyAsJson.Should().NotBeNull();
        testContext.SerializeJson(result.BodyAsJson).Should().Be("{\"name\":\"test\"}");
    }

    [Theory]
    [MemberData(nameof(Transformers))]
    public void TransformBodyAsJson_Replaces_String_Value_With_WireMockList_As_Array(JsonBodyTransformerTestContext testContext)
    {
        // Arrange
        var transformer = testContext.CreateTransformer();
        var bodyData = new BodyData
        {
            DetectedBodyType = BodyType.Json,
            BodyAsJson = testContext.ParseJson("{\"values\":\"{{list}}\"}")
        };

        var transformerContext = new FakeTransformerContext(
            text => text,
            text => text == "{{list}}" ? new WireMockList<string>(["a", "b"]) : text);

        // Act
        var result = transformer.TransformBodyAsJson(transformerContext, ReplaceNodeOptions.EvaluateAndTryToConvert, new { }, bodyData);

        // Assert
        result.BodyAsJson.Should().NotBeNull();
        testContext.SerializeJson(result.BodyAsJson).Should().Be("{\"values\":[\"a\",\"b\"]}");
    }

    public sealed class JsonBodyTransformerTestContext
    {
        private readonly Func<IJsonBodyTransformer> _createTransformer;
        private readonly Func<string, object> _parseJson;
        private readonly Func<object, string> _serializeJson;

        public JsonBodyTransformerTestContext(
            Func<IJsonBodyTransformer> createTransformer,
            Func<string, object> parseJson,
            Func<object, string> serializeJson)
        {
            _createTransformer = createTransformer;
            _parseJson = parseJson;
            _serializeJson = serializeJson;
        }

        public IJsonBodyTransformer CreateTransformer()
        {
            return _createTransformer();
        }

        public object ParseJson(string json)
        {
            return _parseJson(json);
        }

        public string SerializeJson(object body)
        {
            return _serializeJson(body);
        }
    }

    private sealed class FakeTransformerContext(Func<string, string> parseAndRender, Func<string, object> parseAndEvaluate) : ITransformerContext
    {
        public IFileSystemHandler FileSystemHandler { get; private set; } = Mock.Of<IFileSystemHandler>();

        public string ParseAndRender(string text, object model)
        {
            return parseAndRender(text);
        }

        public object ParseAndEvaluate(string text, object model)
        {
            return parseAndEvaluate(text);
        }
    }
}