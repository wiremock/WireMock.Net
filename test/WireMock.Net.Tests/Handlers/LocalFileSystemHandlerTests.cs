// Copyright © WireMock.Net

using NFluent;
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
        Check.That(result).EndsWith(Path.Combine("__admin", "mappings"));
    }

    [Fact]
    public void LocalFileSystemHandler_CreateFolder_ThrowsArgumentNullException()
    {
        // Act
        Check.ThatCode(() => _sut.CreateFolder(null)).Throws<ArgumentNullException>();
    }

    [Fact]
    public void LocalFileSystemHandler_WriteMappingFile_ThrowsArgumentNullException()
    {
        // Act
        Check.ThatCode(() => _sut.WriteMappingFile(null, null)).Throws<ArgumentNullException>();
    }

    [Fact]
    public void LocalFileSystemHandler_ReadResponseBodyAsFile_ThrowsArgumentNullException()
    {
        // Act
        Check.ThatCode(() => _sut.ReadResponseBodyAsFile(null)).Throws<ArgumentNullException>();
    }

    [Fact]
    public void LocalFileSystemHandler_FileExists_ReturnsFalse()
    {
        // Act
        var result = _sut.FileExists("x.x");

        // Assert
        Check.That(result).IsFalse();
    }

    [Fact]
    public void LocalFileSystemHandler_FileExists_ThrowsArgumentNullException()
    {
        // Act
        Check.ThatCode(() => _sut.FileExists(null)).Throws<ArgumentNullException>();
    }

    [Fact]
    public void LocalFileSystemHandler_ReadFile_ThrowsArgumentNullException()
    {
        // Act
        Check.ThatCode(() => _sut.ReadFile(null)).Throws<ArgumentNullException>();
    }

    [Fact]
    public void LocalFileSystemHandler_ReadFileAsString_ThrowsArgumentNullException()
    {
        // Act
        Check.ThatCode(() => _sut.ReadFileAsString(null)).Throws<ArgumentNullException>();
    }

    [Fact]
    public void LocalFileSystemHandler_WriteFile_ThrowsArgumentNullException()
    {
        // Act
        Check.ThatCode(() => _sut.WriteFile(null, null)).Throws<ArgumentNullException>();
    }

    [Fact]
    public void LocalFileSystemHandler_DeleteFile_ThrowsArgumentNullException()
    {
        // Act
        Check.ThatCode(() => _sut.DeleteFile(null)).Throws<ArgumentNullException>();
    }

    [Fact]
    public void LocalFileSystemHandler_GetUnmatchedRequestsFolder()
    {
        // Act
        string result = _sut.GetUnmatchedRequestsFolder();

        // Assert
        Check.That(result).EndsWith(Path.Combine("requests", "unmatched"));
    }

    [Fact]
    public void LocalFileSystemHandler_WriteUnmatchedRequest()
    {
        // Act
        Check.ThatCode(() => _sut.WriteUnmatchedRequest(null, null)).Throws<ArgumentNullException>();
    }
}