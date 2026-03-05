// Copyright (c) WireMock.Net

#if !(NET452 || NET461 || NETCOREAPP3_1)
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using WireMock.Logging;
using WireMock.Owin;
using WireMock.Server;
using WireMock.Settings;
using Xunit;

namespace WireMock.Net.Tests;

public class WireMockServerSoftMaxTests
{
    [Fact]
    public void SoftMaxRequestLogCountEnabled_DefaultsToNull()
    {
        // Arrange & Act
        var settings = new WireMockServerSettings();

        // Assert
        settings.SoftMaxRequestLogCountEnabled.Should().BeNull();
    }

    [Fact]
    public void SoftMaxRequestLogCountEnabled_CanBeSetToTrue()
    {
        // Arrange & Act
        var settings = new WireMockServerSettings
        {
            SoftMaxRequestLogCountEnabled = true
        };

        // Assert
        settings.SoftMaxRequestLogCountEnabled.Should().BeTrue();
    }

    [Fact]
    public void SoftMaxRequestLogCountEnabled_CanBeSetToFalse()
    {
        // Arrange & Act
        var settings = new WireMockServerSettings
        {
            SoftMaxRequestLogCountEnabled = false
        };

        // Assert
        settings.SoftMaxRequestLogCountEnabled.Should().BeFalse();
    }

    [Fact]
    public void SoftMaxRequestLogCountEnabled_PropagatesFromSettingsToMiddlewareOptions()
    {
        // Arrange
        var settings = new WireMockServerSettings
        {
            SoftMaxRequestLogCountEnabled = true,
            Logger = new WireMockNullLogger()
        };

        // Act
        var options = WireMockMiddlewareOptionsHelper.InitFromSettings(settings);

        // Assert
        options.SoftMaxRequestLogCountEnabled.Should().BeTrue();
    }

    [Fact]
    public void SoftMaxRequestLogCountEnabled_PropagatesNullFromSettingsToMiddlewareOptions()
    {
        // Arrange
        var settings = new WireMockServerSettings
        {
            Logger = new WireMockNullLogger()
        };

        // Act
        var options = WireMockMiddlewareOptionsHelper.InitFromSettings(settings);

        // Assert
        options.SoftMaxRequestLogCountEnabled.Should().BeNull();
    }

    [Fact]
    public async Task SoftMaxRequestLogCountEnabled_AdminApi_SettingsGet_ReturnsValue()
    {
        // Arrange
        var server = WireMockServer.Start(new WireMockServerSettings
        {
            SoftMaxRequestLogCountEnabled = true,
            StartAdminInterface = true
        });

        try
        {
            using var httpClient = new HttpClient();

            // Act
            var response = await httpClient.GetStringAsync($"{server.Urls[0]}/__admin/settings");
            var json = JObject.Parse(response);

            // Assert
            json["SoftMaxRequestLogCountEnabled"]?.Value<bool>().Should().BeTrue();
        }
        finally
        {
            server.Stop();
        }
    }

    [Fact]
    public async Task SoftMaxRequestLogCountEnabled_AdminApi_SettingsUpdate_PersistsValue()
    {
        // Arrange
        var server = WireMockServer.Start(new WireMockServerSettings
        {
            StartAdminInterface = true
        });

        try
        {
            using var httpClient = new HttpClient();

            // Act - Update the setting
            var content = new StringContent(
                "{\"SoftMaxRequestLogCountEnabled\": true}",
                Encoding.UTF8,
                "application/json"
            );
            var putResponse = await httpClient.PutAsync($"{server.Urls[0]}/__admin/settings", content);
            putResponse.EnsureSuccessStatusCode();

            // Act - Read back the setting
            var response = await httpClient.GetStringAsync($"{server.Urls[0]}/__admin/settings");
            var json = JObject.Parse(response);

            // Assert
            json["SoftMaxRequestLogCountEnabled"]?.Value<bool>().Should().BeTrue();
        }
        finally
        {
            server.Stop();
        }
    }
}
#endif
