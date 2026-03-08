// Copyright (c) WireMock.Net

#if !(NET452 || NET461 || NETCOREAPP3_1)
using System;
using System.Linq;
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
    // --- Plan 02: Background timer behavior and compatibility tests ---

    [Fact]
    public async Task SoftMaxDisabled_InlineTrimStillRunsOnEveryRequest()
    {
        // Arrange - SoftMaxRequestLogCountEnabled is NOT set (default null = disabled)
        var server = WireMockServer.Start(new WireMockServerSettings
        {
            MaxRequestLogCount = 3
        });

        try
        {
            using var httpClient = new HttpClient();
            var baseUrl = server.Urls[0];

            // Act - Send 5 requests (exceeds MaxRequestLogCount of 3)
            for (int i = 0; i < 5; i++)
            {
                await httpClient.GetAsync($"{baseUrl}/request{i}");
            }

            // Assert - Inline trim should have kept count at exactly MaxRequestLogCount
            server.LogEntries.Count().Should().Be(3);
        }
        finally
        {
            server.Stop();
        }
    }

    [Fact]
    public async Task SoftMaxEnabled_LogCountCanTemporarilyExceedMax()
    {
        // Arrange - SoftMaxRequestLogCountEnabled = true, so inline trim is skipped
        var server = WireMockServer.Start(new WireMockServerSettings
        {
            MaxRequestLogCount = 3,
            SoftMaxRequestLogCountEnabled = true
        });

        try
        {
            using var httpClient = new HttpClient();
            var baseUrl = server.Urls[0];

            // Act - Send 6 requests quickly (before timer fires)
            for (int i = 0; i < 6; i++)
            {
                await httpClient.GetAsync($"{baseUrl}/request{i}");
            }

            // Assert - Count should exceed MaxRequestLogCount since inline trim is bypassed
            server.LogEntries.Count().Should().BeGreaterThan(3);
        }
        finally
        {
            server.Stop();
        }
    }

    [Fact]
    public async Task SoftMaxEnabled_BackgroundTimerEventuallyTrimsExcess()
    {
        // Arrange - SoftMaxRequestLogCountEnabled = true with timer
        var server = WireMockServer.Start(new WireMockServerSettings
        {
            MaxRequestLogCount = 3,
            SoftMaxRequestLogCountEnabled = true
        });

        try
        {
            using var httpClient = new HttpClient();
            var baseUrl = server.Urls[0];

            // Act - Send 10 requests
            for (int i = 0; i < 10; i++)
            {
                await httpClient.GetAsync($"{baseUrl}/request{i}");
            }

            // Verify excess before timer fires
            server.LogEntries.Count().Should().BeGreaterThan(3);

            // Wait for background timer to fire (timer interval is 5 seconds)
            await Task.Delay(TimeSpan.FromSeconds(7));

            // Assert - Timer should have trimmed excess entries
            server.LogEntries.Count().Should().BeLessThanOrEqualTo(3);
        }
        finally
        {
            server.Stop();
        }
    }

    [Fact]
    public void SoftMaxEnabled_ServerDisposesCleanly()
    {
        // Arrange - Start server with soft max enabled (timer running)
        var server = WireMockServer.Start(new WireMockServerSettings
        {
            MaxRequestLogCount = 5,
            SoftMaxRequestLogCountEnabled = true
        });

        // Act & Assert - Dispose should not throw
        var act = () => server.Dispose();
        act.Should().NotThrow();
    }

    [Fact]
    public async Task SoftMaxExplicitlyFalse_InlineTrimStillRunsOnEveryRequest()
    {
        // Arrange - SoftMaxRequestLogCountEnabled explicitly false
        var server = WireMockServer.Start(new WireMockServerSettings
        {
            MaxRequestLogCount = 3,
            SoftMaxRequestLogCountEnabled = false
        });

        try
        {
            using var httpClient = new HttpClient();
            var baseUrl = server.Urls[0];

            // Act - Send 5 requests
            for (int i = 0; i < 5; i++)
            {
                await httpClient.GetAsync($"{baseUrl}/request{i}");
            }

            // Assert - Inline trim should keep count at exactly MaxRequestLogCount
            server.LogEntries.Count().Should().Be(3);
        }
        finally
        {
            server.Stop();
        }
    }
}
#endif
