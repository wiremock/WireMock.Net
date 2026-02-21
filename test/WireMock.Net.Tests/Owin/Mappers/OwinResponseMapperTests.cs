// Copyright © WireMock.Net

using Moq;
using AwesomeAssertions;
using WireMock.Handlers;
using WireMock.Owin.Mappers;
using WireMock.ResponseBuilders;
using WireMock.Types;
using WireMock.Util;
using WireMock.Owin;
#if NET452
using Microsoft.Owin;
using IResponse = Microsoft.Owin.IOwinResponse;
// using Response = Microsoft.Owin.OwinResponse;
#else
using Microsoft.AspNetCore.Http;
using IResponse = Microsoft.AspNetCore.Http.HttpResponse;
// using Response = Microsoft.AspNetCore.Http.HttpResponse;
using Microsoft.Extensions.Primitives;
#endif

namespace WireMock.Net.Tests.Owin.Mappers;

public class OwinResponseMapperTests
{
    private static readonly Task CompletedTask = Task.FromResult(true);
    private readonly OwinResponseMapper _sut;
    private readonly Mock<IResponse> _responseMock;
    private readonly Mock<Stream> _stream;
    private readonly Mock<IHeaderDictionary> _headers;
    private readonly Mock<IFileSystemHandler> _fileSystemHandlerMock;
    private readonly Mock<IWireMockMiddlewareOptions> _optionsMock;

