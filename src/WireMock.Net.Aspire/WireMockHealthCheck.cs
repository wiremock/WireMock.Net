// Copyright © WireMock.Net

using Aspire.Hosting.ApplicationModel;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using WireMock.Client;

namespace WireMock.Net.Aspire;

/// <summary>
/// WireMockHealthCheck
/// </summary>
public class WireMockHealthCheck(WireMockServerResource resource) : IHealthCheck
{
    private const string HealthStatusHealthy = "Healthy";

    /// <inheritdoc />
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (!await IsHealthyAsync(resource.AdminApi.Value, cancellationToken))
        {
            return HealthCheckResult.Unhealthy("WireMock.Net is not healthy");
        }

        if (resource.ApiMappingState == WireMockMappingState.NotSubmitted)
        {
            return HealthCheckResult.Unhealthy("WireMock.Net has not received mappings");
        }

        return HealthCheckResult.Healthy();
    }

    private static async Task<bool> IsHealthyAsync(IWireMockAdminApi adminApi, CancellationToken cancellationToken)
    {
        try
        {
            var status = await adminApi.GetHealthAsync(cancellationToken);
            return string.Equals(status, HealthStatusHealthy, StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }
}
