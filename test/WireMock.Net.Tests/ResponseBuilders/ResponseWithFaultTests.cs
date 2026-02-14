// Copyright © WireMock.Net

using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using WireMock.Models;
using WireMock.ResponseBuilders;
using WireMock.Settings;

namespace WireMock.Net.Tests.ResponseBuilders;

public class ResponseWithFaultTests
{
    private readonly WireMockServerSettings _settings = new();
    private const string ClientIp = "::1";

    private readonly Mock<IMapping> _mappingMock;

    public ResponseWithFaultTests()
    {
        _mappingMock = new Mock<IMapping>();
    }

    [Theory]
    [InlineData(FaultType.EMPTY_RESPONSE)]
    [InlineData(FaultType.MALFORMED_RESPONSE_CHUNK)]
    public async Task Response_ProvideResponse_WithFault(FaultType faultType)
    {
        // Arrange
        var request = new RequestMessage(new UrlDetails("http://localhost/fault"), "GET", ClientIp);

        // Act
        var responseBuilder = Response.Create().WithFault(faultType);
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, Mock.Of<HttpContext>(), request, _settings);

        // Assert
        response.Message.FaultType.Should().Be(faultType);
        response.Message.FaultPercentage.Should().BeNull();
    }

    [Theory]
    [InlineData(FaultType.EMPTY_RESPONSE, 0.5)]
    public async Task Response_ProvideResponse_WithFault_IncludingPercentage(FaultType faultType, double percentage)
    {
        // Arrange
        var request = new RequestMessage(new UrlDetails("http://localhost/fault"), "GET", ClientIp);

        // Act
        var responseBuilder = Response.Create().WithFault(faultType, percentage);
        var response = await responseBuilder.ProvideResponseAsync(_mappingMock.Object, Mock.Of<HttpContext>(), request, _settings);

        // Assert
        response.Message.FaultType.Should().Be(faultType);
        response.Message.FaultPercentage.Should().Be(percentage);
    }
}