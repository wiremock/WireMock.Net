// Copyright © WireMock.Net

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WireMock.Serialization;

namespace WireMock.Util;

internal static class JsonUtils
{
    public static bool IsJson(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        value = value!.Trim();

        return (value.StartsWith("{") && value.EndsWith("}")) || (value.StartsWith("[") && value.EndsWith("]"));
    }

    public static bool TryParseAsJObject(string? strInput, [NotNullWhen(true)] out JObject? value)
    {
        value = null;

        if (!IsJson(strInput))
        {
            return false;
        }

        try
        {
            // Try to convert this string into a JObject
            value = JObject.Parse(strInput!);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static byte[] SerializeAsPactFile(object value)
    {
        var json = JsonConvert.SerializeObject(value, JsonSerializationConstants.JsonSerializerSettingsPact);
        return Encoding.UTF8.GetBytes(json);
    }

    /// <summary>
    /// Load a Newtonsoft.Json.Linq.JObject from a string that contains JSON.
    /// Using : DateParseHandling = DateParseHandling.None
    /// </summary>
    /// <param name="json">A System.String that contains JSON.</param>
    /// <returns>A Newtonsoft.Json.Linq.JToken populated from the string that contains JSON.</returns>
    public static JToken Parse(string json)
    {
        return JsonConvert.DeserializeObject<JToken>(json, JsonSerializationConstants.JsonDeserializerSettingsWithDateParsingNone)!;
    }

    /// <summary>
    /// Deserializes the JSON to a .NET object.
    /// Using : DateParseHandling = DateParseHandling.None
    /// </summary>
    /// <param name="json">A System.String that contains JSON.</param>
    /// <returns>The deserialized object from the JSON string.</returns>
    public static object DeserializeObject(string json)
    {
        return JsonConvert.DeserializeObject(json, JsonSerializationConstants.JsonDeserializerSettingsWithDateParsingNone)!;
    }

    /// <summary>
    /// Deserializes the JSON to the specified .NET type.
    /// Using : DateParseHandling = DateParseHandling.None
    /// </summary>
    /// <param name="json">A System.String that contains JSON.</param>
    /// <returns>The deserialized object from the JSON string.</returns>
    public static T DeserializeObject<T>(string json)
    {
        return JsonConvert.DeserializeObject<T>(json, JsonSerializationConstants.JsonDeserializerSettingsWithDateParsingNone)!;
    }

    public static T? TryDeserializeObject<T>(string json)
    {
        try
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
        catch
        {
            return default;
        }
    }

    public static T ParseJTokenToObject<T>(object? value)
    {
        if (value != null && value.GetType() == typeof(T))
        {
            return (T)value;
        }

        return value switch
        {
            JToken tokenValue => tokenValue.ToObject<T>()!,

            _ => throw new NotSupportedException($"Unable to convert value to {typeof(T)}.")
        };
    }

    public static JToken ConvertValueToJToken(object value)
    {
        // Check if JToken, string, IEnumerable or object
        switch (value)
        {
            case JToken tokenValue:
                return tokenValue;

            case string stringValue:
                return Parse(stringValue);

            case IEnumerable enumerableValue:
                return JArray.FromObject(enumerableValue);

            default:
                return JObject.FromObject(value);
        }
    }
}