// Copyright © WireMock.Net

using System.Net.Http.Json;
using AwesomeAssertions;
using Projects;
using WireMock.Net.Aspire.Tests.Facts;

namespace WireMock.Net.Aspire.Tests;

public class IntegrationTests
{
    private record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary);

    [DockerIsRunningInLinuxContainerModeFact]
    public async Task StartAppHostWithWireMockAndCreateHttpClientToCallTheMockedWeatherForecastEndpoint()
    {
        // Arrange
        var appHostBuilder = await DistributedApplicationTestingBuilder.CreateAsync<WireMock_Net_Aspire_TestAppHost>();
        await using var app = await appHostBuilder.BuildAsync();
        await app.StartAsync();
        await app.ResourceNotifications.WaitForResourceHealthyAsync("wiremock-service");

        using var httpClient = app.CreateHttpClient("wiremock-service");

        // Act 1
        var weatherForecasts1 = await httpClient.GetFromJsonAsync<WeatherForecast[]>("/weatherforecast");

        // Assert 1
        weatherForecasts1.Should().BeEquivalentTo(
        [
            new WeatherForecast(new DateOnly(2024, 5, 24), -10, "Freezing"),
            new WeatherForecast(new DateOnly(2024, 5, 25), +33, "Hot")
        ]);

        // Act 2
        var weatherForecasts2 = await httpClient.GetFromJsonAsync<WeatherForecast[]>("/weatherforecast2");

        // Assert 2
        weatherForecasts2.Should().HaveCount(5);
    }

    [DockerIsRunningInLinuxContainerModeFact]
    public async Task StartAppHostWithWireMockAndCreateWireMockAdminClientToCallTheAdminEndpoint()
    {
        // Arrange
        var appHostBuilder = await DistributedApplicationTestingBuilder.CreateAsync<WireMock_Net_Aspire_TestAppHost>();
        await using var app = await appHostBuilder.BuildAsync();
        await app.StartAsync();
        await app.ResourceNotifications.WaitForResourceHealthyAsync("wiremock-service");

        var adminClient = app.CreateWireMockAdminClient("wiremock-service");

        // Act 1
        var settings = await adminClient.GetSettingsAsync();

        // Assert 1
        settings.Should().NotBeNull();

        // Act 2
        var mappings = await adminClient.GetMappingsAsync();

        // Assert 2
        mappings.Should().HaveCount(2);
    }
}