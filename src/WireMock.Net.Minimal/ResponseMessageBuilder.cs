// Copyright © WireMock.Net

using System.Net;
using WireMock.Admin.Mappings;
using WireMock.Constants;
using WireMock.Http;
using WireMock.Types;
using WireMock.Util;

namespace WireMock;

internal static class ResponseMessageBuilder
{
    private static readonly IDictionary<string, WireMockList<string>> ContentTypeJsonHeaders = new Dictionary<string, WireMockList<string>>
    {
        { HttpKnownHeaderNames.ContentType, new WireMockList<string> { WireMockConstants.ContentTypeJson } }
    };

    internal static ResponseMessage Create(HttpStatusCode statusCode, string? status, Guid? guid = null)
    {
        return Create((int)statusCode, status, guid);
    }

    internal static ResponseMessage Create(int statusCode, string? status, Guid? guid = null)
    {
        return Create(statusCode, status, null, guid);
    }

    internal static ResponseMessage Create(int statusCode, string? status, string? error, Guid? guid = null)
    {
        var response = new ResponseMessage
        {
            StatusCode = statusCode,
            Headers = ContentTypeJsonHeaders
        };

        if (status != null || error != null)
        {
            response.BodyData = new BodyData
            {
                DetectedBodyType = BodyType.Json,
                BodyAsJson = new StatusModel
                {
                    Guid = guid,
                    Status = status,
                    Error = error
                }
            };
        }

        return response;
    }

    internal static ResponseMessage Create(HttpStatusCode statusCode)
    {
        return new ResponseMessage
        {
            StatusCode = statusCode
        };
    }
}