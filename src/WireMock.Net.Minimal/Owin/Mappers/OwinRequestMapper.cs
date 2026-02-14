// Copyright © WireMock.Net

using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using WireMock.Http;
using WireMock.Models;
using WireMock.Util;

namespace WireMock.Owin.Mappers;

/// <summary>
/// OwinRequestMapper
/// </summary>
internal class OwinRequestMapper : IOwinRequestMapper
{
    /// <inheritdoc />
    public async Task<RequestMessage> MapAsync(HttpContext context, IWireMockMiddlewareOptions options)
    {
        var request = context.Request;
        var (urlDetails, clientIP) = ParseRequest(request);

        var method = request.Method;
        var httpVersion = HttpVersionParser.Parse(request.Protocol);

        var headers = new Dictionary<string, string[]>();
        IEnumerable<string>? contentEncodingHeader = null;
        foreach (var header in request.Headers)
        {
            headers.Add(header.Key, header.Value!);

            if (string.Equals(header.Key, HttpKnownHeaderNames.ContentEncoding, StringComparison.OrdinalIgnoreCase))
            {
                contentEncodingHeader = header.Value;
            }
        }

        var cookies = new Dictionary<string, string>();
        if (request.Cookies.Any())
        {
            foreach (var cookie in request.Cookies)
            {
                cookies.Add(cookie.Key, cookie.Value);
            }
        }

        IBodyData? body = null;
        if (request.Body != null && BodyParser.ShouldParseBody(method, options.AllowBodyForAllHttpMethods == true))
        {
            var bodyParserSettings = new BodyParserSettings
            {
                Stream = request.Body,
                ContentType = request.ContentType,
                DeserializeJson = !options.DisableJsonBodyParsing.GetValueOrDefault(false),
                ContentEncoding = contentEncodingHeader?.FirstOrDefault(),
                DecompressGZipAndDeflate = !options.DisableRequestBodyDecompressing.GetValueOrDefault(false)
            };

            body = await BodyParser.ParseAsync(bodyParserSettings).ConfigureAwait(false);
        }

        return new RequestMessage(
            options,
            urlDetails,
            method,
            clientIP,
            body,
            headers,
            cookies,
            httpVersion,
            await request.HttpContext.Connection.GetClientCertificateAsync()
        )
        {
            DateTime = DateTime.UtcNow
        };
    }

    private static (UrlDetails UrlDetails, string ClientIP) ParseRequest(HttpRequest request)
    {
        var urlDetails = UrlUtils.Parse(new Uri(request.GetEncodedUrl()), request.PathBase);

        var connection = request.HttpContext.Connection;
        string clientIP;
        if (connection.RemoteIpAddress is null)
        {
            clientIP = string.Empty;
        }
        else if (connection.RemoteIpAddress.IsIPv4MappedToIPv6)
        {
            clientIP = connection.RemoteIpAddress.MapToIPv4().ToString();
        }
        else
        {
            clientIP = connection.RemoteIpAddress.ToString();
        }

        return (urlDetails, clientIP);
    }
}