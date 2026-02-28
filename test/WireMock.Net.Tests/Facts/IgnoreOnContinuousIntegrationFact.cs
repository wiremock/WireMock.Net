// Copyright © WireMock.Net

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace WireMock.Net.Tests.Facts;

[ExcludeFromCodeCoverage]
public sealed class IgnoreOnContinuousIntegrationFact : FactAttribute
{
    private const string SkipReason = "Ignore when run via CI/CD";
    private static readonly bool IsContinuousIntegrationAzure = bool.TryParse(Environment.GetEnvironmentVariable("TF_BUILD"), out var isTF) && isTF;
    private static readonly bool IsContinuousIntegrationGithub = bool.TryParse(Environment.GetEnvironmentVariable("GITHUB_ACTIONS"), out var isGH) && isGH;
    private static readonly bool IsContinuousIntegration = IsContinuousIntegrationAzure || IsContinuousIntegrationGithub;

    public IgnoreOnContinuousIntegrationFact(
        [CallerFilePath] string? sourceFilePath = null,
        [CallerLineNumber] int sourceLineNumber = -1) : base(sourceFilePath, sourceLineNumber)
    {
        if (IsContinuousIntegration)
        {
            Skip = SkipReason;
        }
    }
}