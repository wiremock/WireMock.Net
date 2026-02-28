// Copyright © WireMock.Net


using WireMock.Handlers;

namespace WireMock.Net.Tests.Handlers;

public class LocalFileSystemHandlerTests
{
    private readonly LocalFileSystemHandler _sut = new();

    [Fact]
    public void LocalFileSystemHandler_GetMappingFolder()
    {
        // Act
        string result = _sut.GetMappingFolder();

        // Assert
        result.Should().EndWith(Path.Combine("__admin", "mappings"));
    }

    [Fact]
    public void LocalFileSystemHandler_CreateFolder_ThrowsArgumentNullException()
    {
        // Act
        Action act = () => _sut.CreateFolder(null);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void LocalFileSystemHandler_WriteMappingFile_ThrowsArgumentNullException()
    {
        // Act
        Action act = () => _sut.WriteMappingFile(null, null);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void LocalFileSystemHandler_ReadResponseBodyAsFile_ThrowsArgumentNullException()
    {
        // Act
        Action act = () => _sut.ReadResponseBodyAsFile(null);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void LocalFileSystemHandler_FileExists_ReturnsFalse()
    {
        // Act
        var result = _sut.FileExists("x.x");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void LocalFileSystemHandler_FileExists_ThrowsArgumentNullException()
    {
        // Act
        Action act = () => _sut.FileExists(null);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void LocalFileSystemHandler_ReadFile_ThrowsArgumentNullException()
    {
        // Act
        Action act = () => _sut.ReadFile(null);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void LocalFileSystemHandler_ReadFileAsString_ThrowsArgumentNullException()
    {
        // Act
        Action act = () => _sut.ReadFileAsString(null);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void LocalFileSystemHandler_WriteFile_ThrowsArgumentNullException()
    {
        // Act
        Action act = () => _sut.WriteFile(null, null);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void LocalFileSystemHandler_DeleteFile_ThrowsArgumentNullException()
    {
        // Act
        Action act = () => _sut.DeleteFile(null);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void LocalFileSystemHandler_GetUnmatchedRequestsFolder()
    {
        // Act
        string result = _sut.GetUnmatchedRequestsFolder();

        // Assert
        result.Should().EndWith(Path.Combine("requests", "unmatched"));
    }

    [Fact]
    public void LocalFileSystemHandler_WriteUnmatchedRequest()
    {
        // Act
        Action act = () => _sut.WriteUnmatchedRequest(null, null);
        act.Should().Throw<ArgumentNullException>();
    }
}



