// Copyright © WireMock.Net

using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace WireMock.Net.WebApplication;

public class App : IHostedService
{
    private readonly IWireMockService _service;
        
    public App(IWireMockService service)
    {
        _service = service;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _service.Start();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _service.Stop();
        return Task.CompletedTask;
    }
}