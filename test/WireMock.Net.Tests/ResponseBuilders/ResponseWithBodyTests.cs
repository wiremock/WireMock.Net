// Copyright © WireMock.Net

using System;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using JsonConverter.Newtonsoft.Json;
using Moq;
using Newtonsoft.Json.Linq;
using NFluent;
using WireMock.Handlers;
using WireMock.Models;
using WireMock.ResponseBuilders;
using WireMock.Settings;
using WireMock.Types;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.ResponseBuilders;

public class ResponseWithBodyTests
{
    private const string ClientIp = "::1";

    private readonly Mock<IMapping> _mappingMock;
    private readonly Mock<IFileSystemHandler> _filesystemHandlerMock;
    private readonly WireMockServerSettings _settings = new();

    public ResponseWithBodyTests()
    {
        _mappingMock = new Mock<IMapping>();

        _filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
        _filesystemHandlerMock.Setup(fs => fs.ReadResponseBodyAsString(It.IsAny<string>())).Returns("abc");

        _settings.FileSystemHandler = _filesystemHandlerMock.Object;
    }

    [Fact]
    public async Task Response_ProvideResponse_WithBody_Bytes_Encoding_Destination_String()
    {
        // given
        var body = new BodyData
        {
            DetectedBodyType = BodyType.String,
            BodyAsString = "abc"
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "POST", ClientIp, body);

        var responseBuilder = Response.Create().WithBody(new byte[] { 48, 49 }, BodyDestinationFormat.String, Encoding.ASCII);

        // act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings).ConfigureAwait(false);

        // then
        Check.That(response.Message.BodyData.BodyAsString).Equals("01");
        Check.That(response.Message.BodyData.BodyAsBytes).IsNull();
        Check.That(response.Message.BodyData.Encoding).Equals(Encoding.ASCII);
    }

    [Fact]
    public async Task Response_ProvideResponse_WithBody_Bytes_Encoding_Destination_Bytes()
    {
        // given
        var body = new BodyData
        {
            DetectedBodyType = BodyType.String,
            BodyAsString = "abc"
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "POST", ClientIp, body);

        var responseBuilder = Response.Create().WithBody(new byte[] { 48, 49 }, BodyDestinationFormat.SameAsSource, Encoding.ASCII);

        // act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings).ConfigureAwait(false);

        // then
        Check.That(response.Message.BodyData.BodyAsBytes).ContainsExactly(new byte[] { 48, 49 });
        Check.That(response.Message.BodyData.BodyAsString).IsNull();
        Check.That(response.Message.BodyData.Encoding).IsNull();
    }

    [Fact]
    public async Task Response_ProvideResponse_WithBody_String_Encoding()
    {
        // given
        var body = new BodyData
        {
            DetectedBodyType = BodyType.String,
            BodyAsString = "abc"
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "POST", ClientIp, body);

        var responseBuilder = Response.Create().WithBody("test", null, Encoding.ASCII);

        // act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings).ConfigureAwait(false);

        // then
        Check.That(response.Message.BodyData.BodyAsString).Equals("test");
        Check.That(response.Message.BodyData.Encoding).Equals(Encoding.ASCII);
    }

    [Fact]
    public async Task Response_ProvideResponse_WithBody_Object_Encoding()
    {
        // given
        var body = new BodyData
        {
            DetectedBodyType = BodyType.String,
            BodyAsString = "abc"
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "POST", ClientIp, body);

        object x = new { value = "test" };
        var responseBuilder = Response.Create().WithBodyAsJson(x, Encoding.ASCII);

        // act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings).ConfigureAwait(false);

        // then
        Check.That(response.Message.BodyData.BodyAsJson).Equals(x);
        Check.That(response.Message.BodyData.Encoding).Equals(Encoding.ASCII);
    }

    [Fact]
    public async Task Response_ProvideResponse_WithBody_String_SameAsSource_Encoding()
    {
        // Assign
        var request = new RequestMessage(new UrlDetails("http://localhost"), "GET", ClientIp);

        var responseBuilder = Response.Create().WithBody("r", BodyDestinationFormat.SameAsSource, Encoding.ASCII);

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings).ConfigureAwait(false);

