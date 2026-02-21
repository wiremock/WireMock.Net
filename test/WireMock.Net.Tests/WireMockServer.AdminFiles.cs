// Copyright © WireMock.Net

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using AwesomeAssertions;
using Moq;
using WireMock.Handlers;
using WireMock.Server;
using WireMock.Settings;

namespace WireMock.Net.Tests;

public class WireMockServerAdminFilesTests
{
    private readonly HttpClient _client = new();

    [Fact]
    public async Task WireMockServer_Admin_Files_Post_Ascii()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
        filesystemHandlerMock.Setup(fs => fs.GetMappingFolder()).Returns("__admin/mappings");
        filesystemHandlerMock.Setup(fs => fs.FolderExists(It.IsAny<string>())).Returns(true);
        filesystemHandlerMock.Setup(fs => fs.WriteFile(It.IsAny<string>(), It.IsAny<byte[]>()));

        using var server = WireMockServer.Start(new WireMockServerSettings
        {
            UseSSL = false,
            StartAdminInterface = true,
            FileSystemHandler = filesystemHandlerMock.Object
        });

        var multipartFormDataContent = new MultipartFormDataContent();
        multipartFormDataContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
        multipartFormDataContent.Add(new StreamContent(new MemoryStream(Encoding.ASCII.GetBytes("Here's a string."))));

        // Act
        var httpResponseMessage = await _client.PostAsync("http://localhost:" + server.Ports[0] + "/__admin/files/filename.txt", multipartFormDataContent, cancellationToken);

        // Assert
        httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify
        filesystemHandlerMock.Verify(fs => fs.GetMappingFolder(), Times.Once);
        filesystemHandlerMock.Verify(fs => fs.FolderExists(It.IsAny<string>()), Times.Once);
        filesystemHandlerMock.Verify(fs => fs.WriteFile(It.Is<string>(p => p == "filename.txt"), It.IsAny<byte[]>()), Times.Once);
        filesystemHandlerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task WireMockServer_Admin_Files_Post_MappingFolderDoesNotExistsButWillBeCreated()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
        filesystemHandlerMock.Setup(fs => fs.GetMappingFolder()).Returns("x");
        filesystemHandlerMock.Setup(fs => fs.CreateFolder(It.IsAny<string>()));
        filesystemHandlerMock.Setup(fs => fs.FolderExists(It.IsAny<string>())).Returns(false);
        filesystemHandlerMock.Setup(fs => fs.WriteFile(It.IsAny<string>(), It.IsAny<byte[]>()));

        using var server = WireMockServer.Start(new WireMockServerSettings
        {
            UseSSL = false,
            StartAdminInterface = true,
            FileSystemHandler = filesystemHandlerMock.Object
        });

        var multipartFormDataContent = new MultipartFormDataContent();
        multipartFormDataContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
        multipartFormDataContent.Add(new StreamContent(new MemoryStream(Encoding.ASCII.GetBytes("Here's a string."))));

        // Act
        var httpResponseMessage = await _client.PostAsync("http://localhost:" + server.Ports[0] + "/__admin/files/filename.txt", multipartFormDataContent, cancellationToken);

        // Assert
        httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify
        filesystemHandlerMock.Verify(fs => fs.GetMappingFolder(), Times.Once);
        filesystemHandlerMock.Verify(fs => fs.FolderExists(It.IsAny<string>()), Times.Once);
        filesystemHandlerMock.Verify(fs => fs.CreateFolder(It.Is<string>(p => p == "x")), Times.Once);
        filesystemHandlerMock.Verify(fs => fs.WriteFile(It.Is<string>(p => p == "filename.txt"), It.IsAny<byte[]>()), Times.Once);
        filesystemHandlerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task WireMockServer_Admin_Files_GetAscii()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
        filesystemHandlerMock.Setup(fs => fs.FileExists(It.IsAny<string>())).Returns(true);
        filesystemHandlerMock.Setup(fs => fs.ReadFile(It.IsAny<string>())).Returns(Encoding.ASCII.GetBytes("Here's a string."));

