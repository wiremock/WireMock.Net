// Copyright © WireMock.Net

using JsonConverter.Abstractions;
using JsonConverter.Abstractions.Models;

namespace WireMock.Serialization;

internal class MappingSerializer(IJsonConverter jsonConverter)
{
    internal T[] DeserializeJsonToArray<T>(string value)
    {
        switch (JsonTypeHelper.GetJsonType(value))
        {
            case JsonType.Array:
                return jsonConverter.Deserialize<T[]>(value, JsonSerializationConstants.JsonConverterOptionsWithDateParsingNone)!;

            case JsonType.Object:
                var singleResult = jsonConverter.Deserialize<T>(value, JsonSerializationConstants.JsonConverterOptionsWithDateParsingNone);
                return [singleResult!];

            default:
                throw new InvalidOperationException("Cannot deserialize the provided value to an array or object.");
        }
    }

    internal T[] DeserializeObjectToArray<T>(object value)
    {
        var json = jsonConverter.Serialize(value, JsonSerializationConstants.JsonConverterOptionsWithDateParsingNone);
        return DeserializeJsonToArray<T>(json);
    }
}