        // Assert
        Check.That(response.Message.BodyData.BodyAsBytes).IsNull();
        Check.That(response.Message.BodyData.BodyAsJson).IsNull();
        Check.That(response.Message.BodyData.BodyAsString).Equals("r");
        Check.That(response.Message.BodyData.Encoding).Equals(Encoding.ASCII);
    }

    [Fact]
    public async Task Response_ProvideResponse_WithBody_String_Bytes_Encoding()
    {
        // Assign
        var request = new RequestMessage(new UrlDetails("http://localhost"), "GET", ClientIp);

        var responseBuilder = Response.Create().WithBody("r", BodyDestinationFormat.Bytes, Encoding.ASCII);

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings).ConfigureAwait(false);

        // Assert
        Check.That(response.Message.BodyData.BodyAsString).IsNull();
        Check.That(response.Message.BodyData.BodyAsJson).IsNull();
        Check.That(response.Message.BodyData.BodyAsBytes).IsNotNull();
        Check.That(response.Message.BodyData.Encoding).Equals(Encoding.ASCII);
    }

    [Fact]
    public async Task Response_ProvideResponse_WithBody_String_Json_Encoding()
    {
        // Assign
        var request = new RequestMessage(new UrlDetails("http://localhost"), "GET", ClientIp);

        var responseBuilder = Response.Create().WithBody("{ \"value\": 42 }", BodyDestinationFormat.Json, Encoding.ASCII);

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings).ConfigureAwait(false);

        // Assert
        Check.That(response.Message.BodyData.BodyAsString).IsNull();
        Check.That(response.Message.BodyData.BodyAsBytes).IsNull();
        Check.That(((dynamic)response.Message.BodyData.BodyAsJson).value).Equals(42);
        Check.That(response.Message.BodyData.Encoding).Equals(Encoding.ASCII);
    }

    [Fact]
    public async Task Response_ProvideResponse_WithBodyAsJson_Object_Indented()
    {
        // given
        var body = new BodyData
        {
            DetectedBodyType = BodyType.String,
            BodyAsString = "abc"
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "POST", ClientIp, body);

        object x = new { message = "Hello" };
        var responseBuilder = Response.Create().WithBodyAsJson(x, true);

        // act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings).ConfigureAwait(false);

        // then
        Check.That(response.Message.BodyData.BodyAsJson).Equals(x);
        Check.That(response.Message.BodyData.BodyAsJsonIndented).IsEqualTo(true);
    }

    [Fact]
    public async Task Response_ProvideResponse_WithBodyAsJson_FuncObject()
    {
        // Arrange
        var requestBody = new BodyData
        {
            DetectedBodyType = BodyType.String,
            BodyAsString = "abc"
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "POST", ClientIp, requestBody);

        object responseBody = new { message = "Hello" };
        var responseBuilder = Response.Create().WithBodyAsJson(requestMessage => responseBody);

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings).ConfigureAwait(false);

        // Assert
        response.Message.BodyData!.BodyAsJson.Should().BeEquivalentTo(responseBody);
    }

    [Fact]
    public async Task Response_ProvideResponse_WithBodyAsJson_AsyncFuncObject()
    {
        // Arrange
        var requestBody = new BodyData
        {
            DetectedBodyType = BodyType.String,
            BodyAsString = "abc"
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "POST", ClientIp, requestBody);

        object responseBody = new { message = "Hello" };
        var responseBuilder = Response.Create().WithBodyAsJson(requestMessage => Task.FromResult(responseBody));

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings).ConfigureAwait(false);

        // Assert
        response.Message.BodyData!.BodyAsJson.Should().BeEquivalentTo(responseBody);
    }

    [Fact]
    public async Task Response_ProvideResponse_WithJsonBodyAndTransform()
    {
        // Assign
        const int request1Id = 1;
        const int request2Id = 2;

        var request1 = new RequestMessage(new UrlDetails($"http://localhost/test?id={request1Id}"), "GET", ClientIp);
        var request2 = new RequestMessage(new UrlDetails($"http://localhost/test?id={request2Id}"), "GET", ClientIp);

        var responseBuilder = Response.Create()
            .WithStatusCode(200)
            .WithBodyAsJson(JObject.Parse("{ \"id\": \"{{request.query.id}}\" }"))
            .WithTransformer();

        // Act
        var response1 = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request1, _settings).ConfigureAwait(false);
        var response2 = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request2, _settings).ConfigureAwait(false);

        // Assert
        Check.That(((JToken)response1.Message.BodyData.BodyAsJson).SelectToken("id")?.Value<int>()).IsEqualTo(request1Id);
        Check.That(response1.Message.BodyData.BodyAsBytes).IsNull();
        Check.That(response1.Message.BodyData.BodyAsString).IsNull();
        Check.That(response1.Message.StatusCode).IsEqualTo(200);

        Check.That(((JToken)response2.Message.BodyData.BodyAsJson).SelectToken("id")?.Value<int>()).IsEqualTo(request2Id);
        Check.That(response2.Message.BodyData.BodyAsBytes).IsNull();
        Check.That(response2.Message.BodyData.BodyAsString).IsNull();
        Check.That(response2.Message.StatusCode).IsEqualTo(200);
    }

    [Fact]
    public async Task Response_ProvideResponse_WithBodyAsFile()
    {
        var fileContents = "testFileContents" + Guid.NewGuid();
        var bodyDataAsFile = new BodyData { BodyAsFile = fileContents };

        var request1 = new RequestMessage(new UrlDetails("http://localhost/__admin/files/filename.txt"), "PUT", ClientIp, bodyDataAsFile);

        var responseBuilder = Response.Create().WithStatusCode(200).WithBody(fileContents);

        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request1, _settings).ConfigureAwait(false);

        Check.That(response.Message.StatusCode).IsEqualTo(200);
        Check.That(response.Message.BodyData.BodyAsString).Contains(fileContents);
    }

    [Fact]
    public async Task Response_ProvideResponse_WithResponseAsFile()
    {
        var fileContents = "testFileContents" + Guid.NewGuid();
        var bodyDataAsFile = new BodyData { BodyAsFile = fileContents };

        var request1 = new RequestMessage(new UrlDetails("http://localhost/__admin/files/filename.txt"), "GET", ClientIp, bodyDataAsFile);

        var responseBuilder = Response.Create().WithStatusCode(200).WithBody(fileContents);

        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request1, _settings).ConfigureAwait(false);

        Check.That(response.Message.StatusCode).IsEqualTo(200);
        Check.That(response.Message.BodyData.BodyAsString).Contains(fileContents);
    }

    [Fact]
    public async Task Response_ProvideResponse_WithResponseDeleted()
    {
        var fileContents = "testFileContents" + Guid.NewGuid();
        var bodyDataAsFile = new BodyData { BodyAsFile = fileContents };

        var request1 = new RequestMessage(new UrlDetails("http://localhost/__admin/files/filename.txt"), "DELETE", ClientIp, bodyDataAsFile);

        var responseBuilder = Response.Create().WithStatusCode(200).WithBody("File deleted.");

        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request1, _settings).ConfigureAwait(false);

        Check.That(response.Message.StatusCode).IsEqualTo(200);
        Check.That(response.Message.BodyData?.BodyAsString).Contains("File deleted.");
    }

    [Fact]
    public async Task Response_ProvideResponse_WithBody_NewtonsoftJsonConverter()
    {
        // Arrange
        var requestBody = new BodyData
        {
            DetectedBodyType = BodyType.String,
            BodyAsString = "abc"
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "POST", ClientIp, requestBody);

        var responseBuilder = Response.Create().WithBody(new { foo = "< > & ' 😀 👍 ❤️", n = 42 }, new NewtonsoftJsonConverter());

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings).ConfigureAwait(false);

        // Assert
        response.Message.BodyData!.BodyAsString.Should().Be("""{"foo":"< > & ' 😀 👍 ❤️","n":42}""");
    }

#if !(NET451 || NET452 || NET461)
    [Fact]
    public async Task Response_ProvideResponse_WithBody_SystemTextJsonConverter()
    {
        // Arrange
        var requestBody = new BodyData
        {
            DetectedBodyType = BodyType.String,
            BodyAsString = "abc"
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "POST", ClientIp, requestBody);

        var responseBuilder = Response.Create().WithBody(new { foo = "< > & ' 😀 👍 ❤️", n = 42 }, new JsonConverter.System.Text.Json.SystemTextJsonConverter());

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, request, _settings).ConfigureAwait(false);

        // Assert
        response.Message.BodyData!.BodyAsString.Should().Be("""{"foo":"\u003C \u003E \u0026 \u0027 \uD83D\uDE00 \uD83D\uDC4D \u2764\uFE0F","n":42}""");
    }
#endif
}