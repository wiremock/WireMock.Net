// Copyright © WireMock.Net

using System.Collections;
using WireMock.OpenTelemetry;

namespace WireMock.Net.Tests.OpenTelemetry;

public class OpenTelemetryOptionsParserTests
{
    [Fact]
    public void TryParseArguments_Enabled_ShouldReturnOptions()
    {
        // Act
        var result = OpenTelemetryOptionsParser.TryParseArguments(
        [
            "--OpenTelemetryEnabled", "true",
            "--OpenTelemetryActivitySourceName", "MyService",
            "--OpenTelemetryExcludeAdminRequests", "false",
            "--OpenTelemetryOtlpExporterEndpoint", "http://localhost:4317"
        ], null, out var options);

        // Assert
        result.Should().BeTrue();
        options.Should().NotBeNull();
        options.ActivitySourceName.Should().Be("MyService");
        options.ExcludeAdminRequests.Should().BeFalse();
        options.OtlpExporterEndpoint.Should().Be("http://localhost:4317");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("MyService")]
    public void TryParseArguments_Environment_UsesConfiguredOrDefaultActivitySourceName(string? name)
    {
        var environment = new Hashtable
        {
            ["WireMockServerSettings__OpenTelemetryEnabled"] = "true"
        };
        if (name is not null)
        {
            environment["WireMockServerSettings__OpenTelemetryActivitySourceName"] = name;
        }

        var result = OpenTelemetryOptionsParser.TryParseArguments([], environment, out var options);

        result.Should().BeTrue();
        options.Should().NotBeNull();
        options.ActivitySourceName.Should().Be(string.IsNullOrWhiteSpace(name) ? "WireMock.Net" : name);
    }

    [Fact]
    public void OpenTelemetryOptions_DefaultActivitySourceName_IsWireMockNet()
    {
        new OpenTelemetryOptions().ActivitySourceName.Should().Be("WireMock.Net");
    }

    [Fact]
    public void TryParseArguments_NotEnabled_ShouldReturnNull()
    {
        // Act
        var result = OpenTelemetryOptionsParser.TryParseArguments(Array.Empty<string>(), null, out var options);

        // Assert
        result.Should().BeFalse();
        options.Should().BeNull();
    }
}