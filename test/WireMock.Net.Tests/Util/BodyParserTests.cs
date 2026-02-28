// Copyright © WireMock.Net

using System.Text;

using WireMock.Types;
using WireMock.Util;

namespace WireMock.Net.Tests.Util;

public class BodyParserTests
{
    [Theory]
    [InlineData("application/json", "{ \"x\": 1 }", BodyType.Json, BodyType.Json)]
    [InlineData("application/json; charset=utf-8", "{ \"x\": 1 }", BodyType.Json, BodyType.Json)]
    [InlineData("application/json; odata.metadata=minimal", "{ \"x\": 1 }", BodyType.Json, BodyType.Json)]
    [InlineData("application/vnd.api+json", "{ \"x\": 1 }", BodyType.Json, BodyType.Json)]
    [InlineData("application/vnd.test+json", "{ \"x\": 1 }", BodyType.Json, BodyType.Json)]
    public async Task BodyParser_Parse_ContentTypeJson(string contentType, string bodyAsJson, BodyType detectedBodyType, BodyType detectedBodyTypeFromContentType)
    {
        // Arrange
        var bodyParserSettings = new BodyParserSettings
        {
            Stream = new MemoryStream(Encoding.UTF8.GetBytes(bodyAsJson)),
            ContentType = contentType,
            DeserializeJson = true
        };

        // Act
        var body = await BodyParser.ParseAsync(bodyParserSettings);

        // Assert
        body.BodyAsBytes.Should().NotBeNull();
        body.BodyAsJson.Should().NotBeNull();
        body.BodyAsString.Should().Be(bodyAsJson);
        body.DetectedBodyType.Should().Be(detectedBodyType);
        body.DetectedBodyTypeFromContentType.Should().Be(detectedBodyTypeFromContentType);
    }

    [Theory]
    [InlineData("application/xml", "<xml>hello</xml>", BodyType.String, BodyType.String)]
    [InlineData("something", "hello", BodyType.String, BodyType.Bytes)]
    public async Task BodyParser_Parse_ContentTypeString(string contentType, string bodyAsString, BodyType detectedBodyType, BodyType detectedBodyTypeFromContentType)
    {
        // Arrange
        var bodyParserSettings = new BodyParserSettings
        {
            Stream = new MemoryStream(Encoding.UTF8.GetBytes(bodyAsString)),
            ContentType = contentType,
            DeserializeJson = true
        };

        // Act
        var body = await BodyParser.ParseAsync(bodyParserSettings);

        // Assert
        body.BodyAsBytes.Should().NotBeNull();
        body.BodyAsJson.Should().BeNull();
        body.BodyAsString.Should().Be(bodyAsString);
        body.DetectedBodyType.Should().Be(detectedBodyType);
        body.DetectedBodyTypeFromContentType.Should().Be(detectedBodyTypeFromContentType);
    }

