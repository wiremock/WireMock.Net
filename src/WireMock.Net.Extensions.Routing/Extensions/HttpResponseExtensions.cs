using Microsoft.AspNetCore.Http;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.Net.Extensions.Routing.Extensions;

internal static class HttpResponseExtensions
{
    public static async Task<ResponseMessage> ToResponseMessageAsync(
        this HttpResponse response)
    {
        var headers = response.Headers.ToDictionary(
            header => header.Key, header => new WireMockList<string?>(header.Value.ToArray()));
        return new()
        {
            Headers = headers!,
            BodyData = new BodyData
            {
                DetectedBodyType = BodyType.String,
                BodyAsString = await response.ReadBodyAsStringAsync(),
            },
            StatusCode = response.StatusCode,
        };
    }

    public static async Task<string> ReadBodyAsStringAsync(this HttpResponse response)
    {
        response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(response.Body);
        return await reader.ReadToEndAsync();
    }
}
