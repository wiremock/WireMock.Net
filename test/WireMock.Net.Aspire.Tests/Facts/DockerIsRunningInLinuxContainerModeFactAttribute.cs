// Copyright © WireMock.Net

using System.Runtime.CompilerServices;

namespace WireMock.Net.Aspire.Tests.Facts;

public sealed class DockerIsRunningInLinuxContainerModeFactAttribute : FactAttribute
{
    private const string SkipReason = "Docker is not running in Linux container mode. Skipping test.";

    public DockerIsRunningInLinuxContainerModeFactAttribute(
        [CallerFilePath] string? sourceFilePath = null,
        [CallerLineNumber] int sourceLineNumber = -1) : base(sourceFilePath, sourceLineNumber)
    {
        if (!DockerUtils.IsDockerRunningLinuxContainerMode.Value)
        {
            Skip = SkipReason;
        }
    }
}