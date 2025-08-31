// Copyright © WireMock.Net

#if NET6_0_OR_GREATER
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using FluentAssertions;
using FluentAssertions.Execution;
using WireMock.Net.Testcontainers;
using WireMock.Net.Testcontainers.Utils;
using WireMock.Net.Tests.Facts;
using Xunit;
using Xunit.Abstractions;

namespace WireMock.Net.Tests.Testcontainers;

public partial class TestcontainersTests(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public async Task WireMockContainer_Build_And_StartAsync_and_StopAsync()
    {
        // Act
        var adminUsername = $"username_{Guid.NewGuid()}";
        var adminPassword = $"password_{Guid.NewGuid()}";
        var wireMockContainer = new WireMockContainerBuilder()
            .WithAdminUserNameAndPassword(adminUsername, adminPassword)
            .WithAutoRemove(true)
            .WithCleanUp(true)
            .Build();

        await StartTestAsync(wireMockContainer);
        await StopAsync(wireMockContainer);
    }

    // https://github.com/testcontainers/testcontainers-dotnet/issues/1322
    [RunOnDockerPlatformFact("Linux")]
    public async Task WireMockContainer_Build_WithNetwork_And_StartAsync_and_StopAsync()
    {
        // Act
        var dummyNetwork = new NetworkBuilder()
            .WithName("Dummy Network for TestcontainersTests")
            .Build();

        var wireMockContainer = new WireMockContainerBuilder()
            .WithNetwork(dummyNetwork)
            .WithWatchStaticMappings(true)
            .Build();

        await StartTestAsync(wireMockContainer);
        await StopAsync(wireMockContainer);
    }

    [Fact]
    public async Task WireMockContainer_Build_WithImage_And_StartAsync_and_StopAsync()
    {
        // Arrange
        var adminUsername = $"username_{Guid.NewGuid()}";
        var adminPassword = $"password_{Guid.NewGuid()}";
        var wireMockContainerBuilder = new WireMockContainerBuilder()
            .WithAdminUserNameAndPassword(adminUsername, adminPassword);

        var imageOS = await TestcontainersUtils.GetImageOSAsync.Value;
        if (imageOS == OSPlatform.Windows)
        {
            wireMockContainerBuilder = wireMockContainerBuilder.WithWindowsImage();
        }
        else
        {
            wireMockContainerBuilder = wireMockContainerBuilder.WithLinuxImage();
        }

        var wireMockContainer = wireMockContainerBuilder.Build();

        await StartTestAsync(wireMockContainer);
        await StopAsync(wireMockContainer);
    }

    [Fact]
    public async Task WireMockContainer_Build_WithImageAsText_And_StartAsync_and_StopAsync()
    {
        // Arrange
        var adminUsername = $"username_{Guid.NewGuid()}";
        var adminPassword = $"password_{Guid.NewGuid()}";
        var wireMockContainerBuilder = new WireMockContainerBuilder()
            .WithAdminUserNameAndPassword(adminUsername, adminPassword);

        var imageOS = await TestcontainersUtils.GetImageOSAsync.Value;
        if (imageOS == OSPlatform.Windows)
        {
            wireMockContainerBuilder = wireMockContainerBuilder.WithImage("sheyenrath/wiremock.net-windows");
        }
        else
        {
            wireMockContainerBuilder = wireMockContainerBuilder.WithImage("sheyenrath/wiremock.net");
        }

        var wireMockContainer = wireMockContainerBuilder.Build();

        await StartTestAsync(wireMockContainer);
        await StopAsync(wireMockContainer);
    }

    private static async Task StartTestAsync(WireMockContainer wireMockContainer)
    {
        // Start
        await wireMockContainer.StartAsync().ConfigureAwait(false);

        // Assert
        using (new AssertionScope())
        {
            var url = wireMockContainer.GetPublicUrl();
            url.Should().NotBeNullOrWhiteSpace();

            var adminClient = wireMockContainer.CreateWireMockAdminClient();

            var settings = await adminClient.GetSettingsAsync();
            settings.Should().NotBeNull();
        }
    }

    private async Task StopAsync(WireMockContainer wireMockContainer)
    {
        try
        {
            await wireMockContainer.StopAsync();
        }
        catch (Exception ex)
        {
            // Sometimes we get this exception, so for now ignore it.
            /*
            Failed WireMock.Net.Tests.Testcontainers.TestcontainersTests.WireMockContainer_Build_WithImageAsText_And_StartAsync_and_StopAsync [9 s]
               Error Message:
                System.NullReferenceException : Object reference not set to an instance of an object.
               Stack Trace:
                  at DotNet.Testcontainers.Containers.DockerContainer.UnsafeStopAsync(CancellationToken ct) in /_/src/Testcontainers/Containers/DockerContainer.cs:line 567
                at DotNet.Testcontainers.Containers.DockerContainer.StopAsync(CancellationToken ct) in /_/src/Testcontainers/Containers/DockerContainer.cs:line 319
            */

            testOutputHelper.WriteLine($"Exception during StopAsync: {ex}");
        }
    }
}
#endif