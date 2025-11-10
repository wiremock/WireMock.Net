// Copyright © WireMock.Net

using System.Threading.Tasks;
using Moq;
using NFluent;
using WireMock.Models;
using WireMock.ResponseBuilders;
using WireMock.Settings;
using Xunit;

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
        var response = await responseBuilder.ProvideResponseAsync(mapping, request, _settings);

        // Assert
        Check.That(response.Message).Equals(responseMessage);
    }
}