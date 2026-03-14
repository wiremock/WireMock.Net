// Copyright © WireMock.Net

using Microsoft.Extensions.Logging;

namespace WireMock.Logging;

internal sealed class WireMockAspNetCoreLoggerProvider : ILoggerProvider
{
    private readonly IWireMockLogger _logger;

    public WireMockAspNetCoreLoggerProvider(IWireMockLogger logger)
    {
        _logger = logger;
    }

    public ILogger CreateLogger(string categoryName) => new WireMockAspNetCoreLogger(_logger, categoryName);

    public void Dispose() { }
}