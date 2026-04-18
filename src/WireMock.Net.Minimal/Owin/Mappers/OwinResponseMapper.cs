// Copyright © WireMock.Net

using System.Globalization;
using System.Net;
using System.Text;
using JsonConverter.Abstractions;
using Microsoft.AspNetCore.Http;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;
using WireMock.Http;
using WireMock.ResponseBuilders;
using WireMock.ResponseProviders;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.Owin.Mappers;

/// <summary>
/// OwinResponseMapper
/// </summary>
internal class OwinResponseMapper(IWireMockMiddlewareOptions options) : IOwinResponseMapper
{
    private readonly IRandomizerNumber<double> _randomizerDouble = RandomizerFactory.GetRandomizer(new FieldOptionsDouble { Min = 0, Max = 1 });
    private readonly IRandomizerBytes _randomizerBytes = RandomizerFactory.GetRandomizer(new FieldOptionsBytes { Min = 100, Max = 200 });
    private readonly Encoding _utf8NoBom = new UTF8Encoding(false);

    // https://msdn.microsoft.com/en-us/library/78h415ay(v=vs.110).aspx
    private static readonly IDictionary<string, Action<HttpResponse, bool, WireMockList<string>>> ResponseHeadersToFix =
        new Dictionary<string, Action<HttpResponse, bool, WireMockList<string>>>(StringComparer.OrdinalIgnoreCase)
        {
            { HttpKnownHeaderNames.ContentType, (r, _, v) => r.ContentType = v.FirstOrDefault() },
            { HttpKnownHeaderNames.ContentLength, (r, hasBody, v) =>
                {
                    // Only set the Content-Length header if the response does not have a body
                    if (!hasBody && long.TryParse(v.FirstOrDefault(), out var contentLength))
                    {
                        r.ContentLength = contentLength;
                    }
                }
            }
        };

    /// <inheritdoc />
    public async Task MapAsync(IResponseMessage? responseMessage, HttpResponse response)
    {
        if (responseMessage == null || responseMessage is WebSocketHandledResponse)
        {
            return;
        }

        var bodyData = responseMessage.BodyData;
        if (bodyData?.GetDetectedBodyType() == BodyType.SseString)
        {
            await HandleSseStringAsync(responseMessage, response, bodyData);
            return;
        }

        byte[]? bytes;
        switch (responseMessage.FaultType)
        {
            case FaultType.EMPTY_RESPONSE:
                bytes = IsFault(responseMessage) ? [] : await GetNormalBodyAsync(responseMessage).ConfigureAwait(false);
                break;

            case FaultType.MALFORMED_RESPONSE_CHUNK:
                bytes = await GetNormalBodyAsync(responseMessage).ConfigureAwait(false) ?? [];
                if (IsFault(responseMessage))
                {
                    bytes = bytes.Take(bytes.Length / 2).Union(_randomizerBytes.Generate()).ToArray();
                }
                break;

            default:
                bytes = await GetNormalBodyAsync(responseMessage).ConfigureAwait(false);
                break;
        }

        if (responseMessage.StatusCode is HttpStatusCode or int)
        {
            response.StatusCode = MapStatusCode((int)responseMessage.StatusCode);
        }
        else if (responseMessage.StatusCode is string statusCodeAsString)
        {
            // Note: this case will also match on null
            _ = int.TryParse(statusCodeAsString, out var statusCodeTypeAsInt);
            response.StatusCode = MapStatusCode(statusCodeTypeAsInt);
        }

        SetResponseHeaders(responseMessage, bytes != null, response);

        if (bytes != null)
        {
            try
            {
                await response.Body.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                options.Logger.Warn("Error writing response body. Exception : {0}", ex);
            }
        }

        SetResponseTrailingHeaders(responseMessage, response);
    }

    private static async Task HandleSseStringAsync(IResponseMessage responseMessage, HttpResponse response, IBodyData bodyData)
    {
        if (bodyData.SseStringQueue == null)
        {
            return;
        }

        SetResponseHeaders(responseMessage, true, response);

        string? text;
        do
        {
            if (bodyData.SseStringQueue.TryRead(out text))
            {
                await response.WriteAsync(text);
                await response.Body.FlushAsync();
            }
        } while (text != null);
    }

