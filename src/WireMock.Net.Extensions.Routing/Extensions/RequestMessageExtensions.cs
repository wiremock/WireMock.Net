using Newtonsoft.Json;

namespace WireMock.Net.Extensions.Routing.Extensions;

internal static class RequestMessageExtensions
{
    public static T? GetBodyAsJson<T>(
        this IRequestMessage requestMessage, JsonSerializerSettings? settings = null) =>
        requestMessage.Body is not null
            ? JsonConvert.DeserializeObject<T>(requestMessage.Body, settings)
            : default;
}
