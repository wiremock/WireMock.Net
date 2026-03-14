// Copyright © WireMock.Net

using Microsoft.Extensions.Logging;

namespace WireMock.Logging;

internal sealed class WireMockAspNetCoreLogger : ILogger
{
    private readonly IWireMockLogger _logger;
    private readonly string _categoryName;

    public WireMockAspNetCoreLogger(IWireMockLogger logger, string categoryName)
    {
        _logger = logger;
        _categoryName = categoryName;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Warning;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        var message = formatter(state, exception);

        if (exception != null)
        {
            message = $"{message} | Exception: {exception}";
        }

        switch (logLevel)
        {
            case LogLevel.Warning:
                _logger.Warn("[{0}] {1}", _categoryName, message);
                break;

            default:
                _logger.Error("[{0}] {1}", _categoryName, message);
                break;
        }
    }
}
