// Copyright © WireMock.Net

using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Lifecycle;
using Microsoft.Extensions.Logging;

namespace WireMock.Net.Aspire;

internal class WireMockServerLifecycleHook(ILoggerFactory loggerFactory) : IDistributedApplicationLifecycleHook, IAsyncDisposable
{
    private readonly CancellationTokenSource _shutdownCts = new();

    private Task? _mappingTask;

    public async Task AfterEndpointsAllocatedAsync(DistributedApplicationModel appModel, CancellationToken cancellationToken = default)
    {
        var cts = CancellationTokenSource.CreateLinkedTokenSource(_shutdownCts.Token, cancellationToken);

        _mappingTask = Task.Run(async () =>
        {
            var wireMockServerResources = appModel.Resources
                .OfType<WireMockServerResource>()
                .ToArray();

            foreach (var wireMockServerResource in wireMockServerResources)
            {
                wireMockServerResource.SetLogger(loggerFactory.CreateLogger<WireMockServerResource>());

                var endpoint = wireMockServerResource.GetEndpoint();
                System.Diagnostics.Debug.Assert(endpoint.IsAllocated);

                await wireMockServerResource.WaitForHealthAsync(cts.Token);

                await wireMockServerResource.CallApiMappingBuilderActionAsync(cts.Token);

                wireMockServerResource.StartWatchingStaticMappings(cts.Token);
            }
        }, cts.Token);
    }

    public async ValueTask DisposeAsync()
    {
        await _shutdownCts.CancelAsync();
        if (_mappingTask is not null)
            await _mappingTask;
    }
}