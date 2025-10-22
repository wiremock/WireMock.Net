// Copyright © WireMock.Net

using System.Threading.Tasks;
using Moq;
using NFluent;
using WireMock.Owin;
using WireMock.Owin.Mappers;
using Xunit;
#if NET452
using IContext = Microsoft.Owin.IOwinContext;
using IResponse = Microsoft.Owin.IOwinResponse;
#else
using IContext = Microsoft.AspNetCore.Http.HttpContext;
using IResponse = Microsoft.AspNetCore.Http.HttpResponse;
#endif

namespace WireMock.Net.Tests.Owin
{
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
            _responseMapperMock.Setup(m => m.MapAsync(It.IsAny<ResponseMessage?>(), It.IsAny<IResponse>())).Returns(Task.FromResult(true));

            _sut = new GlobalExceptionMiddleware(null, _optionsMock.Object, _responseMapperMock.Object);
        }

        [Fact]
        public void GlobalExceptionMiddleware_Invoke_NullAsNext_DoesNotInvokeNextAndDoesNotThrow()
        {
            // Act
            Check.ThatCode(() => _sut.Invoke(null)).DoesNotThrow();
        }
    }
}