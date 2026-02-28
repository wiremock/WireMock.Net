// Copyright © WireMock.Net

using Microsoft.AspNetCore.Http;
using Moq;
using WireMock.Logging;
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
        _optionsMock.SetupGet(o => o.Logger).Returns(Mock.Of<IWireMockLogger>());

        _responseMapperMock = new Mock<IOwinResponseMapper>();
        _responseMapperMock.Setup(m => m.MapAsync(It.IsAny<ResponseMessage?>(), It.IsAny<HttpResponse>())).Returns(Task.FromResult(true));

        _sut = new GlobalExceptionMiddleware(_ => Task.CompletedTask, _optionsMock.Object, _responseMapperMock.Object);
    }

    [Fact]
    public void GlobalExceptionMiddleware_Invoke_ValidNext_ShouldNotThrow()
    {
        // Act
        _sut.Invoke(Mock.Of<HttpContext>());
    }

    [Fact]
    public void GlobalExceptionMiddleware_Invoke_InvalidNext_ShouldCallResponseMapperWith500()
    {
        // Arrange
        var sut = new GlobalExceptionMiddleware(_ => throw new ArgumentException(), _optionsMock.Object, _responseMapperMock.Object);

        // Act
        sut.Invoke(Mock.Of<HttpContext>());

        // Verify
        _responseMapperMock.Verify(m => m.MapAsync(It.IsAny<ResponseMessage>(), It.IsAny<HttpResponse>()), Times.Once);
        _responseMapperMock.VerifyNoOtherCalls();
    }
}