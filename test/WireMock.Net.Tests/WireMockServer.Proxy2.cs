// Copyright © WireMock.Net

using System.Net;
using System.Net.Http;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace WireMock.Net.Tests;

public class WireMockServerProxy2Tests
{
    private readonly CancellationToken _ct = TestContext.Current.CancellationToken;

    [Fact]
    public async Task WireMockServer_ProxyAndRecordSettings_ShouldProxy()
    {
        // Assign
        var serverAsProxy = WireMockServer.Start();
        serverAsProxy.Given(Request.Create().UsingPost())
            .RespondWith(Response.Create().WithStatusCode(201).WithBodyAsJson(new { p = 42 }).WithHeader("Content-Type", "application/json"));

        // Act
        var server = WireMockServer.Start();
        server.Given(Request.Create().UsingPost().WithHeader("prx", "1"))
            .RespondWith(Response.Create().WithProxy(serverAsProxy.Urls[0]));

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"{server.Urls[0]}/TST"),
            Content = new StringContent("test")
        };
        request.Headers.Add("prx", "1");

        // Assert
        using var httpClient = new HttpClient();
        var response = await httpClient.SendAsync(request, _ct);
        string content = await response.Content.ReadAsStringAsync(_ct);

        content.Should().Be("{\"p\":42}");
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Content.Headers.GetValues("Content-Type").First().Should().Be("application/json");

        server.Dispose();
        serverAsProxy.Dispose();
    }
}


