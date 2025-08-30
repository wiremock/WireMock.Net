// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using WireMock.Http;
using WireMock.Models;
using WireMock.Util;
//#if !USE_ASPNETCORE
//using IRequest = Microsoft.Owin.IOwinRequest;
//#else
//using Microsoft.AspNetCore.Http.Extensions;
//using IRequest = Microsoft.AspNetCore.Http.HttpRequest;
//#endif

namespace WireMock.Owin.Mappers;

/// <summary>
/// OwinRequestMapper
/// </summary>
internal class OwinRequestMapper : IOwinRequestMapper
{
    /// <inheritdoc />
    public async Task<RequestMessage> MapAsync(HttpRequest request, IWireMockMiddlewareOptions options)
    {
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
            httpVersion
#if USE_ASPNETCORE
            , await request.HttpContext.Connection.GetClientCertificateAsync()
#endif
            )
        {
            DateTime = DateTime.UtcNow
        };
    }

    private static (UrlDetails UrlDetails, string ClientIP) ParseRequest(HttpRequest request)
    {
        //#if !USE_ASPNETCORE
        //        var urlDetails = UrlUtils.Parse(request.Uri, request.PathBase);
        //        var clientIP = request.RemoteIpAddress;
        //#else
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
        //#endif
        return (urlDetails, clientIP);
    }
}