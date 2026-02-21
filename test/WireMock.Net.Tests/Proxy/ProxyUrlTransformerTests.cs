using System.Globalization;
using Moq;
using WireMock.Handlers;
using WireMock.Proxy;
using WireMock.Settings;
using WireMock.Types;

namespace WireMock.Net.Tests.Proxy;

public class ProxyUrlTransformerTests
{
    private readonly Mock<IFileSystemHandler> _fileSystemHandlerMock = new();

    [Fact]
    public void Transform_WithUseTransformerFalse_PerformsSimpleReplace_CaseSensitive()
    {
        // Arrange
        var settings = new WireMockServerSettings
        {
            FileSystemHandler = _fileSystemHandlerMock.Object,
            Culture = CultureInfo.InvariantCulture
        };

        var replaceSettings = new ProxyUrlReplaceSettings
        {
            TransformTemplate = null,
            OldValue = "/old",
            NewValue = "/new",
            IgnoreCase = false
        };

        var url = "http://example.com/old/path";
        var expected = "http://example.com/new/path";

        // Act
        var actual = ProxyUrlTransformer.Transform(settings, replaceSettings, url);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Transform_WithUseTransformerFalse_PerformsSimpleReplace_IgnoreCase()
    {
        // Arrange
        var settings = new WireMockServerSettings
        {
            FileSystemHandler = _fileSystemHandlerMock.Object,
            Culture = CultureInfo.InvariantCulture
        };

        var replaceSettings = new ProxyUrlReplaceSettings
        {
            TransformTemplate = null, // UseTransformer == false
            OldValue = "/OLD",
            NewValue = "/new",
            IgnoreCase = true
        };

        var url = "http://example.com/old/path"; // lowercase 'old' but OldValue is uppercase
        var expected = "http://example.com/new/path";

        // Act
        var actual = ProxyUrlTransformer.Transform(settings, replaceSettings, url);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Transform_WithUseTransformerTrue_UsesTransformer_ToTransformUrl()
    {
        // Arrange
        var settings = new WireMockServerSettings
        {
            FileSystemHandler = _fileSystemHandlerMock.Object,
            Culture = CultureInfo.InvariantCulture
        };

        // Handlebars is the default TransformerType; the TransformTemplate uses the model directly.
        var replaceSettings = new ProxyUrlReplaceSettings
        {
            TransformTemplate = "{{this}}-transformed",
            // TransformerType defaults to Handlebars but set explicitly for clarity.
            TransformerType = TransformerType.Handlebars
        };

        var url = "http://example.com/path";
        var expected = "http://example.com/path-transformed";

        // Act
        var actual = ProxyUrlTransformer.Transform(settings, replaceSettings, url);

        // Assert
        Assert.Equal(expected, actual);
    }
}