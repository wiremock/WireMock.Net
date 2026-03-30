// Copyright © WireMock.Net

namespace WireMock.Net.WebApplication;

public class App(IWireMockService service) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        service.Start();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        service.Stop();
        return Task.CompletedTask;
    }
}