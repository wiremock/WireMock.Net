// Copyright © WireMock.Net

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using Newtonsoft.Json.Linq;
using WireMock.Handlers;
using WireMock.Models;
using WireMock.ResponseBuilders;
using WireMock.Settings;
using WireMock.Types;
using Xunit;

namespace WireMock.Net.Tests.ResponseBuilders;

public class ResponseWithHandlebarsDateTimeTests
{
    private const string ClientIp = "::1";
    private readonly WireMockServerSettings _settings = new();

    private readonly Mock<IMapping> _mappingMock;

    public ResponseWithHandlebarsDateTimeTests()
    {
        _mappingMock = new Mock<IMapping>();

        var filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
        filesystemHandlerMock.Setup(fs => fs.ReadResponseBodyAsString(It.IsAny<string>())).Returns("abc");

        _settings.FileSystemHandler = filesystemHandlerMock.Object;
    }

    [Theory]
    [InlineData(ReplaceNodeOptions.EvaluateAndTryToConvert, JTokenType.Integer)]
    [InlineData(ReplaceNodeOptions.Evaluate, JTokenType.String)]
    public async Task Response_WithBodyAsJson_ProvideResponseAsync_Handlebars_DateTimeYear(ReplaceNodeOptions options, JTokenType expected)
    {
        // Assign
        var request = new RequestMessage(new UrlDetails("http://localhost"), "GET", ClientIp);

        var responseBuilder = Response.Create()
            .WithBodyAsJson(new
            {
                DateTimeYear = "{{ DateTime.UtcNow \"yyyy\" }}"
            })
            .WithTransformer(options);

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, Mock.Of<HttpContext>(), request, _settings);

        // Assert
        var jObject = JObject.FromObject(response.Message.BodyData!.BodyAsJson!);
        jObject["DateTimeYear"]!.Type.Should().Be(expected);
        jObject["DateTimeYear"]!.Value<string>().Should().Be(DateTime.Now.Year.ToString());
    }

    [Theory]
    [InlineData(ReplaceNodeOptions.EvaluateAndTryToConvert, JTokenType.Date)]
    [InlineData(ReplaceNodeOptions.Evaluate, JTokenType.String)]
    public async Task Response_WithBodyAsJson_ProvideResponseAsync_Handlebars_DateTime(ReplaceNodeOptions options, JTokenType expected)
    {
        // Assign
        var request = new RequestMessage(new UrlDetails("http://localhost"), "GET", ClientIp);

        var responseBuilder = Response.Create()
            .WithBodyAsJson(new
            {
                DateTime = "{{ DateTime.UtcNow }}"
            })
            .WithTransformer(options);

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, Mock.Of<HttpContext>(), request, _settings);

        // Assert
        var jObject = JObject.FromObject(response.Message.BodyData!.BodyAsJson!);
        jObject["DateTime"]!.Type.Should().Be(expected);
    }

    [Theory]
    [InlineData(ReplaceNodeOptions.EvaluateAndTryToConvert, JTokenType.Integer)]
    [InlineData(ReplaceNodeOptions.Evaluate, JTokenType.String)]
    public async Task Response_WithBodyAsJson_ProvideResponseAsync_Handlebars_DateTimeWithStringFormat(ReplaceNodeOptions options, JTokenType expected)
    {
        // Assign
        var request = new RequestMessage(new UrlDetails("http://localhost"), "GET", ClientIp);

        var responseBuilder = Response.Create()
            .WithBodyAsJson(new
            {
                StringFormatDateTime = "{{ String.Format (DateTime.UtcNow) \"yyMMddhhmmss\" }}"
            })
            .WithTransformer(options);

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, Mock.Of<HttpContext>(), request, _settings);

        // Assert
        var jObject = JObject.FromObject(response.Message.BodyData!.BodyAsJson!);
        jObject["StringFormatDateTime"]!.Type.Should().Be(expected);
    }

    [Fact]
    public async Task Response_WithBody_ProvideResponseAsync_Handlebars_DateTime()
    {
        // Assign
        var request = new RequestMessage(new UrlDetails("http://localhost"), "GET", ClientIp);

        var responseBuilder = Response.Create()
            .WithBody("{\nDateTimeYear = \"{{ DateTime.UtcNow \"yyyy\" }}\"")
            .WithTransformer();

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, Mock.Of<HttpContext>(), request, _settings);

        // Assert
        response.Message.BodyData!.BodyAsString.Should().Contain($"DateTimeYear = \"{DateTime.Now.Year}\"");
    }

    [Theory]
    [InlineData(ReplaceNodeOptions.EvaluateAndTryToConvert)]
    [InlineData(ReplaceNodeOptions.Evaluate)]
    public async Task Response_WithBodyAsJson_ProvideResponseAsync_Handlebars_WithStringFormatAsString(ReplaceNodeOptions options)
    {
        // Assign
        var request = new RequestMessage(new UrlDetails("http://localhost"), "GET", ClientIp);

        var responseBuilder = Response.Create()
            .WithBodyAsJson(new
            {
                FormatAsString1 = "{{ String.FormatAsString (DateTime.UtcNow) \"yyMMddhhmmss\" }}",
                FormatAsString2 = "{{ String.FormatAsString (DateTime.UtcNow) }}",
                FormatAsString3 = "{{ String.FormatAsString 42 \"X\" }}",
                FormatAsString4 = "{{ String.FormatAsString 42 }}"
            })
            .WithTransformer(options);

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, Mock.Of<HttpContext>(), request, _settings);

        // Assert
        var jObject = JObject.FromObject(response.Message.BodyData!.BodyAsJson!);
        jObject["FormatAsString1"]!.Type.Should().Be(JTokenType.String);
        jObject["FormatAsString2"]!.Type.Should().Be(JTokenType.String);

        var formatAsString3 = jObject["FormatAsString3"]!;
        formatAsString3.Type.Should().Be(JTokenType.String);
        formatAsString3.Value<string>().Should().Be("2A");

        var formatAsString4 = jObject["FormatAsString4"]!;
        formatAsString4.Type.Should().Be(JTokenType.String);
        formatAsString4.Value<string>().Should().Be("42");
    }
}