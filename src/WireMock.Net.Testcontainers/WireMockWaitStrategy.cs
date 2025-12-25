// Copyright © WireMock.Net

using System;
using System.Threading.Tasks;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;

namespace WireMock.Net.Testcontainers;

internal class WireMockWaitStrategy : IWaitUntil
{
    public async Task<bool> UntilAsync(IContainer container)
    {
        if (container is not WireMockContainer wireMockContainer)
        {
            throw new InvalidOperationException("The passed container is not a WireMockContainer.");

        }

        await wireMockContainer.CallAdditionalActionsAfterReadyAsync();
        return true;
    }
}
