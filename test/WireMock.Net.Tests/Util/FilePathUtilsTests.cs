// Copyright © WireMock.Net

using NFluent;
using WireMock.Util;

namespace WireMock.Net.Tests.Util;

public class FilePathUtilsTests
{
    [Theory]
    [InlineData(@"subdirectory/MyXmlResponse.xml")]
    [InlineData(@"subdirectory\MyXmlResponse.xml")]
    public void PathUtils_CleanPath(string path)
    {
        // Act
        var cleanPath = FilePathUtils.CleanPath(path);

        // Assert
        Check.That(cleanPath).Equals("subdirectory" + Path.DirectorySeparatorChar + "MyXmlResponse.xml");
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("", "")]
    [InlineData("a", "a")]
    [InlineData(@"/", "")]
    [InlineData(@"//", "")]
    [InlineData(@"//a", "a")]
    [InlineData(@"\", "")]
    [InlineData(@"\\", "")]
    [InlineData(@"\\a", "a")]
    public void PathUtils_CleanPath_RemoveLeadingDirectorySeparators(string? path, string? expected)
    {
        // Arrange
        var cleanPath = FilePathUtils.CleanPath(path);

        // Act
        var withoutDirectorySeparators = FilePathUtils.RemoveLeadingDirectorySeparators(cleanPath);

        // Assert
        Check.That(withoutDirectorySeparators).Equals(expected);
    }
}