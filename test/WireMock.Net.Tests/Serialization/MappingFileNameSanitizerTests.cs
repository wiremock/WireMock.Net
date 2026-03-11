// Copyright © WireMock.Net

using Moq;
using WireMock.Serialization;
using WireMock.Settings;

namespace WireMock.Net.Tests.Serialization;

public class MappingFileNameSanitizerTests
{
    private const string MappingGuid = "ce216a13-e7d6-42d7-91ac-8ae709e2add1";
    private const string MappingTitle = "Proxy Mapping for POST _ordermanagement_v1_orders_cancel";

    [Fact]
    public void BuildSanitizedFileName_WithTitleAndGuid_AppendsGuid()
    {
        // Arrange
        var mappingMock = new Mock<IMapping>();
        mappingMock.Setup(m => m.Title).Returns(MappingTitle);
        mappingMock.Setup(m => m.Guid).Returns(new Guid(MappingGuid));

        var settings = new WireMockServerSettings
        {
            ProxyAndRecordSettings = new ProxyAndRecordSettings
            {
                AppendGuidToSavedMappingFile = true
            }
        };

        var sanitizer = new MappingFileNameSanitizer(settings);

        // Act
        var result = sanitizer.BuildSanitizedFileName(mappingMock.Object);

        // Assert
        Assert.Equal($"Proxy Mapping for _POST_ordermanagement_v1_orders_cancel_{MappingGuid}.json", result);
    }

    [Fact]
    public void BuildSanitizedFileName_WithoutTitle_UsesGuid()
    {
        // Arrange
        var mappingMock = new Mock<IMapping>();
        mappingMock.Setup(m => m.Title).Returns((string?)null);
        mappingMock.Setup(m => m.Guid).Returns(new Guid(MappingGuid));

        var settings = new WireMockServerSettings
        {
            ProxyAndRecordSettings = new ()
        };
        var sanitizer = new MappingFileNameSanitizer(settings);

        // Act
        var result = sanitizer.BuildSanitizedFileName(mappingMock.Object);

        // Assert
        Assert.Equal($"Proxy Mapping for _{MappingGuid}.json", result);
    }

    [Fact]
    public void BuildSanitizedFileName_WithTitleAndGuid_NoAppendGuidSetting()
    {
        // Arrange
        var mappingMock = new Mock<IMapping>();
        mappingMock.Setup(m => m.Title).Returns(MappingTitle);
        mappingMock.Setup(m => m.Guid).Returns(new Guid(MappingGuid));

        var settings = new WireMockServerSettings
        {
            ProxyAndRecordSettings = new ProxyAndRecordSettings
            {
                AppendGuidToSavedMappingFile = false
            }
        };

        var sanitizer = new MappingFileNameSanitizer(settings);

        // Act
        var result = sanitizer.BuildSanitizedFileName(mappingMock.Object);

        // Assert
        Assert.Equal("Proxy Mapping for _POST_ordermanagement_v1_orders_cancel.json", result);
    }

    [Fact]
    public void BuildSanitizedFileName_WithPrefix_AddsPrefix()
    {
        // Arrange
        var mappingMock = new Mock<IMapping>();
        mappingMock.Setup(m => m.Title).Returns(MappingTitle);
        mappingMock.Setup(m => m.Guid).Returns(new Guid(MappingGuid));

        var settings = new WireMockServerSettings
        {
            ProxyAndRecordSettings = new ProxyAndRecordSettings
            {
                PrefixForSavedMappingFile = "Prefix"
            }
        };

        var sanitizer = new MappingFileNameSanitizer(settings);

        // Act
        var result = sanitizer.BuildSanitizedFileName(mappingMock.Object);

        // Assert
        Assert.Equal($"Prefix_POST_ordermanagement_v1_orders_cancel.json", result);
    }

    [Fact]
    public void BuildSanitizedFileName_WithPrefix_AddsPrefixEmptyString()
    {
        // Arrange
        var mappingMock = new Mock<IMapping>();
        mappingMock.Setup(m => m.Title).Returns(MappingTitle);
        mappingMock.Setup(m => m.Guid).Returns(new Guid(MappingGuid));

        var settings = new WireMockServerSettings
        {
            ProxyAndRecordSettings = new ProxyAndRecordSettings
            {
                PrefixForSavedMappingFile = string.Empty
            }
        };

        var sanitizer = new MappingFileNameSanitizer(settings);

        // Act
        var result = sanitizer.BuildSanitizedFileName(mappingMock.Object);

        // Assert
        Assert.Equal($"POST_ordermanagement_v1_orders_cancel.json", result);
    }

    [Fact]
    public void BuildSanitizedFileName_WithTitleAndGuid_WithPrefixAndAppendGuidSetting()
    {
        // Arrange
        var mappingMock = new Mock<IMapping>();
        mappingMock.Setup(m => m.Title).Returns(MappingTitle);
        mappingMock.Setup(m => m.Guid).Returns(new Guid(MappingGuid));

        var settings = new WireMockServerSettings
        {
            ProxyAndRecordSettings = new ProxyAndRecordSettings
            {
                PrefixForSavedMappingFile = "Prefix",
                AppendGuidToSavedMappingFile = true
            }
        };

        var sanitizer = new MappingFileNameSanitizer(settings);

        // Act
        var result = sanitizer.BuildSanitizedFileName(mappingMock.Object);

        // Assert
        Assert.Equal($"Prefix_POST_ordermanagement_v1_orders_cancel_{MappingGuid}.json", result);
    }
}
