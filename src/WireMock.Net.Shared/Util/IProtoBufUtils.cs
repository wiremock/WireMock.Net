// Copyright © WireMock.Net

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JsonConverter.Abstractions;
using WireMock.ResponseBuilders;

namespace WireMock.Util;

/// <summary>
/// Defines the interface for ProtoBufUtils.
/// </summary>
public interface IProtoBufUtils
{
    Task<byte[]> GetProtoBufMessageWithHeaderAsync(IReadOnlyList<string>? protoDefinitions, string? messageType, object? value, IJsonConverter? jsonConverter = null, CancellationToken cancellationToken = default);

    IResponseBuilder UpdateResponseBuilder(IResponseBuilder responseBuilder, string protoBufMessageType, object bodyAsJson, params string[] protoDefinitions);
}