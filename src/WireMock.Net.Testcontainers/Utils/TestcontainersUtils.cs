// Copyright © WireMock.Net

using System;
using System.Runtime.InteropServices;
using DotNet.Testcontainers.Configurations;
using System.Threading.Tasks;

namespace WireMock.Net.Testcontainers.Utils;

/// <summary>
/// Some utility methods for containers.
/// </summary>
public static class TestcontainersUtils
{
    /// <summary>
    /// Get the OS platform of the Docker image.
    /// </summary>
    public static Lazy<Task<OSPlatform>> GetImageOSAsync = new(async () =>
    {
        if (TestcontainersSettings.OS.DockerEndpointAuthConfig == null)
        {
            throw new InvalidOperationException($"The {nameof(TestcontainersSettings.OS.DockerEndpointAuthConfig)} is null. Check if Docker is started.");
        }

        var dockerClientBuilder = TestcontainersSettings.OS.DockerEndpointAuthConfig.GetDockerClientBuilder();
        using var dockerClient = dockerClientBuilder.Build();

        var version = await dockerClient.System.GetVersionAsync();
        return version.Os.IndexOf("Windows", StringComparison.OrdinalIgnoreCase) >= 0 ? OSPlatform.Windows : OSPlatform.Linux;
    });
}