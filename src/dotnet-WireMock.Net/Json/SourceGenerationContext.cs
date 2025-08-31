// Copyright © WireMock.Net

using System.Text.Json.Serialization;
using WireMock.Admin.Mappings;
using WireMock.Admin.Requests;
using WireMock.Types;

namespace WireMock.Net.Json;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(EncodingModel))]
[JsonSerializable(typeof(LogEntryModel))]
[JsonSerializable(typeof(LogRequestModel))]
[JsonSerializable(typeof(LogResponseModel))]
[JsonSerializable(typeof(LogRequestMatchModel))]
[JsonSerializable(typeof(WireMockList<string>))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
}