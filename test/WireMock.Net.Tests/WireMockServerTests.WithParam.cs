// Copyright © WireMock.Net

using System.Net;
using AwesomeAssertions;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.Server;
using WireMock.Settings;
using WireMock.Types;

namespace WireMock.Net.Tests;

public partial class WireMockServerTests
{
    [Theory]
    [InlineData("SELECT id, value FROM table WHERE id = 1")]
    [InlineData("select id, name, value from table where id in (1, 2, 3, 4, 5)")]
    [InlineData("1,2,3")]
    public async Task WireMockServer_WithParam_QueryParameterMultipleValueSupport_NoComma_Should_Ignore_Comma(string queryValue)
    {
        // Arrange
        var cancelationToken = TestContext.Current.CancellationToken;
        var settings = new WireMockServerSettings
        {
            QueryParameterMultipleValueSupport = QueryParameterMultipleValueSupport.NoComma
        };
        var server = WireMockServer.Start(settings);
        server
            .WhenRequest(r => r
                .UsingGet()
                .WithPath("/foo")
                .WithParam("query", queryValue)
            )
            .ThenRespondWith(r => r
                .WithStatusCode(HttpStatusCode.Accepted)
            );

        // Act
        var requestUri = new Uri($"http://localhost:{server.Port}/foo?query={queryValue}");
        var response = await server.CreateClient().GetAsync(requestUri, cancelationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);

        server.Stop();
    }

    [Fact]
    public async Task WireMockServer_WithParam_MultiValueComma()
    {
        // Arrange
        var cancelationToken = TestContext.Current.CancellationToken;
        var queryValue = "1,2,3";
        var server = WireMockServer.Start();
        server
            .WhenRequest(r => r
                .UsingGet()
                .WithPath("/foo")
                .WithParam("query", "1", "2", "3")
            )
            .ThenRespondWithStatusCode(200);

        // Act
        var requestUri = new Uri($"http://localhost:{server.Port}/foo?query={queryValue}");
        var response = await server.CreateClient().GetAsync(requestUri, cancelationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        server.Stop();
    }

    [Fact]
    public async Task WireMockServer_WithParam_RejectOnMatch_OnNonMatchingParam_ShouldReturnMappingOk()
    {
        // Arrange
        var cancelationToken = TestContext.Current.CancellationToken;
        var server = WireMockServer.Start();
        server.Given(
            Request.Create()
                .WithPath("/v1/person/workers")
                .WithParam("delta_from", MatchBehaviour.RejectOnMatch)
                .UsingGet()
            )
            .ThenRespondWithOK();

        // Act
        var requestUri = new Uri($"http://localhost:{server.Port}/v1/person/workers?showsourcesystem=true&count=700&page=1&sections=personal%2Corganizations%2Cemployment");
        var response = await server.CreateClient().GetAsync(requestUri, cancelationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        server.Stop();
    }

    [Fact]
    public async Task WireMockServer_WithParam_AcceptOnMatch_OnNonMatchingParam_ShouldReturnMappingOk()
    {
        // Arrange
        var cancelationToken = TestContext.Current.CancellationToken;
        var server = WireMockServer.Start();
        server.Given(
            Request.Create()
                .WithPath("/v1/person/workers")
                .WithParam("delta_from")
                .UsingGet()
            )
            .ThenRespondWithStatusCode("300");

        // Act
        var requestUri = new Uri($"http://localhost:{server.Port}/v1/person/workers?showsourcesystem=true&count=700&page=1&sections=personal%2Corganizations%2Cemployment");
        var response = await server.CreateClient().GetAsync(requestUri, cancelationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        server.Stop();
    }
}