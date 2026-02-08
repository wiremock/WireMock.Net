// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using NFluent;
using WireMock.Models;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;
using WireMock.Types;
using WireMock.Util;
using Xunit;

namespace WireMock.Net.Tests.ResponseBuilders;

public class ResponseWithProxyTests : IDisposable
{
    private const string ClientIp = "::1";
    private readonly WireMockServerSettings _settings = new();
    private readonly WireMockServer _server;
    private readonly Guid _guid;

    private readonly Mock<IMapping> _mappingMock;

    public ResponseWithProxyTests()
    {
        _mappingMock = new Mock<IMapping>();

        _guid = Guid.NewGuid();

        _server = WireMockServer.Start();
        _server.Given(Request.Create().UsingPost().WithPath($"/{_guid}"))
            .RespondWith(Response.Create().WithStatusCode(201).WithBodyAsJson(new { p = 42 }).WithHeader("Content-Type", "application/json"));
        _server.Given(Request.Create().UsingPost().WithPath($"/{_guid}/append"))
            .RespondWith(Response.Create().WithStatusCode(201).WithBodyAsJson(new { p = 10 }).WithHeader("Content-Type", "application/json"));
        _server.Given(Request.Create().UsingPost().WithPath($"/prepend/{_guid}"))
            .RespondWith(Response.Create().WithStatusCode(201).WithBodyAsJson(new { p = 11 }).WithHeader("Content-Type", "application/json"));
        _server.Given(Request.Create().UsingPost().WithPath($"/prepend/{_guid}/append"))
            .RespondWith(Response.Create().WithStatusCode(201).WithBodyAsJson(new { p = 12 }).WithHeader("Content-Type", "application/json"));
    }

    [Theory]
    [InlineData("", "", "{\"p\":42}")]
    [InlineData("", "/append", "{\"p\":10}")]
    [InlineData("/prepend", "", "{\"p\":11}")]
    [InlineData("/prepend", "/append", "{\"p\":12}")]
    public async Task Response_WithProxy(string prepend, string append, string expectedBody)
    {
        // Assign
        var headers = new Dictionary<string, string[]> { { "Content-Type", new[] { "application/xml" } } };
        var request = new RequestMessage(new UrlDetails($"{_server.Urls[0]}{prepend}/{_guid}{append}"), "POST", ClientIp, new BodyData { DetectedBodyType = BodyType.Json, BodyAsJson = new { a = 1 } }, headers);
        var responseBuilder = Response.Create().WithProxy(_server.Urls[0]);

        // Act
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, Mock.Of<HttpContext>(), request, _settings);

        // Assert
        Check.That(request.ProxyUrl).IsNotNull();
        Check.That(response.Message.BodyData.BodyAsString).IsEqualTo(expectedBody);
        Check.That(response.Message.StatusCode).IsEqualTo(201);
        Check.That(response.Message.Headers["Content-Type"].ToString()).IsEqualTo("application/json");
    }

    [Fact]
    public void Response_WithProxy_WebProxySettings()
    {
        // Assign
        var settings = new ProxyAndRecordSettings
        {
            Url = "http://test.nl",
            WebProxySettings = new WebProxySettings
            {
                Address = "http://company",
                UserName = "x",
                Password = "y"
            }
        };
        var responseBuilder = Response.Create().WithProxy(settings);

        // Act
        var request = new RequestMessage(new UrlDetails($"{_server.Urls[0]}/{_guid}"), "GET", ClientIp);

        Check.ThatCode(() => responseBuilder.ProvideResponseAsync(_mappingMock.Object, Mock.Of<HttpContext>(), request, _settings)).Throws<HttpRequestException>();
    }

    public void Dispose()
    {
        _server?.Stop();
        _server?.Dispose();
    }
}