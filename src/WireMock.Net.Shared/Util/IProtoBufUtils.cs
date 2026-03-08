// Copyright © WireMock.Net

using JsonConverter.Abstractions;
using WireMock.ResponseBuilders;

namespace WireMock.Util;

/// <summary>
/// Defines the interface for ProtoBufUtils.
/// </summary>
public interface IProtoBufUtils
{
    /// <summary>
    /// Converts a JSON-like object to a ProtoBuf message including the length header.
    /// </summary>
    /// <param name="protoDefinitions">The Proto definition content used to resolve message types.</param>
    /// <param name="messageType">The fully qualified ProtoBuf message type name to serialize to.</param>
    /// <param name="value">The source object to convert.</param>
    /// <param name="jsonConverter">Optional JSON converter used during serialization.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The serialized ProtoBuf payload with header, or an empty byte array when input is invalid.</returns>
    Task<byte[]> GetProtoBufMessageWithHeaderAsync(IReadOnlyList<string>? protoDefinitions, string? messageType, object? value, IJsonConverter? jsonConverter = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the response builder to return a ProtoBuf body using method-level or mapping-level proto definitions.
    /// </summary>
    /// <param name="responseBuilder">The response builder to update.</param>
    /// <param name="protoBufMessageType">The ProtoBuf message type for the response body.</param>
    /// <param name="bodyAsJson">The response body object represented as JSON-like data.</param>
    /// <param name="protoDefinitions">Optional Proto definitions for this call; when omitted, mapping/server-level definitions are used.</param>
    /// <returns>The updated response builder.</returns>
    IResponseBuilder UpdateResponseBuilder(IResponseBuilder responseBuilder, string protoBufMessageType, object bodyAsJson, params string[] protoDefinitions);
}