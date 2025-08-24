// Copyright © WireMock.Net

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JsonConverter.Abstractions;
using ProtoBufJsonConverter;
using ProtoBufJsonConverter.Models;
using WireMock.ResponseBuilders;

namespace WireMock.Util;

internal class ProtoBufUtils : IProtoBufUtils
{
    public async Task<byte[]> GetProtoBufMessageWithHeaderAsync(
        IReadOnlyList<string>? protoDefinitions,
        string? messageType,
        object? value,
        IJsonConverter? jsonConverter = null,
        CancellationToken cancellationToken = default
    )
    {
        if (protoDefinitions == null || string.IsNullOrWhiteSpace(messageType) || value is null)
        {
            return [];
        }

        var resolver = new WireMockProtoFileResolver(protoDefinitions);
        var request = new ConvertToProtoBufRequest(protoDefinitions[0], messageType!, value, true)
            .WithProtoFileResolver(resolver);

        return await SingletonFactory<Converter>
            .GetInstance()
            .ConvertAsync(request, cancellationToken).ConfigureAwait(false);
    }

    public IResponseBuilder UpdateResponseBuilder(IResponseBuilder responseBuilder, string protoBufMessageType, object bodyAsJson, params string[] protoDefinitions)
    {
        if (protoDefinitions.Length > 0)
        {
            return responseBuilder.WithBodyAsProtoBuf(protoDefinitions, protoBufMessageType, bodyAsJson);
        }

        // ProtoDefinition(s) is/are defined at Mapping/Server level
        return responseBuilder.WithBodyAsProtoBuf(protoBufMessageType, bodyAsJson);
    }
}