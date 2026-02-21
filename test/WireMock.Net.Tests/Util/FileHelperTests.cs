// Copyright © WireMock.Net

using AwesomeAssertions;
using Moq;
using WireMock.Handlers;
using WireMock.Util;

namespace WireMock.Net.Tests.Util;

public class FileHelperTests
{
    [Fact]
    public void TryReadMappingFileWithRetryAndDelay_WithIFileSystemHandlerOk_ReturnsTrue()
    {
        // Assign
        var staticMappingHandlerMock = new Mock<IFileSystemHandler>();
        staticMappingHandlerMock.Setup(m => m.ReadMappingFile(It.IsAny<string>())).Returns("text");

        // Act
        bool result = FileHelper.TryReadMappingFileWithRetryAndDelay(staticMappingHandlerMock.Object, @"c:\temp", out var value);

        // Assert
        result.Should().BeTrue();
        value.Should().Be("text");

        // Verify
        staticMappingHandlerMock.Verify(m => m.ReadMappingFile(@"c:\temp"), Times.Once);
    }

    [Fact]
    public void TryReadMappingFileWithRetryAndDelay_WithIFileSystemHandlerThrows_ReturnsFalse()
    {
        // Assign
        var staticMappingHandlerMock = new Mock<IFileSystemHandler>();
        staticMappingHandlerMock.Setup(m => m.ReadMappingFile(It.IsAny<string>())).Throws<NotSupportedException>();

        // Act
        bool result = FileHelper.TryReadMappingFileWithRetryAndDelay(staticMappingHandlerMock.Object, @"c:\temp", out var value);

        // Assert
        result.Should().BeFalse();
        value.Should().BeNull();

        // Verify
        staticMappingHandlerMock.Verify(m => m.ReadMappingFile(@"c:\temp"), Times.Exactly(3));
    }
}