// Copyright © WireMock.Net

using WireMock.Models;
using WireMock.ResponseBuilders;
using WireMock.Settings;
using WireMock.Types;
using WireMock.Util;
using WireMock.Handlers;
using Moq;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Scriban.Runtime;

namespace WireMock.Net.Tests.ResponseBuilders;

public class ResponseWithScribanTests
{
    private const string ClientIp = "::1";
    private readonly WireMockServerSettings _settings = new();

    private readonly Mock<IMapping> _mappingMock;

    public ResponseWithScribanTests()
    {
        _mappingMock = new Mock<IMapping>();

        var filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
        filesystemHandlerMock.Setup(fs => fs.ReadResponseBodyAsString(It.IsAny<string>())).Returns("abc");

        _settings.FileSystemHandler = filesystemHandlerMock.Object;
    }

    [Fact]
    public async Task Response_ProvideResponse_DotLiquid_WithNullBody_ShouldNotThrowException()
    {
        // Assign
        var urlDetails = UrlUtils.Parse(new Uri("http://localhost/wiremock/a/b"), new PathString("/wiremock"));
        var request = new RequestMessage(urlDetails, "GET", ClientIp);

        var responseBuilder = Response.Create().WithTransformer(TransformerType.ScribanDotLiquid);

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, Mock.Of<HttpContext>(), request, _settings);

        // Assert
        response.Message.BodyData.Should().BeNull();
    }

    [Fact]
    public async Task Response_ProvideResponse_DotLiquid_UrlPathVerb()
    {
        // Assign
        var body = new BodyData
        {
            BodyAsString = "whatever",
            DetectedBodyType = BodyType.String
        };
        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "POSt", ClientIp, body);

        var responseBuilder = Response.Create()
            .WithBody("test {{request.Url}} {{request.Path}} {{request.Method}}")
            .WithTransformer(TransformerType.Scriban);

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, Mock.Of<HttpContext>(), request, _settings);

        // Assert
        response.Message.BodyData.BodyAsString.Should().Be("test http://localhost/foo /foo POSt");
    }

    [Theory]
    [InlineData(TransformerType.Scriban)]
    [InlineData(TransformerType.ScribanDotLiquid)]
    public async Task Response_ProvideResponse_WithScribanRegistrationCallback_UsesCustomFunctionInBodyAndHeaders(TransformerType transformerType)
    {
        // Arrange
        _settings.ScribanRegistrationCallback = (scriptObject, _) =>
            ((ScriptObject)scriptObject).Import("fake_name", new Func<string>(() => "Jane Doe"));

        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "GET", ClientIp);

        var responseBuilder = Response.Create()
            .WithBody("{{ fake_name }} {{ request.Path }}")
            .WithHeader("x-name", "{{ fake_name }}")
            .WithTransformer(transformerType);

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, Mock.Of<HttpContext>(), request, _settings);

        // Assert
        response.Message.BodyData!.BodyAsString.Should().Be("Jane Doe /foo");
        response.Message.Headers!["x-name"].Should().ContainSingle().Which.Should().Be("Jane Doe");
    }

    [Theory]
    [InlineData(TransformerType.Scriban)]
    [InlineData(TransformerType.ScribanDotLiquid)]
    public async Task Response_ProvideResponse_WithScribanRegistrationCallback_UsesCustomFunctionInBodyAsJson(TransformerType transformerType)
    {
        // Arrange
        _settings.WithScribanRegistration((scriptObject, _) => scriptObject.Import("fake_name", new Func<string>(() => "Jane Doe")));

        var request = new RequestMessage(new UrlDetails("http://localhost/foo"), "GET", ClientIp);

        var responseBuilder = Response.Create()
            .WithBodyAsJson(new { name = "{{ fake_name }}", path = "{{ request.Path }}" })
            .WithTransformer(transformerType);

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, Mock.Of<HttpContext>(), request, _settings);

        // Assert
        JsonConvert.SerializeObject(response.Message.BodyData!.BodyAsJson).Should().Be("{\"name\":\"Jane Doe\",\"path\":\"/foo\"}");
    }
}