        using var server = WireMockServer.Start(new WireMockServerSettings
        {
            UseSSL = false,
            StartAdminInterface = true,
            FileSystemHandler = filesystemHandlerMock.Object
        });

        var multipartFormDataContent = new MultipartFormDataContent();
        multipartFormDataContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
        multipartFormDataContent.Add(new StreamContent(new MemoryStream()));

        // Act
        var httpResponseMessageGet = await _client.GetAsync("http://localhost:" + server.Ports[0] + "/__admin/files/filename.txt", cancellationToken);

        // Assert
        httpResponseMessageGet.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await httpResponseMessageGet.Content.ReadAsStringAsync(cancellationToken);
        result.Should().Be("Here's a string.");

        // Verify
        filesystemHandlerMock.Verify(fs => fs.ReadFile(It.Is<string>(p => p == "filename.txt")), Times.Once);
        filesystemHandlerMock.Verify(fs => fs.FileExists(It.Is<string>(p => p == "filename.txt")), Times.Once);
        filesystemHandlerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task WireMockServer_Admin_Files_GetUTF16()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var symbol = Encoding.UTF32.GetBytes(char.ConvertFromUtf32(0x1D161));
        var filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
        filesystemHandlerMock.Setup(fs => fs.FileExists(It.IsAny<string>())).Returns(true);
        filesystemHandlerMock.Setup(fs => fs.ReadFile(It.IsAny<string>())).Returns(symbol);

        using var server = WireMockServer.Start(new WireMockServerSettings
        {
            UseSSL = false,
            StartAdminInterface = true,
            FileSystemHandler = filesystemHandlerMock.Object
        });

        var multipartFormDataContent = new MultipartFormDataContent();
        multipartFormDataContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
        multipartFormDataContent.Add(new StreamContent(new MemoryStream()));

        // Act
        var httpResponseMessageGet = await _client.GetAsync("http://localhost:" + server.Ports[0] + "/__admin/files/filename.bin", cancellationToken);

        // Assert
        httpResponseMessageGet.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await httpResponseMessageGet.Content.ReadAsByteArrayAsync(cancellationToken);
        result.Should().BeEquivalentTo(symbol);

        // Verify
        filesystemHandlerMock.Verify(fs => fs.ReadFile(It.Is<string>(p => p == "filename.bin")), Times.Once);
        filesystemHandlerMock.Verify(fs => fs.FileExists(It.Is<string>(p => p == "filename.bin")), Times.Once);
        filesystemHandlerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task WireMockServer_Admin_Files_Head()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
        filesystemHandlerMock.Setup(fs => fs.FileExists(It.IsAny<string>())).Returns(true);

        using var server = WireMockServer.Start(new WireMockServerSettings
        {
            UseSSL = false,
            StartAdminInterface = true,
            FileSystemHandler = filesystemHandlerMock.Object
        });

        // Act
        var requestUri = "http://localhost:" + server.Ports[0] + "/__admin/files/filename.txt";
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Head, requestUri);
        var httpResponseMessage = await _client.SendAsync(httpRequestMessage, cancellationToken);

        // Assert
        httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify
        filesystemHandlerMock.Verify(fs => fs.FileExists(It.IsAny<string>()), Times.Once);
        filesystemHandlerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task WireMockServer_Admin_Files_Head_FileDoesNotExistsReturns404()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;
        var filesystemHandlerMock = new Mock<IFileSystemHandler>(MockBehavior.Strict);
        filesystemHandlerMock.Setup(fs => fs.FileExists(It.IsAny<string>())).Returns(false);

        using var server = WireMockServer.Start(new WireMockServerSettings
        {
            UseSSL = false,
            StartAdminInterface = true,
            FileSystemHandler = filesystemHandlerMock.Object
        });

        // Act
        var requestUri = "http://localhost:" + server.Ports[0] + "/__admin/files/filename.txt";
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Head, requestUri);
        var httpResponseMessage = await _client.SendAsync(httpRequestMessage, cancellationToken);

        // Assert
        httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.NotFound);

        // Verify
        filesystemHandlerMock.Verify(fs => fs.FileExists(It.IsAny<string>()), Times.Once);
        filesystemHandlerMock.VerifyNoOtherCalls();
    }
}