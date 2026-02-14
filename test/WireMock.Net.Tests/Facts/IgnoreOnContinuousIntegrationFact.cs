// Copyright © WireMock.Net

using System.Diagnostics.CodeAnalysis;

namespace WireMock.Net.Tests.Facts;

[ExcludeFromCodeCoverage]
public sealed class IgnoreOnContinuousIntegrationFact : FactAttribute
{
    private const string SkipReason = "Ignore when run via CI/CD";
    private static readonly bool IsContinuousIntegrationAzure = bool.TryParse(Environment.GetEnvironmentVariable("TF_BUILD"), out var isTF) && isTF;
    private static readonly bool IsContinuousIntegrationGithub = bool.TryParse(Environment.GetEnvironmentVariable("GITHUB_ACTIONS"), out var isGH) && isGH;
    private static readonly bool IsContinuousIntegration = IsContinuousIntegrationAzure || IsContinuousIntegrationGithub;

    public IgnoreOnContinuousIntegrationFact()
    {
        if (IsContinuousIntegration)
        {
            Skip = SkipReason;
        }
    }
}