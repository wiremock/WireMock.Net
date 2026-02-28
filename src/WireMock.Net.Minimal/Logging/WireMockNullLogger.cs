// Copyright © WireMock.Net

using WireMock.Admin.Requests;

namespace WireMock.Logging;

/// <summary>
/// WireMockNullLogger which does not log.
/// </summary>
/// <seealso cref="IWireMockLogger" />
public class WireMockNullLogger : IWireMockLogger
{
    /// <see cref="IWireMockLogger.Debug"/>
    public void Debug(string formatString, params object[] args)
    {
        // Log nothing
    }

    /// <see cref="IWireMockLogger.Info"/>
    public void Info(string formatString, params object[] args)
    {
        // Log nothing
    }

    /// <see cref="IWireMockLogger.Warn"/>
    public void Warn(string formatString, params object[] args)
    {
        // Log nothing
    }

    /// <see cref="IWireMockLogger.Error(string, object[])"/>
    public void Error(string formatString, params object[] args)
    {
        // Log nothing
    }

    /// <see cref="IWireMockLogger.Error(string, Exception)"/>
    public void Error(string message, Exception exception)
    {
        // Log nothing
    }

    /// <see cref="IWireMockLogger.DebugRequestResponse"/>
    public void DebugRequestResponse(LogEntryModel logEntryModel, bool isAdminRequest)
    {
        // Log nothing
    }
}