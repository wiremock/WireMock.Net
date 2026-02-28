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
        eventing.Subscribe<ResourceEndpointsAllocatedEvent>(async (@event, ct) =>
        {
            if (@event.Resource is WireMockServerResource wireMockServerResource)
            {
                wireMockServerResource.SetLogger(loggerFactory.CreateLogger<WireMockServerResource>());

                var endpoint = wireMockServerResource.GetEndpoint();
                Debug.Assert(endpoint.IsAllocated);

                await wireMockServerResource.WaitForHealthAsync(ct);

                await wireMockServerResource.CallAddProtoDefinitionsAsync(ct);

                await wireMockServerResource.CallApiMappingBuilderActionAsync(ct);

                wireMockServerResource.StartWatchingStaticMappings(ct);
            }
        });

        return Task.CompletedTask;
    }
}