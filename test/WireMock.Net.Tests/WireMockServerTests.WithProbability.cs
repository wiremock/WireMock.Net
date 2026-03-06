// Copyright © WireMock.Net

using System.Net;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace WireMock.Net.Tests;

public partial class WireMockServerTests
{
    [Fact]
    public async Task WireMockServer_WithProbability()
    {
        // Arrange
        var cancelationToken = TestContext.Current.CancellationToken;
        var server = WireMockServer.Start();
        server
            .Given(Request.Create().UsingGet().WithPath("/foo"))
            .WithProbability(0.5)
            .RespondWith(Response.Create().WithStatusCode(200));

        server
            .Given(Request.Create().UsingGet().WithPath("/foo"))
            .RespondWith(Response.Create().WithStatusCode(500));

        // Act
        var requestUri = new Uri($"http://localhost:{server.Port}/foo");
        var response = await server.CreateClient().GetAsync(requestUri, cancelationToken);

        // Assert
        Assert.Contains(response.StatusCode, new HashSet<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.InternalServerError });

        server.Stop();
    }
}