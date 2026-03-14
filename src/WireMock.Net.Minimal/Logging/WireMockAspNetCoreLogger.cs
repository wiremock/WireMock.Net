// Copyright © WireMock.Net

using Microsoft.Extensions.Logging;

namespace WireMock.Logging;

internal sealed class WireMockAspNetCoreLogger(IWireMockLogger logger, string categoryName) : ILogger
{
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
                logger.Warn("[{0}] {1}", categoryName, message);
                break;

            default:
                logger.Error("[{0}] {1}", categoryName, message);
                break;
        }
    }
}