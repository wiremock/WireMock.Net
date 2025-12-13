// Copyright © WireMock.Net

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Stef.Validation;
using WireMock.Constants;
using WireMock.Matchers;
using WireMock.Types;

namespace WireMock.Util;

internal static class BodyParser
{
    private static readonly Encoding DefaultEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);
    private static readonly Encoding[] SupportedBodyAsStringEncodingForMultipart = [DefaultEncoding, Encoding.ASCII];

    /*
        HEAD - No defined body semantics.
        GET - No defined body semantics.
        PUT - Body supported.
        POST - Body supported.
        DELETE - No defined body semantics.
        TRACE - Body not supported.
        OPTIONS - Body supported but no semantics on usage (maybe in the future).
        CONNECT - No defined body semantics
        PATCH - Body supported.
    */
    private static readonly IDictionary<string, bool> BodyAllowedForMethods = new Dictionary<string, bool>
    {
        { HttpRequestMethod.HEAD, false },
        { HttpRequestMethod.GET, false },
        { HttpRequestMethod.PUT, true },
        { HttpRequestMethod.POST, true },
        { HttpRequestMethod.DELETE, true },
        { HttpRequestMethod.TRACE, false },
        { HttpRequestMethod.OPTIONS, true },
        { HttpRequestMethod.CONNECT, false },
        { HttpRequestMethod.PATCH, true }
    };

    private static readonly IStringMatcher[] MultipartContentTypesMatchers =
    [
        new WildcardMatcher("multipart/*", true)
    ];

    private static readonly IStringMatcher[] JsonContentTypesMatchers =
    [
        new WildcardMatcher("application/json", true),
        new WildcardMatcher("application/vnd.*+json", true)
    ];

    private static readonly IStringMatcher FormUrlEncodedMatcher = new WildcardMatcher("application/x-www-form-urlencoded", true);

    private static readonly IStringMatcher[] TextContentTypeMatchers =
    [
        new WildcardMatcher("text/*", true),
        new RegexMatcher("^application\\/(java|type)script$", true),
        new WildcardMatcher("application/*xml", true),
        FormUrlEncodedMatcher
    ];

    private static readonly IStringMatcher[] GrpcContentTypesMatchers =
    [
        new WildcardMatcher("application/grpc", true),
        new WildcardMatcher("application/grpc+proto", true)
    ];

    public static bool ShouldParseBody(string? httpMethod, bool allowBodyForAllHttpMethods)
    {
        if (string.IsNullOrEmpty(httpMethod))
        {
            return false;
        }

        if (allowBodyForAllHttpMethods)
        {
            return true;
        }

        if (BodyAllowedForMethods.TryGetValue(httpMethod!.ToUpper(), out var allowed))
        {
            return allowed;
        }

        // If we don't have any knowledge of this method, we should assume that a body *may*
        // be present, so we should parse it if it is. Therefore, if a new method is added to
        // the HTTP Method Registry, we only really need to add it to BodyAllowedForMethods if
        // we want to make it clear that a body is *not* allowed.
        return true;
    }

    public static BodyType DetectBodyTypeFromContentType(string? contentTypeValue)
    {
        if (string.IsNullOrEmpty(contentTypeValue) || !MediaTypeHeaderValue.TryParse(contentTypeValue, out var contentType))
        {
            return BodyType.Bytes;
        }

        if (FormUrlEncodedMatcher.IsMatch(contentType.MediaType).IsPerfect())
        {
            return BodyType.FormUrlEncoded;
        }

        if (TextContentTypeMatchers.Any(matcher => matcher.IsMatch(contentType.MediaType).IsPerfect()))
        {
            return BodyType.String;
        }

        if (JsonContentTypesMatchers.Any(matcher => matcher.IsMatch(contentType.MediaType).IsPerfect()))
        {
            return BodyType.Json;
        }

        if (GrpcContentTypesMatchers.Any(matcher => matcher.IsMatch(contentType.MediaType).IsPerfect()))
        {
            return BodyType.ProtoBuf;
        }

        if (MultipartContentTypesMatchers.Any(matcher => matcher.IsMatch(contentType.MediaType).IsPerfect()))
        {
            return BodyType.MultiPart;
        }

        return BodyType.Bytes;
    }

    public static async Task<BodyData> ParseAsync(BodyParserSettings settings)
    {
        Guard.NotNull(settings);

        bool decompressGZipAndDeflate = settings.BodyHandling.HasFlag(BodyHandling.DecompressGZipAndDeflate);

        var bodyWithContentEncoding = await ReadBytesAsync(settings.Stream, settings.ContentEncoding, decompressGZipAndDeflate).ConfigureAwait(false);
        var data = new BodyData
        {
            BodyAsBytes = bodyWithContentEncoding.Bytes,
            DetectedCompression = bodyWithContentEncoding.ContentType,
            DetectedBodyType = BodyType.Bytes,
            DetectedBodyTypeFromContentType = DetectBodyTypeFromContentType(settings.ContentType)
        };

        if (settings.BodyHandling == BodyHandling.None)
        {
            return data;
        }

        // In case of MultiPart: check if the BodyAsBytes is a valid UTF8 or ASCII string, in that case read as String else keep as-is
        if (data.DetectedBodyTypeFromContentType == BodyType.MultiPart)
        {
            if (BytesEncodingUtils.TryGetEncoding(data.BodyAsBytes, out var encoding) &&
                SupportedBodyAsStringEncodingForMultipart.Select(x => x.Equals(encoding)).Any())
            {
                data.BodyAsString = encoding.GetString(data.BodyAsBytes);
                data.Encoding = encoding;
                data.DetectedBodyType = BodyType.String;
            }

            return data;
        }

        // Try to get the body as String, FormUrlEncoded or Json
        try
        {
            data.BodyAsString = DefaultEncoding.GetString(data.BodyAsBytes);
            data.Encoding = DefaultEncoding;
            data.DetectedBodyType = BodyType.String;

            // If string is not null or empty, try to deserialize the string to a IDictionary<string, string>
            if (settings.BodyHandling.HasFlag(BodyHandling.TryDeserializeFormUrlEncoded) &&
                data.DetectedBodyTypeFromContentType == BodyType.FormUrlEncoded &&
                QueryStringParser.TryParse(data.BodyAsString, false, out var nameValueCollection)
            )
            {
                try
                {
                    data.BodyAsFormUrlEncoded = nameValueCollection;
                    data.DetectedBodyType = BodyType.FormUrlEncoded;
                }
                catch
                {
                    // Deserialize FormUrlEncoded failed, just ignore.
                }
            }

            // If string is not null or empty, try to deserialize the string to a JObject
            if (settings.BodyHandling.HasFlag(BodyHandling.TryDeserializeJson) && JsonUtils.IsJson(data.BodyAsString))
            {
                try
                {
                    data.BodyAsJson = JsonUtils.DeserializeObject(data.BodyAsString);
                    data.DetectedBodyType = BodyType.Json;
                }
                catch
                {
                    // JsonConvert failed, just ignore.
                }
            }
        }
        catch
        {
            // Reading as string failed, just ignore
        }

        return data;
    }

    private static async Task<(string? ContentType, byte[] Bytes)> ReadBytesAsync(Stream stream, string? contentEncoding = null, bool decompressGZipAndDeflate = true)
    {
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream).ConfigureAwait(false);
        byte[] data = memoryStream.ToArray();

        var type = contentEncoding?.ToLowerInvariant();
        if (decompressGZipAndDeflate && type is "gzip" or "deflate")
        {
            return (type, CompressionUtils.Decompress(type, data));
        }

        return (null, data);
    }
}