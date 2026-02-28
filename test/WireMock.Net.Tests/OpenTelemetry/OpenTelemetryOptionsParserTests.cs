// Copyright © WireMock.Net

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
            "--OpenTelemetryExcludeAdminRequests", "false",
            "--OpenTelemetryOtlpExporterEndpoint", "http://localhost:4317"
        ], null, out var options);

        // Assert
        result.Should().BeTrue();
        options.Should().NotBeNull();
        options!.ExcludeAdminRequests.Should().BeFalse();
        options.OtlpExporterEndpoint.Should().Be("http://localhost:4317");
    }

    [Fact]
    public void TryParseArguments_NotEnabled_ShouldReturnNull()
    {
        // Act
        var result = OpenTelemetryOptionsParser.TryParseArguments(Array.Empty<string>(), null, out var options);

        // Assert
        result.Should().BeTrue();
        options.Should().BeNull();
    }
}