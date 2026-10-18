// Copyright © WireMock.Net
#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WireMock.Net.Testcontainers.Utils;

namespace WireMock.Net.Tests.Facts;

[ExcludeFromCodeCoverage]
public sealed class RunOnDockerPlatformFact : FactAttribute
{
    public RunOnDockerPlatformFact(
        string platform,
        [CallerFilePath] string? sourceFilePath = null,
        [CallerLineNumber] int sourceLineNumber = -1) : base(sourceFilePath, sourceLineNumber)
    {

        OSPlatform currentPlatform;
        try
        {
            currentPlatform = TestcontainersUtils.GetImageOSAsync.Value.Result;
        }
        catch
        {
            Skip = $"Only run test when Docker is installed.";
            return;
        }

        if (currentPlatform != OSPlatform.Create(platform))
        {
            Skip = $"Only run test when Docker OS Platform {platform} is used.";
        }
    }
}
#endif