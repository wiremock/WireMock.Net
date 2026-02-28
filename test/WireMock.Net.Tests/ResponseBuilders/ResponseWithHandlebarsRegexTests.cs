// Copyright © WireMock.Net

using HandlebarsDotNet;
using Microsoft.AspNetCore.Http;
using Moq;

using WireMock.Handlers;
using WireMock.Models;
using WireMock.ResponseBuilders;
using WireMock.Settings;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.Net.Tests.ResponseBuilders;

public class ResponseWithHandlebarsRegexTests
{
    private const string ClientIp = "::1";
    private readonly WireMockServerSettings _settings = new();

    private readonly Mock<IMapping> _mappingMock;

    public ResponseWithHandlebarsRegexTests()
    {
        _mappingMock = new Mock<IMapping>();

        var filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
        filesystemHandlerMock.Setup(fs => fs.ReadResponseBodyAsString(It.IsAny<string>())).Returns("abc");

        _settings.FileSystemHandler = filesystemHandlerMock.Object;
    }

    [Fact]
    public async Task Response_ProvideResponseAsync_Handlebars_RegexMatch()
    {
        // Assign
        var body = new BodyData { BodyAsString = "abc", DetectedBodyType = BodyType.String };

        var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

        var responseBuilder = Response.Create()
            .WithBody("{{Regex.Match request.body \"^(\\w+)$\"}}")
            .WithTransformer();

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, Mock.Of<HttpContext>(), request, _settings);

        // assert
        response.Message.BodyData.BodyAsString.Should().Be("abc");
    }

    [Fact]
    public async Task Response_ProvideResponseAsync_Handlebars_RegexMatch_NoMatch()
    {
        // Assign
        var body = new BodyData { BodyAsString = "abc", DetectedBodyType = BodyType.String };

        var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

        var responseBuilder = Response.Create()
            .WithBody("{{Regex.Match request.body \"^?0$\"}}")
            .WithTransformer();

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, Mock.Of<HttpContext>(), request, _settings);

        // assert
        response.Message.BodyData.BodyAsString.Should().Be("");
    }

    [Fact]
    public async Task Response_ProvideResponseAsync_Handlebars_RegexMatch2()
    {
        // Assign
        var body = new BodyData { BodyAsString = "https://localhost:5000/", DetectedBodyType = BodyType.String };

        var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

        var responseBuilder = Response.Create()
            .WithBody("{{#Regex.Match request.body \"^(?<proto>\\w+)://[^/]+?(?<port>\\d+)/?\"}}{{this.port}}-{{this.proto}}{{/Regex.Match}}")
            .WithTransformer();

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, Mock.Of<HttpContext>(), request, _settings);

        // assert
        response.Message.BodyData.BodyAsString.Should().Be("5000-https");
    }

    [Fact]
    public async Task Response_ProvideResponseAsync_Handlebars_RegexMatch2_NoMatch()
    {
        // Assign
        var body = new BodyData { BodyAsString = "{{\\test", DetectedBodyType = BodyType.String };

        var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

        var responseBuilder = Response.Create()
            .WithBody("{{#Regex.Match request.body \"^(?<proto>\\w+)://[^/]+?(?<port>\\d+)/?\"}}{{this}}{{/Regex.Match}}")
            .WithTransformer();

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, Mock.Of<HttpContext>(), request, _settings);

        // assert
        response.Message.BodyData.BodyAsString.Should().Be("");
    }

    [Fact]
    public void Response_ProvideResponseAsync_Handlebars_RegexMatch2_Throws()
    {
        // Assign
        var body = new BodyData { BodyAsString = "{{\\test", DetectedBodyType = BodyType.String };

        var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "POST", ClientIp, body);

        var responseBuilder = Response.Create()
            .WithBody("{{#Regex.Match request.bodyAsJson \"^(?<proto>\\w+)://[^/]+?(?<port>\\d+)/?\"}}{{/Regex.Match}}")
            .WithTransformer();

        // Act
        Func<Task> act = () => responseBuilder.ProvideResponseAsync(_mappingMock.Object, Mock.Of<HttpContext>(), request, _settings);

        // Assert
        act.Should().ThrowAsync<HandlebarsException>();
    }
}
