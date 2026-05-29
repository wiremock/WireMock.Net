// Copyright © WireMock.Net

using System.Collections;
using HandlebarsDotNet.Helpers.Models;
using JetBrains.Annotations;
using JsonConverter.Newtonsoft.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WireMock.Settings;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.Transformers;

/// <summary>
/// Default JSON body transformer implementation based on Newtonsoft.Json.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="NewtonsoftJsonBodyTransformer"/> class.
/// </remarks>
/// <param name="settings">The server settings used to configure JSON transformation behavior.</param>
[PublicAPI]
public class NewtonsoftJsonBodyTransformer(WireMockServerSettings settings) : IJsonBodyTransformer
{
    private static readonly NewtonsoftJsonConverter _jsonConverter = new();

    /// <inheritdoc />
    public BodyData TransformBodyAsJson(
        ITransformerContext transformerContext,
        ReplaceNodeOptions options,
        object model,
        IBodyData original)
    {
        var jsonSerializer = new JsonSerializer
        {
            Culture = settings.Culture
        };

        JToken? jToken = null;
        switch (original.BodyAsJson)
        {
            case JObject bodyAsJObject:
                jToken = bodyAsJObject.DeepClone();
                WalkNode(transformerContext, jsonSerializer, options, jToken, model);
                break;

            case JArray bodyAsJArray:
                jToken = bodyAsJArray.DeepClone();
                WalkNode(transformerContext, jsonSerializer, options, jToken, model);
                break;

            case var bodyAsEnumerable when bodyAsEnumerable is IEnumerable and not string:
                jToken = JArray.FromObject(bodyAsEnumerable, jsonSerializer);
                WalkNode(transformerContext, jsonSerializer, options, jToken, model);
                break;

            case string bodyAsString:
                jToken = ReplaceSingleNode(transformerContext, jsonSerializer, options, bodyAsString, model);
                break;

            case not null:
                jToken = JObject.FromObject(original.BodyAsJson, jsonSerializer);
                WalkNode(transformerContext, jsonSerializer, options, jToken, model);
                break;
        }

        return new BodyData
        {
            Encoding = original.Encoding,
            DetectedBodyType = original.DetectedBodyType,
            DetectedBodyTypeFromContentType = original.DetectedBodyTypeFromContentType,
            ProtoDefinition = original.ProtoDefinition,
            ProtoBufMessageType = original.ProtoBufMessageType,
            BodyAsJson = jToken
        };
    }

    private JToken ParseAsJObject(string stringValue)
    {
        if (_jsonConverter.IsValidJson(stringValue))
        {
            try
            {
                // Try to convert this string into a JObject
                return JObject.Parse(stringValue!);
            }
            catch
            {
                settings.Logger.Warn("Failed to parse string ''{0}'' as JSON. Returning the original string value.", stringValue);
            }
        }

        return stringValue;
    }

    private JToken ReplaceSingleNode(ITransformerContext transformerContext, JsonSerializer jsonSerializer, ReplaceNodeOptions options, string stringValue, object model)
    {
        var transformedString = transformerContext.ParseAndRender(stringValue, model);

        if (!string.Equals(stringValue, transformedString))
        {
            const string property = "_";
            JObject dummy = JObject.Parse($"{{ \"{property}\": null }}");
            if (dummy[property] == null)
            {
                return string.Empty;
            }

            JToken node = dummy[property]!;

            ReplaceNodeValue(jsonSerializer, options, node, transformedString);

            return dummy[property]!;
        }

        return stringValue;
    }

    private void WalkNode(ITransformerContext transformerContext, JsonSerializer jsonSerializer, ReplaceNodeOptions options, JToken node, object model)
    {
        switch (node.Type)
        {
            case JTokenType.Object:
                foreach (var child in node.Children<JProperty>().ToArray())
                {
                    WalkNode(transformerContext, jsonSerializer, options, child.Value, model);
                }
                break;

            case JTokenType.Array:
                foreach (var child in node.Children().ToArray())
                {
                    WalkNode(transformerContext, jsonSerializer, options, child, model);
                }
                break;

            case JTokenType.String:
                var stringValue = node.Value<string>();
                if (string.IsNullOrEmpty(stringValue))
                {
                    return;
                }

                var transformed = transformerContext.ParseAndEvaluate(stringValue!, model);
                if (!Equals(stringValue, transformed))
                {
                    ReplaceNodeValue(jsonSerializer, options, node, transformed);
                }
                break;
        }
    }

    private void ReplaceNodeValue(JsonSerializer jsonSerializer, ReplaceNodeOptions options, JToken node, object? transformedValue)
    {
        switch (transformedValue)
        {
            case JValue jValue:
                node.Replace(jValue);
                return;

            case string transformedString:
                var (isConvertedFromString, convertedValueFromString) = TryConvert(options, transformedString);
                if (isConvertedFromString)
                {
                    node.Replace(JToken.FromObject(convertedValueFromString, jsonSerializer));
                }
                else
                {
                    node.Replace(ParseAsJObject(transformedString));
                }
                break;

            case WireMockList<string> strings:
                switch (strings.Count)
                {
                    case 1:
                        node.Replace(ParseAsJObject(strings[0]));
                        return;

                    case > 1:
                        node.Replace(JToken.FromObject(strings.ToArray(), jsonSerializer));
                        return;
                }
                break;

            case { }:
                var (isConverted, convertedValue) = TryConvert(options, transformedValue);
                if (isConverted)
                {
                    node.Replace(JToken.FromObject(convertedValue, jsonSerializer));
                }
                return;

            default:
                return;
        }
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
                : TryConvertToKnownType(valueAsString);
        }

        return (false, value);
    }

    internal static (bool IsConverted, object ConvertedValue) TryConvertToKnownType(string value)
    {
        if (bool.TryParse(value, out var boolResult))
        {
            return (true, boolResult);
        }

        if (int.TryParse(value, out var intResult))
        {
            return (true, intResult);
        }

        if (long.TryParse(value, out var longResult))
        {
            return (true, longResult);
        }

        if (double.TryParse(value, out var doubleResult))
        {
            return (true, doubleResult);
        }

        if (Guid.TryParseExact(value, "D", out var guidResult))
        {
            return (true, guidResult);
        }

        if (TimeSpan.TryParse(value, out var timeSpanResult))
        {
            return (true, timeSpanResult);
        }

        if (DateTime.TryParse(value, out var dateTimeResult))
        {
            return (true, dateTimeResult);
        }

        if ((value.StartsWith("ftp://", StringComparison.OrdinalIgnoreCase) ||
            value.StartsWith("ftps://", StringComparison.OrdinalIgnoreCase) ||
            value.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            value.StartsWith("https://", StringComparison.OrdinalIgnoreCase)) &&
            Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out var uriResult))
        {
            return (true, uriResult);
        }

        return (false, value);
    }
}
