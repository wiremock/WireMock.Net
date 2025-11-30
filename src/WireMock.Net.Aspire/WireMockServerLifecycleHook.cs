// Copyright © WireMock.Net

using System.Diagnostics;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Lifecycle;
using Microsoft.Extensions.Logging;

namespace WireMock.Net.Aspire;

internal class WireMockServerLifecycleHook(ILoggerFactory loggerFactory) : IDistributedApplicationLifecycleHook, IAsyncDisposable
{
    private readonly CancellationTokenSource _shutdownCts = new();

    private CancellationTokenSource? _linkedCts;
    private Task? _mappingTask;

    public Task AfterEndpointsAllocatedAsync(DistributedApplicationModel appModel, CancellationToken cancellationToken = default)
    {
        _linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_shutdownCts.Token, cancellationToken);

        _mappingTask = Task.Run(async () =>
        {
            var wireMockServerResources = appModel.Resources
                .OfType<WireMockServerResource>()
                .ToArray();

            foreach (var wireMockServerResource in wireMockServerResources)
            {
                wireMockServerResource.SetLogger(loggerFactory.CreateLogger<WireMockServerResource>());

                var endpoint = wireMockServerResource.GetEndpoint();
                Debug.Assert(endpoint.IsAllocated);

                await wireMockServerResource.WaitForHealthAsync(_linkedCts.Token);

                await wireMockServerResource.CallAddProtoDefinitionsAsync(_linkedCts.Token);

                await wireMockServerResource.CallApiMappingBuilderActionAsync(_linkedCts.Token);

                wireMockServerResource.StartWatchingStaticMappings(_linkedCts.Token);
            }
        }, _linkedCts.Token);

        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        await _shutdownCts.CancelAsync();

        _linkedCts?.Dispose();
        _shutdownCts.Dispose();

        if (_mappingTask is not null)
        {
            await _mappingTask;
        }
    }
}