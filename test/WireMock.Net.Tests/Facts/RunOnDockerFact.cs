// Copyright © WireMock.Net
#if NET6_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WireMock.Net.Testcontainers.Utils;

namespace WireMock.Net.Tests.Facts;

[ExcludeFromCodeCoverage]
public sealed class RunOnDockerFact : FactAttribute
{
    public RunOnDockerFact(
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
        }
    }
}
#endif