// Copyright © WireMock.Net

using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Stef.Validation;
using WireMock.Constants;
using WireMock.Types;

namespace WireMock.Http;

internal static class HttpRequestMessageHelper
{
    private static readonly IDictionary<string, bool> ContentLengthHeaderAllowed = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase)
    {
        { HttpRequestMethod.HEAD, true }
    };

    internal static HttpRequestMessage Create(IRequestMessage requestMessage, string url)
    {
        Guard.NotNull(requestMessage);
        Guard.NotNullOrEmpty(url);

        var httpRequestMessage = new HttpRequestMessage(new HttpMethod(requestMessage.Method), url);

        MediaTypeHeaderValue? contentType = null;
        if (requestMessage.Headers != null && requestMessage.Headers.ContainsKey(HttpKnownHeaderNames.ContentType))
        {
            var value = requestMessage.Headers[HttpKnownHeaderNames.ContentType].FirstOrDefault();
            MediaTypeHeaderValue.TryParse(value, out contentType);
        }

        var bodyData = requestMessage.BodyData;
        httpRequestMessage.Content = bodyData?.DetectedBodyType switch
        {
            BodyType.Bytes => ByteArrayContentHelper.Create(bodyData.BodyAsBytes!, contentType),
            BodyType.Json => StringContentHelper.Create(JsonConvert.SerializeObject(bodyData.BodyAsJson), contentType),
            BodyType.String => StringContentHelper.Create(bodyData.BodyAsString!, contentType),
            BodyType.FormUrlEncoded => StringContentHelper.Create(bodyData.BodyAsString!, contentType),
            BodyType.MultiPart => StringContentHelper.Create(bodyData.BodyAsString!, contentType),

            _ => httpRequestMessage.Content
        };

        // Overwrite the host header
        httpRequestMessage.Headers.Host = new Uri(url).Authority;

        // Set other headers if present
        if (requestMessage.Headers == null || requestMessage.Headers.Count == 0)
        {
            return httpRequestMessage;
        }

        var excludeHeaders = new List<string> { HttpKnownHeaderNames.Host };

        var contentLengthHeaderAllowed = ContentLengthHeaderAllowed.TryGetValue(requestMessage.Method, out var allowed) && allowed;
        if (contentLengthHeaderAllowed)
        {
            // Set Content to empty ByteArray to be able to set the Content-Length on the content in case of a HEAD method.
            httpRequestMessage.Content ??= new ByteArrayContent([]);
        }
        else
        {
            excludeHeaders.Add(HttpKnownHeaderNames.ContentLength);
        }

        if (contentType != null)
        {
            // Content-Type should be set on the content
            excludeHeaders.Add(HttpKnownHeaderNames.ContentType);
        }

        foreach (var header in requestMessage.Headers.Where(h => !excludeHeaders.Contains(h.Key, StringComparer.OrdinalIgnoreCase)))
        {
            // Skip if already added. We need to ToList() else calling httpRequestMessage.Headers.Contains() with a header starting with a ":" throws an exception.
            if (httpRequestMessage.Headers.ToList().Any(h => string.Equals(h.Key, header.Key, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            // Skip if already added. We need to ToList() else calling httpRequestMessage.Content.Headers.Contains(...) with a header starting with a ":" throws an exception.
            if (httpRequestMessage.Content != null && httpRequestMessage.Content.Headers.ToList().Any(h => string.Equals(h.Key, header.Key, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            // Try to add to request headers. If failed - try to add to content headers. If still fails, just ignore this header.
            try
            {
                if (!httpRequestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value))
                {
                    httpRequestMessage.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }
            catch
            {
                // Just continue
            }
        }

        return httpRequestMessage;
    }
}