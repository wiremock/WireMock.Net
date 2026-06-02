// Copyright © WireMock.Net

using System.Diagnostics;
using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Eventing;
using Aspire.Hosting.Lifecycle;
using Microsoft.Extensions.Logging;

namespace WireMock.Net.Aspire;

internal class WireMockLifecycleSubscriber(ILoggerFactory loggerFactory) : IDistributedApplicationEventingSubscriber
{
    public Task SubscribeAsync(IDistributedApplicationEventing eventing, DistributedApplicationExecutionContext executionContext, CancellationToken cancellationToken)
    {
        eventing.Subscribe<ResourceEndpointsAllocatedEvent>((@event, ct) =>
        {
            if (@event.Resource is WireMockServerResource wireMockServerResource)
            {
                wireMockServerResource.SetLogger(loggerFactory.CreateLogger<WireMockServerResource>());

                var endpoint = wireMockServerResource.GetEndpoint();
                Debug.Assert(endpoint.IsAllocated);

                var logger = loggerFactory.CreateLogger<WireMockLifecycleSubscriber>();
                _ = Task.Run(() => ConfigureWireMockServerAsync(wireMockServerResource, logger, ct), CancellationToken.None);
            }

            return Task.CompletedTask;
        });

        return Task.CompletedTask;
    }

    private static async Task ConfigureWireMockServerAsync(WireMockServerResource wireMockServerResource, ILogger logger, CancellationToken cancellationToken)
    {
        try
        {
            await wireMockServerResource.WaitForHealthAsync(cancellationToken);

            await wireMockServerResource.CallAddProtoDefinitionsAsync(cancellationToken);

            await wireMockServerResource.CallApiMappingBuilderActionAsync(cancellationToken);

            wireMockServerResource.StartWatchingStaticMappings(cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // AppHost is stopping.
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Error configuring WireMock.Net resource {ResourceName}.", wireMockServerResource.Name);
        }
    }
}
