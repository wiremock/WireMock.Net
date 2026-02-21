// Copyright © WireMock.Net

using Microsoft.AspNetCore.Http;
using Moq;
using NFluent;
using WireMock.Handlers;
using WireMock.Models;
using WireMock.ResponseBuilders;
using WireMock.Settings;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.Net.Tests.ResponseBuilders;

public class ResponseWithHandlebarsHelpersTests
{
    private const string ClientIp = "::1";

    private readonly WireMockServerSettings _settings = new();

    public ResponseWithHandlebarsHelpersTests()
    {
        var filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
        filesystemHandlerMock.Setup(fs => fs.ReadResponseBodyAsString(It.IsAny<string>())).Returns("abc");

        _settings.FileSystemHandler = filesystemHandlerMock.Object;
    }

    [Fact]
    public async Task Response_ProvideResponseAsync_HandlebarsHelpers_String_Uppercase()
    {
        // Assign
        var body = new BodyData { BodyAsString = "abc", DetectedBodyType = BodyType.String };

        var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

        var responseBuilder = Response.Create()
            .WithBody("{{String.Uppercase request.body}}")
            .WithTransformer();

        // Act
        var response = await responseBuilder.ProvideResponseAsync(Mock.Of<IMapping>(), Mock.Of<HttpContext>(), request, _settings);

        // assert
        Check.That(response.Message.BodyData.BodyAsString).Equals("ABC");
    }
}