    private int MapStatusCode(int code)
    {
        if (options.AllowOnlyDefinedHttpStatusCodeInResponse == true && !Enum.IsDefined(typeof(HttpStatusCode), code))
        {
            return (int)HttpStatusCode.OK;
        }

        return code;
    }

    private bool IsFault(IResponseMessage responseMessage)
    {
        return responseMessage.FaultPercentage == null || _randomizerDouble.Generate() <= responseMessage.FaultPercentage;
    }

    private async Task<byte[]?> GetNormalBodyAsync(IResponseMessage responseMessage)
    {
        var bodyData = responseMessage.BodyData;
        switch (bodyData?.GetDetectedBodyType())
        {
            case BodyType.String:
            case BodyType.FormUrlEncoded:
                return (bodyData.Encoding ?? _utf8NoBom).GetBytes(bodyData.BodyAsString!);

            case BodyType.Json:
                var jsonConverterOptions = new JsonConverterOptions
                {
                    WriteIndented = bodyData.BodyAsJsonIndented == true,
                    IgnoreNullValues = true
                };

                var jsonBody = options.DefaultJsonSerializer.Serialize(bodyData.BodyAsJson!, jsonConverterOptions);
                return (bodyData.Encoding ?? _utf8NoBom).GetBytes(jsonBody);

            case BodyType.ProtoBuf:
                if (TypeLoader.TryLoadStaticInstance<IProtoBufUtils>(out var protoBufUtils))
                {
                    var protoDefinitions = bodyData.ProtoDefinition?.Invoke().Texts;
                    return await protoBufUtils.GetProtoBufMessageWithHeaderAsync(protoDefinitions, bodyData.ProtoBufMessageType, bodyData.BodyAsJson).ConfigureAwait(false);
                }
                break;

            case BodyType.Bytes:
                return bodyData.BodyAsBytes;

            case BodyType.File:
                return options.FileSystemHandler?.ReadResponseBodyAsFile(bodyData.BodyAsFile!);

            case BodyType.MultiPart:
                options.Logger.Warn("MultiPart body type is not handled!");
                break;

            case BodyType.None:
                break;
        }

        return null;
    }

    private static void SetResponseHeaders(IResponseMessage responseMessage, bool hasBody, HttpResponse response)
    {
        // Force setting the Date header (#577)
        AppendResponseHeader(
            response,
            HttpKnownHeaderNames.Date,
            [DateTime.UtcNow.ToString(CultureInfo.InvariantCulture.DateTimeFormat.RFC1123Pattern, CultureInfo.InvariantCulture)]
        );

        // Set other headers
        foreach (var item in responseMessage.Headers!)
        {
            var headerName = item.Key;
            var value = item.Value;
            if (ResponseHeadersToFix.TryGetValue(headerName, out var action))
            {
                action.Invoke(response, hasBody, value);
            }
            else
            {
                // Check if this response header can be added (#148, #227 and #720)
                if (!HttpKnownHeaderNames.IsRestrictedResponseHeader(headerName))
                {
                    AppendResponseHeader(response, headerName, value.ToArray());
                }
            }
        }
    }

    private static void SetResponseTrailingHeaders(IResponseMessage responseMessage, HttpResponse response)
    {
        if (responseMessage.TrailingHeaders == null)
        {
            return;
        }

#if TRAILINGHEADERS
        foreach (var (headerName, value) in responseMessage.TrailingHeaders)
        {
            if (ResponseHeadersToFix.TryGetValue(headerName, out var action))
            {
                action.Invoke(response, false, value);
            }
            else
            {
                // Check if this trailing header can be added to the response
                if (response.SupportsTrailers() && !HttpKnownHeaderNames.IsRestrictedResponseHeader(headerName))
                {
                    response.AppendTrailer(headerName, new Microsoft.Extensions.Primitives.StringValues(value.ToArray()));
                }
            }
        }
#endif
    }

    private static void AppendResponseHeader(HttpResponse response, string headerName, string[] values)
    {
        response.Headers.Append(headerName, values);
    }
}