// Copyright © WireMock.Net

using Microsoft.AspNetCore.Http;
using Moq;

using WireMock.Models;
using WireMock.ResponseBuilders;
using WireMock.Settings;

namespace WireMock.Net.Tests.ResponseBuilders;

public class ResponseCreateTests
{
    private readonly WireMockServerSettings _settings = new ();

    [Fact]
    public async Task Response_Create_Func()
    {
        // Assign
        var responseMessage = new ResponseMessage { StatusCode = 500 };
        var request = new RequestMessage(new UrlDetails("http://localhost"), "GET", "::1");
        var mapping = new Mock<IMapping>().Object;

        var responseBuilder = Response.Create(() => responseMessage);

        // Act
        var response = await responseBuilder.ProvideResponseAsync(mapping, Mock.Of<HttpContext>(), request, _settings);

        // Assert
        response.Message.Should().Be(responseMessage);
    }
}
