// Copyright © WireMock.Net

using System.Net;

namespace WireMock;

internal interface IResponseMessageBuilder
{
    ResponseMessage Create(HttpStatusCode statusCode, string? status, Guid? guid = null);

    ResponseMessage Create(int statusCode, string? status, Guid? guid = null);

    ResponseMessage Create(int statusCode, string? status, string? error, Guid? guid = null);

    ResponseMessage Create(HttpStatusCode statusCode);
}