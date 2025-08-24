// Copyright © WireMock.Net

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JsonConverter.Abstractions;

namespace WireMock.Util;

/// <summary>
/// Defines the interface for ProtoBufUtils.
/// </summary>
public interface IProtoBufUtils
{
    Task<byte[]> GetProtoBufMessageWithHeaderAsync(IReadOnlyList<string>? protoDefinitions, string? messageType, object? value, IJsonConverter? jsonConverter = null, CancellationToken cancellationToken = default);
}