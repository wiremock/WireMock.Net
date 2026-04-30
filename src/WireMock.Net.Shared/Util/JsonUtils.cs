// Copyright © WireMock.Net

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
    /// Deserializes the JSON to a .NET object.
    /// Using : DateParseHandling = DateParseHandling.None
    /// </summary>
    /// <param name="json">A System.String that contains JSON.</param>
    /// <returns>The deserialized object from the JSON string.</returns>
    public static object DeserializeObject(string json)
    {
        return JsonConvert.DeserializeObject(json, JsonSerializationConstants.JsonDeserializerSettingsWithDateParsingNone)!;
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
}