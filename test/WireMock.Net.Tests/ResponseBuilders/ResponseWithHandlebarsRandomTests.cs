// Copyright © WireMock.Net

using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using Newtonsoft.Json.Linq;
using WireMock.Handlers;
using WireMock.Models;
using WireMock.ResponseBuilders;
using WireMock.Settings;
using WireMock.Types;

namespace WireMock.Net.Tests.ResponseBuilders;

public class ResponseWithHandlebarsRandomTests
{
    private const string ClientIp = "::1";
    private readonly WireMockServerSettings _settings = new();

    private readonly Mock<IMapping> _mappingMock;

    public ResponseWithHandlebarsRandomTests()
    {
        _mappingMock = new Mock<IMapping>();

        var filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
        filesystemHandlerMock.Setup(fs => fs.ReadResponseBodyAsString(It.IsAny<string>())).Returns("abc");

        _settings.FileSystemHandler = filesystemHandlerMock.Object;
    }

    [Fact]
    public async Task Response_ProvideResponseAsync_Handlebars_Random1()
    {
        // Assign
        var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "GET", ClientIp);

        var responseBuilder = Response.Create()
            .WithBodyAsJson(new
            {
                Text = "{{Random Type=\"Text\" Min=8 Max=20}}",
                DateTime = "{{Random Type=\"DateTime\"}}",
                Integer = "{{Random Type=\"Integer\" Min=1000 Max=1000}}",
                Long = "{{Random Type=\"Long\" Min=77777777 Max=99999999}}"
            })
            .WithTransformer();

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, Mock.Of<HttpContext>(), request, _settings);

        // Assert
        JObject j = JObject.FromObject(response.Message.BodyData!.BodyAsJson!);
        j["Text"]?.Value<string>().Should().NotBeNullOrEmpty();
        j["Integer"]?.Value<int>().Should().Be(1000);
        j["Long"]?.Value<long>().Should().BeInRange(77777777, 99999999);
    }

    [Fact]
    public async Task Response_ProvideResponseAsync_Handlebars_Random1_Boolean()
    {
        // Assign
        var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "GET", ClientIp);

        var responseBuilder = Response.Create()
            .WithBodyAsJson(new
            {
                Value = "{{Random Type=\"Boolean\"}}"
            })
            .WithTransformer();

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, Mock.Of<HttpContext>(), request, _settings);

        // Assert
        JObject j = JObject.FromObject(response.Message.BodyData!.BodyAsJson!);
        j["Value"]?.Type.Should().Be(JTokenType.Boolean);
    }

    [Theory]
    [InlineData(ReplaceNodeOptions.EvaluateAndTryToConvert, JTokenType.Integer)]
    [InlineData(ReplaceNodeOptions.Evaluate, JTokenType.String)]
    public async Task Response_ProvideResponseAsync_Handlebars_Random1_Integer(ReplaceNodeOptions options, JTokenType expected)
    {
        // Assign
        var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "GET", ClientIp);

        var responseBuilder = Response.Create()
            .WithBodyAsJson(new
            {
                Value = "{{Random Type=\"Integer\"}}"
            })
            .WithTransformer(options);

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, Mock.Of<HttpContext>(), request, _settings);

        // Assert
        var jObject = JObject.FromObject(response.Message.BodyData!.BodyAsJson!);
        jObject["Value"]!.Type.Should().Be(expected);
    }

    [Theory]
    [InlineData(ReplaceNodeOptions.EvaluateAndTryToConvert, JTokenType.Guid)]
    [InlineData(ReplaceNodeOptions.Evaluate, JTokenType.String)]
    public async Task Response_ProvideResponseAsync_Handlebars_Random_Guid(ReplaceNodeOptions options, JTokenType expected)
    {
        // Assign
        var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "GET", ClientIp);

        var responseBuilder = Response.Create()
            .WithBodyAsJson(new
            {
                Guid1 = "{{Random Type=\"Guid\" Uppercase=false}}",
                Guid2 = "{{Random Type=\"Guid\"}}",
                Guid3 = "{{ String.Replace (Random Type=\"Guid\") \"-\" \"\" }}"
            })
            .WithTransformer(options);

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, Mock.Of<HttpContext>(), request, _settings);

        // Assert
        var jObject = JObject.FromObject(response.Message.BodyData!.BodyAsJson!);
        jObject["Guid1"]!.Type.Should().Be(expected);
        jObject["Guid2"]!.Type.Should().Be(expected);
    }

    [Fact]
    public async Task Response_ProvideResponseAsync_Handlebars_Random_StringReplaceGuid()
    {
        // Assign
        var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "GET", ClientIp);

        var responseBuilder = Response.Create()
            .WithBodyAsJson(new
            {
                MyGuid = "{{ String.Replace (Random Type=\"Guid\") \"-\" \"\" }}"
            })
            .WithTransformer();

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, Mock.Of<HttpContext>(), request, _settings);

        // Assert
        var jObject = JObject.FromObject(response.Message.BodyData!.BodyAsJson!);
        jObject["MyGuid"]!.Type.Should().Be(JTokenType.String);
    }

    [Fact]
    public async Task Response_ProvideResponseAsync_Handlebars_Random1_StringList()
    {
        // Assign
        var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "GET", ClientIp);

        var responseBuilder = Response.Create()
            .WithBodyAsJson(new
            {
                StringValue = "{{Random Type=\"StringList\" Values=[\"a\", \"b\", \"c\"]}}"
            })
            .WithTransformer();

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, Mock.Of<HttpContext>(), request, _settings);

        // Assert
        JObject j = JObject.FromObject(response.Message.BodyData!.BodyAsJson!);
        var value = j["StringValue"]?.Value<string>();
        new[] { "a", "b", "c" }.Should().Contain(value);
    }

    [Fact]
    public async Task Response_ProvideResponseAsync_Handlebars_Random_Integer()
    {
        // Assign
        var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "GET", ClientIp);

        var responseBuilder = Response.Create()
            .WithBodyAsJson(new
            {
                Integer = "{{#Random Type=\"Integer\" Min=10000000 Max=99999999}}{{this}}{{/Random}}",
            })
            .WithTransformer();

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, Mock.Of<HttpContext>(), request, _settings);

        // Assert
        JObject j = JObject.FromObject(response.Message.BodyData!.BodyAsJson!);
        j["Integer"]?.Value<int>().Should().BeInRange(10000000, 99999999);
    }

    [Fact]
    public async Task Response_ProvideResponseAsync_Handlebars_Random_Long()
    {
        // Assign
        var request = new RequestMessage(new UrlDetails("http://localhost:1234"), "GET", ClientIp);

        var responseBuilder = Response.Create()
            .WithBodyAsJson(new
            {
                Long = "{{#Random Type=\"Long\" Min=1000000000 Max=9999999999}}{{this}}{{/Random}}",
            })
            .WithTransformer();

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, Mock.Of<HttpContext>(), request, _settings);

        // Assert
        var j = JObject.FromObject(response.Message.BodyData!.BodyAsJson!);
        j["Long"]?.Value<long>().Should().BeInRange(1000000000, 9999999999);
    }
}