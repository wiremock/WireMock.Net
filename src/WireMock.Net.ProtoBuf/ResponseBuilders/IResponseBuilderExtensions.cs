// Copyright © WireMock.Net

using System.Collections.Generic;
using JsonConverter.Abstractions;
using Stef.Validation;
using WireMock.Exceptions;
using WireMock.Models;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.ResponseBuilders;

/// <summary>
/// Extensions for <see cref="IResponseBuilder"/> to implement WithBodyAsProtoBuf.
/// </summary>
public static class IResponseBuilderExtensions
{
    /// <summary>
    /// WithBodyAsProtoBuf : Create a ProtoBuf byte[] response based on a proto definition, message type and the value.
    /// </summary>
    /// <param name="responseBuilder">The response builder.</param>
    /// <param name="protoDefinition">The proto definition as text.</param>
    /// <param name="messageType">The full type of the protobuf (request/response) message object. Format is "{package-name}.{type-name}".</param>
    /// <param name="value">The object to convert to protobuf byte[].</param>
    /// <param name="jsonConverter">The <see cref="IJsonConverter"/> [optional]. Default value is NewtonsoftJsonConverter.</param>
    /// <param name="options">The <see cref="JsonConverterOptions"/> [optional].</param>
    /// <returns>A <see cref="IResponseBuilder"/>.</returns>
    public static IResponseBuilder WithBodyAsProtoBuf(
        this IResponseBuilder responseBuilder,
        string protoDefinition,
        string messageType,
        object value,
        IJsonConverter? jsonConverter = null,
        JsonConverterOptions? options = null
    )
    {
        return responseBuilder.WithBodyAsProtoBuf([protoDefinition], messageType, value, jsonConverter, options);
    }

    /// <summary>
    /// WithBodyAsProtoBuf : Create a ProtoBuf byte[] response based on proto definitions, message type and the value.
    /// </summary>
    /// <param name="responseBuilder">The response builder.</param>
    /// <param name="protoDefinitions">The proto definition as text.</param>
    /// <param name="messageType">The full type of the protobuf (request/response) message object. Format is "{package-name}.{type-name}".</param>
    /// <param name="value">The object to convert to protobuf byte[].</param>
    /// <param name="jsonConverter">The <see cref="IJsonConverter"/> [optional]. Default value is NewtonsoftJsonConverter.</param>
    /// <param name="options">The <see cref="JsonConverterOptions"/> [optional].</param>
    /// <returns>A <see cref="IResponseBuilder"/>.</returns>
    public static IResponseBuilder WithBodyAsProtoBuf(
        this IResponseBuilder responseBuilder,
        IReadOnlyList<string> protoDefinitions,
        string messageType,
        object value,
        IJsonConverter? jsonConverter = null,
        JsonConverterOptions? options = null
    )
    {
        Guard.NotNullOrEmpty(protoDefinitions);
        Guard.NotNullOrWhiteSpace(messageType);
        Guard.NotNull(value);

        responseBuilder.ResponseMessage.BodyDestination = null;
        responseBuilder.ResponseMessage.BodyData = new BodyData
        {
            DetectedBodyType = BodyType.ProtoBuf,
            BodyAsJson = value,
            ProtoDefinition = () => new IdOrTexts(null, protoDefinitions),
            ProtoBufMessageType = messageType
        };

        return responseBuilder;
    }

    /// <summary>
    /// WithBodyAsProtoBuf : Create a ProtoBuf byte[] response based on a proto definition, message type and the value.
    /// </summary>
    /// <param name="responseBuilder">The response builder.</param>
    /// <param name="messageType">The full type of the protobuf (request/response) message object. Format is "{package-name}.{type-name}".</param>
    /// <param name="value">The object to convert to protobuf byte[].</param>
    /// <param name="jsonConverter">The <see cref="IJsonConverter"/> [optional]. Default value is NewtonsoftJsonConverter.</param>
    /// <param name="options">The <see cref="JsonConverterOptions"/> [optional].</param>
    /// <returns>A <see cref="IResponseBuilder"/>.</returns>
    public static IResponseBuilder WithBodyAsProtoBuf(
        this IResponseBuilder responseBuilder,
        string messageType,
        object value,
        IJsonConverter? jsonConverter = null,
        JsonConverterOptions? options = null
    )
    {
        Guard.NotNullOrWhiteSpace(messageType);
        Guard.NotNull(value);

        responseBuilder.ResponseMessage.BodyDestination = null;
        responseBuilder.ResponseMessage.BodyData = new BodyData
        {
            DetectedBodyType = BodyType.ProtoBuf,
            BodyAsJson = value,
            ProtoDefinition = () => responseBuilder.Mapping.ProtoDefinition ?? throw new WireMockException("ProtoDefinition cannot be resolved. You probably forgot to call .WithProtoDefinition(...) on the mapping."),
            ProtoBufMessageType = messageType
        };

        return responseBuilder;
    }
}