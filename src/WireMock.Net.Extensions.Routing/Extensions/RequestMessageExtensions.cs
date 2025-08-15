// Copyright © WireMock.Net

using JsonConverter.Abstractions;

namespace WireMock.Net.Extensions.Routing.Extensions;

internal static class RequestMessageExtensions
{
    public static T? GetBodyAsJson<T>(
        this IRequestMessage requestMessage,
        IJsonConverter jsonConverter,
        JsonConverterOptions? jsonOptions = null) =>
        requestMessage.Body is not null
            ? jsonConverter.Deserialize<T>(requestMessage.Body, jsonOptions)
            : default;
}
