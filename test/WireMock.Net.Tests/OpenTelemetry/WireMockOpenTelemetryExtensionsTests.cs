// Copyright © WireMock.Net

#if NET6_0_OR_GREATER
using AwesomeAssertions;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Trace;
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

    [Theory]
    [InlineData("WireMock.Net")]
    [InlineData("MyService")]
    public void AddWireMockOpenTelemetry_UsesConfiguredServiceAndListensToSources(string name)
    {
        var services = new ServiceCollection();
        var processor = new RecordingProcessor();
        services.AddWireMockOpenTelemetry(new OpenTelemetryOptions { ActivitySourceName = name });
        services.AddOpenTelemetry().WithTracing(builder => builder.AddProcessor(processor));
        using var provider = services.BuildServiceProvider();
        var tracerProvider = provider.GetRequiredService<TracerProvider>();
        using var wireMockSource = new ActivitySource("WireMock.Net");
        using var configuredSource = new ActivitySource(name);

        wireMockSource.StartActivity("wiremock-test")?.Dispose();
        configuredSource.StartActivity("configured-test")?.Dispose();

        processor.Names.Should().Contain(["wiremock-test", "configured-test"]);
        tracerProvider.GetResource().Attributes.Should().Contain(attribute => attribute.Key == "service.name" && (string)attribute.Value == name);
    }

    [Theory]
    [InlineData(null, "WireMock.Net")]
    [InlineData("MyInstrumentation", "MyInstrumentation")]
    public void AddWireMockInstrumentation_ListensToConfiguredAndWireMockSources(string? name, string expectedName)
    {
        var processor = new RecordingProcessor();
        using var provider = Sdk.CreateTracerProviderBuilder()
            .AddWireMockInstrumentation(name is null ? null : new OpenTelemetryOptions { ActivitySourceName = name })
            .AddProcessor(processor)
            .Build();
        using var wireMockSource = new ActivitySource("WireMock.Net");
        using var configuredSource = new ActivitySource(expectedName);

        wireMockSource.StartActivity("wiremock-test")?.Dispose();
        configuredSource.StartActivity("configured-test")?.Dispose();

        processor.Names.Should().Contain(["wiremock-test", "configured-test"]);
    }

    private sealed class RecordingProcessor : BaseProcessor<Activity>
    {
        public List<string> Names { get; } = [];

        public override void OnEnd(Activity data) => Names.Add(data.DisplayName);
    }
}
#endif
