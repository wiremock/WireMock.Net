// Copyright © WireMock.Net

using AwesomeAssertions;
using NFluent;
using WireMock.Models;
using WireMock.ResponseBuilders;
using WireMock.Settings;
using WireMock.Types;
using WireMock.Util;
using WireMock.Handlers;
using Moq;
#if NET452
using Microsoft.Owin;
#else
using Microsoft.AspNetCore.Http;
#endif

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
        Check.That(response.Message.BodyData.BodyAsString).Equals("test http://localhost/foo /foo POSt");
    }
}
