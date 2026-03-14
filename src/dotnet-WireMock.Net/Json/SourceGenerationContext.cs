// Copyright © WireMock.Net

using System.Text.Json.Serialization;
using Newtonsoft.Json.Linq;
using WireMock.Admin.Mappings;
using WireMock.Admin.Requests;
using WireMock.Types;

namespace WireMock.Net.Json;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(EncodingModel))]
[JsonSerializable(typeof(JArray))]
[JsonSerializable(typeof(JObject))]
[JsonSerializable(typeof(LogEntryModel))]
[JsonSerializable(typeof(LogRequestModel))]
[JsonSerializable(typeof(LogResponseModel))]
[JsonSerializable(typeof(LogRequestMatchModel))]
[JsonSerializable(typeof(StatusModel))]
[JsonSerializable(typeof(WireMockList<string>))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
}