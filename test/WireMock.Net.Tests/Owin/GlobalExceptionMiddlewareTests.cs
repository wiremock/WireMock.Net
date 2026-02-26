// Copyright © WireMock.Net

using Microsoft.AspNetCore.Http;
using Moq;
using WireMock.Owin;
using WireMock.Owin.Mappers;

namespace WireMock.Net.Tests.Owin;

public class GlobalExceptionMiddlewareTests
{
    private readonly Mock<IWireMockMiddlewareOptions> _optionsMock;
    private readonly Mock<IOwinResponseMapper> _responseMapperMock;

    private readonly GlobalExceptionMiddleware _sut;

    public GlobalExceptionMiddlewareTests()
    {
        _optionsMock = new Mock<IWireMockMiddlewareOptions>();
        _optionsMock.SetupAllProperties();

        _responseMapperMock = new Mock<IOwinResponseMapper>();
        _responseMapperMock.SetupAllProperties();
        _responseMapperMock.Setup(m => m.MapAsync(It.IsAny<ResponseMessage?>(), It.IsAny<HttpResponse>())).Returns(Task.FromResult(true));

        _sut = new GlobalExceptionMiddleware(null, _optionsMock.Object, _responseMapperMock.Object);
    }

    [Fact]
    public void GlobalExceptionMiddleware_Invoke_NullAsNext_DoesNotInvokeNextAndDoesNotThrow()
    {
        // Act
        Action act = () => _sut.Invoke(null);
        act.Should().NotThrow();
    }
}