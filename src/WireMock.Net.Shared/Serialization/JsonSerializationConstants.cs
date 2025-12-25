// Copyright © WireMock.Net

using JsonConverter.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace WireMock.Serialization;

internal static class JsonSerializationConstants
{
    internal static readonly JsonConverterOptions JsonConverterOptionsDefault = new()
    {
        WriteIndented = true,
        IgnoreNullValues = true
    };

    //internal static readonly JsonSerializerSettings JsonSerializerSettingsDefault = new()
    //{
    //    Formatting = Formatting.Indented,
    //    NullValueHandling = NullValueHandling.Ignore
    //};

    internal static readonly JsonSerializerSettings JsonSerializerSettingsIncludeNullValues = new()
    {
        Formatting = Formatting.Indented,
        NullValueHandling = NullValueHandling.Include
    };

    internal static readonly JsonSerializerSettings JsonDeserializerSettingsWithDateParsingNone = new()
    {
        DateParseHandling = DateParseHandling.None
    };

    internal static readonly JsonSerializerSettings JsonSerializerSettingsPact = new()
    {
        Formatting = Formatting.Indented,
        NullValueHandling = NullValueHandling.Ignore,
        ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        }
    };
}