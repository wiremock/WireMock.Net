// Copyright © WireMock.Net

using AwesomeAssertions;
using WireMock.Settings;

namespace WireMock.Net.Tests.Settings;

public class WireMockServerSettingsParserTests
{
    [Fact]
    public void TryParseArguments_With_Args()
    {
        // Act
        var result = WireMockServerSettingsParser.TryParseArguments(new[]
        {
            "--adminPath", "ap"
        }, null, out var settings);

        // Assert
        result.Should().BeTrue();
        settings.Should().NotBeNull();
        settings!.AdminPath.Should().Be("ap");
    }

    [Fact]
    public void TryParseArguments_Without_Args()
    {
        // Act
        var result = WireMockServerSettingsParser.TryParseArguments(new string[] { }, null, out var settings);

        // Assert
        result.Should().BeTrue();
        settings.Should().NotBeNull();
        settings!.AdminPath.Should().Be("/__admin");
    }

    [Fact]
    public void TryParseArguments_With_ActivityTracingEnabled_ShouldParseOptions()
    {
        // Act
        var result = WireMockServerSettingsParser.TryParseArguments(new[]
        {
            "--ActivityTracingEnabled", "true",
            "--ActivityTracingExcludeAdminRequests", "false",
            "--ActivityTracingRecordRequestBody", "true",
            "--ActivityTracingRecordResponseBody", "true"
        }, null, out var settings);

        // Assert
        result.Should().BeTrue();
        settings.Should().NotBeNull();
        settings!.ActivityTracingOptions.Should().NotBeNull();
        settings.ActivityTracingOptions!.ExcludeAdminRequests.Should().BeFalse();
        settings.ActivityTracingOptions.RecordRequestBody.Should().BeTrue();
        settings.ActivityTracingOptions.RecordResponseBody.Should().BeTrue();
        settings.ActivityTracingOptions.RecordMatchDetails.Should().BeTrue();
    }
}