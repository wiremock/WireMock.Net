// Copyright © WireMock.Net

using System.Text;
using FluentAssertions;
using NFluent;
using WireMock.Http;
using WireMock.Models;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.Net.Tests.Http;

public class HttpRequestMessageHelperTests
{
    private const string ClientIp = "::1";

    [Fact]
    public void HttpRequestMessageHelper_Create()
    {
        // Assign
        var headers = new Dictionary<string, string[]> { { "x", ["value-1"] } };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "PUT", ClientIp, null, headers);

        // Act
        var message = HttpRequestMessageHelper.Create(request, "http://url");

        // Assert
        Check.That(message.Headers.GetValues("x")).ContainsExactly("value-1");
    }

    [Fact]
    public async Task HttpRequestMessageHelper_Create_Bytes_Without_ContentTypeHeader()
    {
        // Assign
        var body = new BodyData
        {
            BodyAsBytes = Encoding.UTF8.GetBytes("hi"),
            DetectedBodyType = BodyType.Bytes
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "POST", ClientIp, body);

        // Act
        var message = HttpRequestMessageHelper.Create(request, "http://url");

        // Assert
        Check.That(await message.Content!.ReadAsByteArrayAsync()).ContainsExactly(Encoding.UTF8.GetBytes("hi"));
    }

    [Fact]
    public async Task HttpRequestMessageHelper_Create_TextPlain_Without_ContentTypeHeader()
    {
        // Assign
        var body = new BodyData
        {
            BodyAsString = "0123", // or 83 in decimal
            DetectedBodyType = BodyType.String
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "POST", ClientIp, body);

        // Act
        var message = HttpRequestMessageHelper.Create(request, "http://url");

        // Assert
        Check.That(await message.Content!.ReadAsStringAsync()).Equals("0123");
    }

    [Fact]
    public async Task HttpRequestMessageHelper_Create_Json()
    {
        // Assign
        var body = new BodyData
        {
            BodyAsJson = new { x = 42 },
            DetectedBodyType = BodyType.Json
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "POST", ClientIp, body);

        // Act
        var message = HttpRequestMessageHelper.Create(request, "http://url");

        // Assert
        Check.That(await message.Content!.ReadAsStringAsync()).Equals("{\"x\":42}");
    }

    [Fact]
    public async Task HttpRequestMessageHelper_Create_Json_With_ContentType_ApplicationJson()
    {
        // Assign
        var headers = new Dictionary<string, string[]> { { "Content-Type", ["application/json"] } };
        var body = new BodyData
        {
            BodyAsJson = new { x = 42 },
            DetectedBodyType = BodyType.Json
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "POST", ClientIp, body, headers);

        // Act
        var message = HttpRequestMessageHelper.Create(request, "http://url");

        // Assert
        Check.That(await message.Content!.ReadAsStringAsync()).Equals("{\"x\":42}");
        Check.That(message.Content.Headers.GetValues("Content-Type")).ContainsExactly("application/json");
    }

    [Fact]
    public async Task HttpRequestMessageHelper_Create_Json_With_ContentType_ApplicationJson_UTF8()
    {
        // Assign
        var headers = new Dictionary<string, string[]> { { "Content-Type", ["application/json; charset=utf-8"] } };
        var body = new BodyData
        {
            BodyAsJson = new { x = 42 },
            DetectedBodyType = BodyType.Json
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "POST", ClientIp, body, headers);

        // Act
        var message = HttpRequestMessageHelper.Create(request, "http://url");

        // Assert
        Check.That(await message.Content!.ReadAsStringAsync()).Equals("{\"x\":42}");
        Check.That(message.Content.Headers.GetValues("Content-Type")).ContainsExactly("application/json; charset=utf-8");
    }

    [Fact]
    public async Task HttpRequestMessageHelper_Create_Json_With_ContentType_MultiPartFormData()
    {
        // Assign
        var headers = new Dictionary<string, string[]> { { "Content-Type", ["multipart/form-data"] } };
        var body = new BodyData
        {
            BodyAsJson = new { x = 42 },
            DetectedBodyTypeFromContentType = BodyType.MultiPart,
            DetectedBodyType = BodyType.Json
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "POST", ClientIp, body, headers);

        // Act
        var message = HttpRequestMessageHelper.Create(request, "http://url");

        // Assert
        Check.That(await message.Content!.ReadAsStringAsync()).Equals("{\"x\":42}");
        Check.That(message.Content.Headers.GetValues("Content-Type")).ContainsExactly("multipart/form-data");
    }


    [Fact]
    public void HttpRequestMessageHelper_Create_String_With_ContentType_ApplicationXml()
    {
        // Assign
        var headers = new Dictionary<string, string[]> { { "Content-Type", ["application/xml"] } };
        var body = new BodyData
        {
            BodyAsString = "<xml>hello</xml>",
            DetectedBodyType = BodyType.String
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "PUT", ClientIp, body, headers);

        // Act
        var message = HttpRequestMessageHelper.Create(request, "http://url");

        // Assert
        Check.That(message.Content!.Headers.GetValues("Content-Type")).ContainsExactly("application/xml");
    }

    [Fact]
    public void HttpRequestMessageHelper_Create_String_With_ContentType_ApplicationXml_UTF8()
    {
        // Assign
        var headers = new Dictionary<string, string[]> { { "Content-Type", ["application/xml; charset=UTF-8"] } };
        var body = new BodyData
        {
            BodyAsString = "<xml>hello</xml>",
            DetectedBodyType = BodyType.String
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "PUT", ClientIp, body, headers);

        // Act
        var message = HttpRequestMessageHelper.Create(request, "http://url");

        // Assert
        Check.That(message.Content!.Headers.GetValues("Content-Type")).ContainsExactly("application/xml; charset=UTF-8");
    }

    [Fact]
    public void HttpRequestMessageHelper_Create_String_With_ContentType_ApplicationXml_ASCII()
    {
        // Assign
        var headers = new Dictionary<string, string[]> { { "Content-Type", ["application/xml; charset=Ascii"] } };
        var body = new BodyData
        {
            BodyAsString = "<xml>hello</xml>",
            DetectedBodyType = BodyType.String
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "PUT", ClientIp, body, headers);

        // Act
        var message = HttpRequestMessageHelper.Create(request, "http://url");

        // Assert
        Check.That(message.Content!.Headers.GetValues("Content-Type")).ContainsExactly("application/xml; charset=Ascii");
    }

    [Fact]
    public async Task HttpRequestMessageHelper_Create_MultiPart_With_ContentType_MultiPartFormData()
    {
        // Assign
        var contentType = "multipart/form-data";
        var headers = new Dictionary<string, string[]> { { "Content-Type", [contentType] } };
        var body =
            """
            -----------------------------9051914041544843365972754266
            Content-Disposition: form-data; name="text"

            text default
            -----------------------------9051914041544843365972754266
            Content-Disposition: form-data; name="file1"; filename="a.txt"
            Content-Type: text/plain

            Content of a txt

            -----------------------------9051914041544843365972754266
            Content-Disposition: form-data; name="file2"; filename="a.html"
            Content-Type: text/html

            <!DOCTYPE html><title>Content of a.html.</title>

            -----------------------------9051914041544843365972754266--
            """;
        var bodyData = new BodyData
        {
            BodyAsString = body,
            DetectedBodyType = BodyType.String,
            DetectedBodyTypeFromContentType = BodyType.MultiPart
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "POST", ClientIp, bodyData, headers);

        // Act
        var message = HttpRequestMessageHelper.Create(request, "http://url");

        // Assert
        Check.That(await message.Content!.ReadAsStringAsync()).Equals(body);
        Check.That(message.Content.Headers.GetValues("Content-Type")).ContainsExactly("multipart/form-data");
    }

    [Theory]
    [InlineData("HEAD", true)]
    [InlineData("GET", false)]
    [InlineData("PUT", false)]
    [InlineData("POST", false)]
    [InlineData("DELETE", false)]
    [InlineData("TRACE", false)]
    [InlineData("OPTIONS", false)]
    [InlineData("CONNECT", false)]
    [InlineData("PATCH", false)]
    public void HttpRequestMessageHelper_Create_ContentLengthAllowedForMethod(string method, bool resultShouldBe)
    {
        // Arrange
        var key = "Content-Length";
        var value = 1234;
        var headers = new Dictionary<string, string[]> { { key, ["1234"] } };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), method, ClientIp, null, headers);

        // Act
        var message = HttpRequestMessageHelper.Create(request, "http://url");

        // Assert
        message.Content?.Headers.ContentLength.Should().Be(resultShouldBe ? value : null);
    }
}