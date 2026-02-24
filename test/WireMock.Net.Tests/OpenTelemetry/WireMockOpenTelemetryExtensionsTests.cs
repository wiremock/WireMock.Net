// Copyright © WireMock.Net

#if NET6_0_OR_GREATER
using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using WireMock.OpenTelemetry;

namespace WireMock.Net.Tests.OpenTelemetry;

public class WireMockOpenTelemetryExtensionsTests
{
    [Fact]
    public void AddWireMockOpenTelemetry_WithNullOptions_ShouldNotAddServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var initialCount = services.Count;

        // Act
        var result = services.AddWireMockOpenTelemetry(null);

        // Assert
        result.Should().BeSameAs(services);
        services.Count.Should().Be(initialCount);
    }

    [Fact]
    public void AddWireMockOpenTelemetry_WithOptions_ShouldAddServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var initialCount = services.Count;

        // Act
        var result = services.AddWireMockOpenTelemetry(new OpenTelemetryOptions());

        // Assert
        result.Should().BeSameAs(services);
        services.Count.Should().BeGreaterThan(initialCount);
    }
}
#endif