    [Theory]
    [InlineData(new byte[] { 123, 32, 34, 120, 34, 58, 32, 49, 50, 51, 32, 125 }, BodyType.Json)]
    [InlineData(new byte[] { 97 }, BodyType.String)]
    [InlineData(new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }, BodyType.Bytes)]
    public async Task BodyParser_Parse_DetectedBodyType(byte[] content, BodyType detectedBodyType)
    {
        // arrange
        var bodyParserSettings = new BodyParserSettings
        {
            Stream = new MemoryStream(content),
            ContentType = null,
            DeserializeJson = true
        };

        // act
        var body = await BodyParser.ParseAsync(bodyParserSettings);

        // assert
        body.DetectedBodyType.Should().Be(detectedBodyType);
    }

    [Theory]
    [InlineData(new byte[] { 123, 32, 34, 120, 34, 58, 32, 49, 50, 51, 32, 125 }, BodyType.String)]
    [InlineData(new byte[] { 97 }, BodyType.String)]
    [InlineData(new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }, BodyType.Bytes)]
    public async Task BodyParser_Parse_DetectedBodyTypeNoJsonParsing(byte[] content, BodyType detectedBodyType)
    {
        // arrange
        var bodyParserSettings = new BodyParserSettings
        {
            Stream = new MemoryStream(content),
            ContentType = null,
            DeserializeJson = false
        };

        // act
        var body = await BodyParser.ParseAsync(bodyParserSettings);

        // assert
        body.DetectedBodyType.Should().Be(detectedBodyType);
    }

    [Fact]
    public async Task BodyParser_Parse_WithUTF8EncodingAndContentTypeMultipart_DetectedBodyTypeEqualsString()
    {
        // Arrange
        var contentType = "multipart/form-data";
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

        var bodyParserSettings = new BodyParserSettings
        {
            Stream = new MemoryStream(Encoding.UTF8.GetBytes(body)),
            ContentType = contentType,
            DeserializeJson = true
        };

        // Act
        var result = await BodyParser.ParseAsync(bodyParserSettings);

        // Assert
        result.DetectedBodyType.Should().Be(BodyType.String);
        result.DetectedBodyTypeFromContentType.Should().Be(BodyType.MultiPart);
        result.BodyAsBytes.Should().NotBeNull();
        result.BodyAsJson.Should().BeNull();
        result.BodyAsString.Should().NotBeNull();
    }

    [Fact]
    public async Task BodyParser_Parse_WithUTF16EncodingAndContentTypeMultipart_DetectedBodyTypeEqualsString()
    {
        // Arrange
        string contentType = "multipart/form-data";
        string body = char.ConvertFromUtf32(0x1D161); //U+1D161 = MUSICAL SYMBOL SIXTEENTH NOTE
        var bodyParserSettings = new BodyParserSettings
        {
            Stream = new MemoryStream(Encoding.UTF8.GetBytes(body)),
            ContentType = contentType,
            DeserializeJson = true
        };

        // Act
        var result = await BodyParser.ParseAsync(bodyParserSettings);

        // Assert
        result.DetectedBodyType.Should().Be(BodyType.Bytes);
        result.DetectedBodyTypeFromContentType.Should().Be(BodyType.MultiPart);
        result.BodyAsBytes.Should().NotBeNull();
        result.BodyAsJson.Should().BeNull();
        result.BodyAsString.Should().BeNull();
    }

    [Theory]
    [InlineData("hello", BodyType.String, BodyType.Bytes)]
    public async Task BodyParser_Parse_ContentTypeIsNull(string bodyAsString, BodyType detectedBodyType, BodyType detectedBodyTypeFromContentType)
    {
        // Arrange
        var bodyParserSettings = new BodyParserSettings
        {
            Stream = new MemoryStream(Encoding.UTF8.GetBytes(bodyAsString)),
            ContentType = null,
            DeserializeJson = true
        };

        // Act
        var body = await BodyParser.ParseAsync(bodyParserSettings);

        // Assert
        body.BodyAsBytes.Should().NotBeNull();
        body.BodyAsJson.Should().BeNull();
        body.BodyAsString.Should().Be(bodyAsString);
        body.DetectedBodyType.Should().Be(detectedBodyType);
        body.DetectedBodyTypeFromContentType.Should().Be(detectedBodyTypeFromContentType);
    }

    [Theory]
    [InlineData("gzip")]
    [InlineData("deflate")]
    public async Task BodyParser_Parse_ContentEncoding_GZip_And_DecompressGzipAndDeflate_Is_True_Should_Decompress(string compression)
    {
        // Arrange
        var bytes = Encoding.ASCII.GetBytes("0");
        var compressed = CompressionUtils.Compress(compression, bytes);
        var bodyParserSettings = new BodyParserSettings
        {
            Stream = new MemoryStream(compressed),
            ContentType = "text/plain",
            DeserializeJson = false,
            ContentEncoding = compression.ToUpperInvariant(),
            DecompressGZipAndDeflate = true
        };

        // Act
        var result = await BodyParser.ParseAsync(bodyParserSettings);

        // Assert
        result.DetectedBodyType.Should().Be(BodyType.String);
        result.DetectedBodyTypeFromContentType.Should().Be(BodyType.String);
        result.BodyAsBytes.Should().BeEquivalentTo(new byte[] { 48 });
        result.BodyAsJson.Should().BeNull();
        result.BodyAsString.Should().Be("0");
        result.DetectedCompression.Should().Be(compression);
    }

    [Theory]
    [InlineData("gzip")]
    [InlineData("deflate")]
    public async Task BodyParser_Parse_ContentEncoding_GZip_And_DecompressGzipAndDeflate_Is_False_Should_Not_Decompress(string compression)
    {
        // Arrange
        var bytes = Encoding.ASCII.GetBytes(Guid.NewGuid().ToString());
        var compressed = CompressionUtils.Compress(compression, bytes);
        var bodyParserSettings = new BodyParserSettings
        {
            Stream = new MemoryStream(compressed),
            ContentType = "text/plain",
            DeserializeJson = false,
            ContentEncoding = compression.ToUpperInvariant(),
            DecompressGZipAndDeflate = false
        };

        // Act
        var result = await BodyParser.ParseAsync(bodyParserSettings);

        // Assert
        result.BodyAsBytes.Should().BeEquivalentTo(compressed);
        result.DetectedCompression.Should().BeNull();
    }

    [Theory]
    [InlineData("HEAD", false)]
    [InlineData("GET", false)]
    [InlineData("PUT", true)]
    [InlineData("POST", true)]
    [InlineData("DELETE", true)]
    [InlineData("TRACE", false)]
    [InlineData("OPTIONS", true)]
    [InlineData("CONNECT", false)]
    [InlineData("PATCH", true)]
    public void BodyParser_ShouldParseBodyForMethodAndAllowAllIsFalse_ExpectedResultForKnownMethods(string method, bool resultShouldBe)
    {
        BodyParser.ShouldParseBody(method, false).Should().Be(resultShouldBe);
    }

    [Theory]
    [InlineData("HEAD")]
    [InlineData("GET")]
    [InlineData("PUT")]
    [InlineData("POST")]
    [InlineData("DELETE")]
    [InlineData("TRACE")]
    [InlineData("OPTIONS")]
    [InlineData("CONNECT")]
    [InlineData("PATCH")]
    [InlineData("REPORT")]
    [InlineData("SOME-UNKNOWN-METHOD")]
    public void BodyParser_ShouldParseBodyForMethodAndAllowAllIsTrue_ExpectedResultShouldBeTrue(string method)
    {
        BodyParser.ShouldParseBody(method, true).Should().BeTrue();
    }

    [Theory]
    [InlineData("REPORT")]
    [InlineData("SOME-UNKNOWN-METHOD")]
    public void BodyParser_ShouldParseBody_DefaultIsTrueForUnknownMethods(string method)
    {
        BodyParser.ShouldParseBody(method, false).Should().BeTrue();
    }
}

