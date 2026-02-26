// Copyright © WireMock.Net

using Microsoft.AspNetCore.Http;
using Moq;
using Newtonsoft.Json.Linq;

using WireMock.Handlers;
using WireMock.Models;
using WireMock.ResponseBuilders;
using WireMock.Settings;

namespace WireMock.Net.Tests.ResponseBuilders;

public class ResponseWithHandlebarsXegerTests
{
    private const string ClientIp = "::1";
    private readonly WireMockServerSettings _settings = new();

    private readonly Mock<IMapping> _mappingMock;

    public ResponseWithHandlebarsXegerTests()
    {
        _mappingMock = new Mock<IMapping>();

        var filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
        filesystemHandlerMock.Setup(fs => fs.ReadResponseBodyAsString(It.IsAny<string>())).Returns("abc");

        _settings.FileSystemHandler = filesystemHandlerMock.Object;
    }

    [Fact]
    public async Task Response_ProvideResponseAsync_Handlebars_Xeger1()
    {
        // Assign
        var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "GET", ClientIp);

        var responseBuilder = Response.Create()
            .WithBodyAsJson(new
            {
                Number = "{{Xeger.Generate \"[1-9]{1}\\d{3}\"}}",
                Postcode = "{{Xeger.Generate \"[1-9][0-9]{3}[A-Z]{2}\"}}"
            })
            .WithTransformer();

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, Mock.Of<HttpContext>(), request, _settings);

        // Assert
        JObject j = JObject.FromObject(response.Message.BodyData.BodyAsJson);
        j["Number"].Value<int>().Should().BeGreaterThan(1000).And.BeLessThan(9999);
        j["Postcode"].Value<string>().Should().NotBeEmpty();
    }

    [Fact]
    public async Task Response_ProvideResponseAsync_Handlebars_Xeger2()
    {
        // Assign
        var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "GET", ClientIp);

        var responseBuilder = Response.Create()
            .WithBodyAsJson(new
            {
                Number = "{{#Xeger.Generate \"[1-9]{1}\\d{3}\"}}{{this}}{{/Xeger.Generate}}",
                Postcode = "{{#Xeger.Generate \"[1-9][0-9]{3}[A-Z]{2}\"}}{{this}}{{/Xeger.Generate}}"
            })
            .WithTransformer();

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, Mock.Of<HttpContext>(), request, _settings);

        // Assert
        JObject j = JObject.FromObject(response.Message.BodyData.BodyAsJson);
        j["Number"].Value<int>().Should().BeGreaterThan(1000).And.BeLessThan(9999);
        j["Postcode"].Value<string>().Should().NotBeEmpty();
    }
}

