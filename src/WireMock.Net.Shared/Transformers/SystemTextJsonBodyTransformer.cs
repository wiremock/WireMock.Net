// Copyright © WireMock.Net

using System.Collections;
using System.Text.Json;
using System.Text.Json.Nodes;
using HandlebarsDotNet.Helpers.Models;
using JetBrains.Annotations;
using JsonConverter.System.Text.Json;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.Transformers;

/// <summary>
/// JSON body transformer implementation based on System.Text.Json.
/// </summary>
[PublicAPI]
public class SystemTextJsonBodyTransformer() : IJsonBodyTransformer
{
    private readonly SystemTextJsonConverter _jsonConverter = new();

    /// <inheritdoc />
    public BodyData TransformBodyAsJson(
        ITransformerContext transformerContext,
        ReplaceNodeOptions options,
        object model,
        IBodyData original)
    {
        JsonNode? jsonNode = null;
        switch (original.BodyAsJson)
        {
            case JsonObject bodyAsJsonObject:
                jsonNode = CloneNode(bodyAsJsonObject);
                jsonNode = WalkNode(transformerContext, options, jsonNode, model);
                break;

            case JsonArray bodyAsJsonArray:
                jsonNode = CloneNode(bodyAsJsonArray);
                jsonNode = WalkNode(transformerContext, options, jsonNode, model);
                break;

            case var bodyAsEnumerable when bodyAsEnumerable is IEnumerable and not string:
                jsonNode = JsonSerializer.SerializeToNode(bodyAsEnumerable);
                if (jsonNode != null)
                {
                    jsonNode = WalkNode(transformerContext, options, jsonNode, model);
                }
                break;

            case string bodyAsString:
                jsonNode = ReplaceSingleNode(transformerContext, options, bodyAsString, model);
                break;

            case not null:
                jsonNode = JsonSerializer.SerializeToNode(original.BodyAsJson);
                if (jsonNode != null)
                {
                    jsonNode = WalkNode(transformerContext, options, jsonNode, model);
                }
                break;
        }

        return new BodyData
        {
            Encoding = original.Encoding,
            DetectedBodyType = original.DetectedBodyType,
            DetectedBodyTypeFromContentType = original.DetectedBodyTypeFromContentType,
            ProtoDefinition = original.ProtoDefinition,
            ProtoBufMessageType = original.ProtoBufMessageType,
            BodyAsJson = jsonNode
        };
    }

    private JsonNode ParseAsJsonObject(string stringValue)
    {
        if (_jsonConverter.IsValidJson(stringValue))
        {
            try
            {
                var parsed = JsonNode.Parse(stringValue);
                if (parsed is JsonObject)
                {
                    return parsed;
                }
            }
            catch
            {
                // Ignore and return as string.
            }
        }

        return JsonValue.Create(stringValue)!;
    }

    private JsonNode? ReplaceSingleNode(ITransformerContext transformerContext, ReplaceNodeOptions options, string stringValue, object model)
    {
        var transformedString = transformerContext.ParseAndRender(stringValue, model);

        if (!string.Equals(stringValue, transformedString))
        {
            return ReplaceNodeValue(options, transformedString);
        }

        return JsonValue.Create(stringValue);
    }

    private JsonNode? WalkNode(ITransformerContext transformerContext, ReplaceNodeOptions options, JsonNode? node, object model)
    {
        switch (node)
        {
            case JsonObject jsonObject:
                foreach (var property in jsonObject.ToArray())
                {
                    jsonObject[property.Key] = WalkNode(transformerContext, options, property.Value, model);
                }
                return jsonObject;

            case JsonArray jsonArray:
                for (var i = 0; i < jsonArray.Count; i++)
                {
                    jsonArray[i] = WalkNode(transformerContext, options, jsonArray[i], model);
                }
                return jsonArray;

            case JsonValue jsonValue when jsonValue.TryGetValue<string>(out var stringValue):
                if (string.IsNullOrEmpty(stringValue))
                {
                    return jsonValue;
                }

                var transformed = transformerContext.ParseAndEvaluate(stringValue!, model);
                return !Equals(stringValue, transformed) ? ReplaceNodeValue(options, transformed) ?? jsonValue : jsonValue;

            default:
                return node;
        }
    }

    private JsonNode? ReplaceNodeValue(ReplaceNodeOptions options, object? transformedValue)
    {
        switch (transformedValue)
        {
            case JsonNode jsonNode:
                return CloneNode(jsonNode);

            case string transformedString:
                var (isConvertedFromString, convertedValueFromString) = TryConvert(options, transformedString);
                return isConvertedFromString
                    ? JsonSerializer.SerializeToNode(convertedValueFromString)
                    : ParseAsJsonObject(transformedString);

            case WireMockList<string> strings:
                switch (strings.Count)
                {
                    case 1:
                        return ParseAsJsonObject(strings[0]);

                    case > 1:
                        return JsonSerializer.SerializeToNode(strings.ToArray());
                }
                break;

            case { }:
                var (isConverted, convertedValue) = TryConvert(options, transformedValue);
                if (isConverted)
                {
                    return JsonSerializer.SerializeToNode(convertedValue);
                }
                break;
        }

        return null;
    }

    private static JsonNode? CloneNode(JsonNode? node)
    {
#if NET8_0_OR_GREATER
        return node?.DeepClone();
#else
        return node == null ? null : JsonNode.Parse(node.ToJsonString());
#endif
    }

    private static (bool IsConverted, object ConvertedValue) TryConvert(ReplaceNodeOptions options, object value)
    {
        var valueAsString = value as string;

        if (options == ReplaceNodeOptions.Evaluate)
        {
            if (valueAsString != null && WrappedString.TryDecode(valueAsString, out var decoded))
            {
                return (true, decoded);
            }

            return (false, value);
        }

        if (valueAsString != null)
        {
            return WrappedString.TryDecode(valueAsString, out var decoded)
                ? (true, decoded)
                : NewtonsoftJsonBodyTransformer.TryConvertToKnownType(valueAsString);
        }

        return (false, value);
    }
}