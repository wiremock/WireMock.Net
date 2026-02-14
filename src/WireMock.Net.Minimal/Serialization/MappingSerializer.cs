// Copyright © WireMock.Net

using System;
using JsonConverter.Abstractions;
using Newtonsoft.Json.Linq;
#if NETSTANDARD2_0_OR_GREATER || NETCOREAPP3_1_OR_GREATER || NET6_0_OR_GREATER || NET461
using System.Text.Json;
#endif

namespace WireMock.Serialization;

internal class MappingSerializer(IJsonConverter jsonConverter)
{
    internal T[] DeserializeJsonToArray<T>(string value)
    {
        return DeserializeObjectToArray<T>(jsonConverter.Deserialize<object>(value)!);
    }

    internal static T[] DeserializeObjectToArray<T>(object value)
    {
        if (value is JArray jArray)
        {
            return jArray.ToObject<T[]>()!;
        }

        if (value is JObject jObject)
        {
            var singleResult = jObject.ToObject<T>();
            return [singleResult!];
        }

#if NETSTANDARD2_0_OR_GREATER || NETCOREAPP3_1_OR_GREATER || NET6_0_OR_GREATER || NET461
        if (value is JsonElement jElement)
        {
            if (jElement.ValueKind == JsonValueKind.Array)
            {
                return jElement.Deserialize<T[]>()!;
            }

            if (jElement.ValueKind == JsonValueKind.Object)
            {
                var singleResult = jElement.Deserialize<T>();
                return [singleResult!];
            }
        }
#endif
        throw new InvalidOperationException("Cannot deserialize the provided value to an array or object.");
    }
}