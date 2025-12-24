// Copyright © WireMock.Net

using WireMock.Net.Testcontainers;

namespace DotNet.Testcontainers.Configurations;

internal static class HttpWaitStrategyExtensions
{
    internal static HttpWaitStrategy WithBasicAuthentication(this HttpWaitStrategy strategy, WireMockConfiguration configuration)
    {
        if (configuration.HasBasicAuthentication)
        {
            return strategy.WithBasicAuthentication(configuration.Username, configuration.Password);
        }

        return strategy;
    }
}