    public OwinResponseMapperTests()
    {
        _stream = new Mock<Stream>();
        _stream.SetupAllProperties();
        _stream.Setup(s => s.WriteAsync(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(CompletedTask);

        _fileSystemHandlerMock = new Mock<IFileSystemHandler>();
        _fileSystemHandlerMock.SetupAllProperties();

        _optionsMock = new Mock<IWireMockMiddlewareOptions>();
        _optionsMock.SetupAllProperties();
        _optionsMock.SetupGet(o => o.FileSystemHandler).Returns(_fileSystemHandlerMock.Object);

        _headers = new Mock<IHeaderDictionary>();
        _headers.SetupAllProperties();
#if NET452
        _headers.Setup(h => h.AppendValues(It.IsAny<string>(), It.IsAny<string[]>()));
#else
        _headers.Setup(h => h.Add(It.IsAny<string>(), It.IsAny<StringValues>()));
#endif

        _responseMock = new Mock<IResponse>();
        _responseMock.SetupAllProperties();
        _responseMock.SetupGet(r => r.Body).Returns(_stream.Object);
        _responseMock.SetupGet(r => r.Headers).Returns(_headers.Object);

        _sut = new OwinResponseMapper(_optionsMock.Object);
    }

    [Fact]
    public async Task OwinResponseMapper_MapAsync_Null()
    {
        // Act
        await _sut.MapAsync(null, _responseMock.Object);
    }

    [Theory]
    [InlineData(300, 300)]
    [InlineData(500, 500)]
    public async Task OwinResponseMapper_MapAsync_Valid_StatusCode(object code, int expected)
    {
        // Arrange
        var responseMessage = new ResponseMessage
        {
            StatusCode = code
        };

        // Act
        await _sut.MapAsync(responseMessage, _responseMock.Object);

        // Assert
        _responseMock.VerifySet(r => r.StatusCode = expected, Times.Once);
    }

    [Theory]
    [InlineData(0, 200)]
    [InlineData(-1, 200)]
    [InlineData(10000, 200)]
    [InlineData(300, 300)]
    public async Task OwinResponseMapper_MapAsync_Invalid_StatusCode_When_AllowOnlyDefinedHttpStatusCodeInResponseSet_Is_True(object code, int expected)
    {
        // Arrange
        _optionsMock.SetupGet(o => o.AllowOnlyDefinedHttpStatusCodeInResponse).Returns(true);
        var responseMessage = new ResponseMessage
        {
            StatusCode = code
        };

        // Act
        await _sut.MapAsync(responseMessage, _responseMock.Object);

        // Assert
        _responseMock.VerifySet(r => r.StatusCode = expected, Times.Once);
    }

    [Fact]
    public async Task OwinResponseMapper_MapAsync_StatusCode_Is_Null()
    {
        // Arrange
        var responseMessage = new ResponseMessage
        {
            StatusCode = null
        };

        // Act
        await _sut.MapAsync(responseMessage, _responseMock.Object);

        // Assert
        _responseMock.VerifySet(r => r.StatusCode = It.IsAny<int>(), Times.Never);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(-1, -1)]
    [InlineData(10000, 10000)]
    [InlineData(300, 300)]
    public async Task OwinResponseMapper_MapAsync_StatusCode_Is_NotInEnumRange(object code, int expected)
    {
        // Arrange
        var responseMessage = new ResponseMessage
        {
            StatusCode = code
        };

        // Act
        await _sut.MapAsync(responseMessage, _responseMock.Object);

        // Assert
        _responseMock.VerifySet(r => r.StatusCode = expected, Times.Once);
    }

    [Fact]
    public async Task OwinResponseMapper_MapAsync_NoBody()
    {
        // Arrange
        var responseMessage = new ResponseMessage
        {
            Headers = new Dictionary<string, WireMockList<string>>()
        };

        // Act
        await _sut.MapAsync(responseMessage, _responseMock.Object);

        // Assert
        _stream.Verify(s => s.WriteAsync(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task OwinResponseMapper_MapAsync_Body()
    {
        // Arrange
        string body = "abcd";
        var responseMessage = new ResponseMessage
        {
            Headers = new Dictionary<string, WireMockList<string>>(),
            BodyData = new BodyData { DetectedBodyType = BodyType.String, BodyAsString = body }
        };

        // Act
        await _sut.MapAsync(responseMessage, _responseMock.Object);

        // Assert
        _stream.Verify(s => s.WriteAsync(new byte[] { 97, 98, 99, 100 }, 0, 4, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task OwinResponseMapper_MapAsync_BodyAsBytes()
    {
        // Arrange
        var bytes = new byte[] { 48, 49 };
        var responseMessage = new ResponseMessage
        {
            Headers = new Dictionary<string, WireMockList<string>>(),
            BodyData = new BodyData { DetectedBodyType = BodyType.Bytes, BodyAsBytes = bytes }
        };

        // Act
        await _sut.MapAsync(responseMessage, _responseMock.Object);

        // Assert
        _stream.Verify(s => s.WriteAsync(bytes, 0, bytes.Length, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task OwinResponseMapper_MapAsync_BodyAsJson()
    {
        // Arrange
        var json = new { t = "x", i = (string?)null };
        var responseMessage = new ResponseMessage
        {
            Headers = new Dictionary<string, WireMockList<string>>(),
            BodyData = new BodyData { DetectedBodyType = BodyType.Json, BodyAsJson = json, BodyAsJsonIndented = false }
        };

        // Act
        await _sut.MapAsync(responseMessage, _responseMock.Object);

        // Assert
        _stream.Verify(s => s.WriteAsync(new byte[] { 123, 34, 116, 34, 58, 34, 120, 34, 125 }, 0, 9, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task OwinResponseMapper_MapAsync_SetResponseHeaders()
    {
        // Arrange
        var responseMessage = new ResponseMessage
        {
            Headers = new Dictionary<string, WireMockList<string>> { { "h", new WireMockList<string>("x", "y") } }
        };

        // Act
        await _sut.MapAsync(responseMessage, _responseMock.Object);

        // Assert
#if NET452
        _headers.Verify(h => h.AppendValues("h", new string[] { "x", "y" }), Times.Once);
#else
        var v = new StringValues();
        _headers.Verify(h => h.TryGetValue("h", out v), Times.Once);
#endif
    }

    [Fact]
    public void OwinResponseMapper_MapAsync_BodyAsFile_ThrowsException()
    {
        // Arrange
        var responseMessage = new ResponseMessage
        {
            Headers = new Dictionary<string, WireMockList<string>>(),
            BodyData = new BodyData { DetectedBodyType = BodyType.File, BodyAsFile = string.Empty }
        };
        _fileSystemHandlerMock.Setup(f => f.ReadResponseBodyAsFile(It.IsAny<string>())).Throws<FileNotFoundException>();

        // Act
        Func<Task> action = () => _sut.MapAsync(responseMessage, _responseMock.Object);

        // Assert
        action.Should().ThrowAsync<FileNotFoundException>();
    }

    [Fact]
    public async Task OwinResponseMapper_MapAsync_WithFault_EMPTY_RESPONSE()
    {
        // Arrange
        string body = "abc";
        var responseMessage = new ResponseMessage
        {
            Headers = new Dictionary<string, WireMockList<string>>(),
            BodyData = new BodyData { DetectedBodyType = BodyType.String, BodyAsString = body },
            FaultType = FaultType.EMPTY_RESPONSE
        };

        // Act
        await _sut.MapAsync(responseMessage, _responseMock.Object);

        // Assert
        _stream.Verify(s => s.WriteAsync(new byte[0], 0, 0, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData("abcd", BodyType.String)]
    [InlineData("", BodyType.String)]
    [InlineData(null, BodyType.None)]
    public async Task OwinResponseMapper_MapAsync_WithFault_MALFORMED_RESPONSE_CHUNK(string? body, BodyType detected)
    {
        // Arrange
        var responseMessage = new ResponseMessage
        {
            Headers = new Dictionary<string, WireMockList<string>>(),
            BodyData = new BodyData { DetectedBodyType = detected, BodyAsString = body },
            StatusCode = 100,
            FaultType = FaultType.MALFORMED_RESPONSE_CHUNK
        };

        // Act
        await _sut.MapAsync(responseMessage, _responseMock.Object);

        // Assert
        _responseMock.VerifySet(r => r.StatusCode = 100, Times.Once);
        _stream.Verify(s => s.WriteAsync(It.IsAny<byte[]>(), 0, It.Is<int>(count => count >= 0), It.IsAny<CancellationToken>()), Times.Once);
    }
}