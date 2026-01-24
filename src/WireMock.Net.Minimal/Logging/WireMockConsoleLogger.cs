// Copyright © WireMock.Net

using Newtonsoft.Json;
using System;
using WireMock.Admin.Requests;

namespace WireMock.Logging;

/// <summary>
/// WireMockConsoleLogger which logs to Console
/// </summary>
/// <seealso cref="IWireMockLogger" />
public class WireMockConsoleLogger : IWireMockLogger
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WireMockConsoleLogger"/> class.
    /// </summary>
    public WireMockConsoleLogger()
    {
        try
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
        }
        catch
        {
            // Ignored
        }
    }

    /// <see cref="IWireMockLogger.Debug"/>
    public void Debug(string formatString, params object[] args)
    {
        Console.WriteLine(Format("Debug", formatString, args));
    }

    /// <see cref="IWireMockLogger.Info"/>
    public void Info(string formatString, params object[] args)
    {
        Console.WriteLine(Format("Info", formatString, args));
    }

    /// <see cref="IWireMockLogger.Warn"/>
    public void Warn(string formatString, params object[] args)
    {
        Console.WriteLine(Format("Warn", formatString, args));
    }

    /// <see cref="IWireMockLogger.Error(string, object[])"/>
    public void Error(string formatString, params object[] args)
    {
        Console.WriteLine(Format("Error", formatString, args));
    }

    /// <see cref="IWireMockLogger.Error(string, Exception)"/>
    public void Error(string message, Exception exception)
    {
        Console.WriteLine(Format("Error", $"{message} {{0}}", exception));

        if (exception is AggregateException ae)
        {
            ae.Handle(ex =>
            {
                Console.WriteLine(Format("Error", "Exception {0}", ex));
                return true;
            });
        }
    }

    /// <see cref="IWireMockLogger.DebugRequestResponse"/>
    public void DebugRequestResponse(LogEntryModel logEntryModel, bool isAdminRequest)
    {
        string message = JsonConvert.SerializeObject(logEntryModel, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        Console.WriteLine(Format("DebugRequestResponse", "Admin[{0}] {1}", isAdminRequest, message));
    }

    private static string Format(string level, string formatString, params object[] args)
    {
        var message = args.Length > 0 ? string.Format(formatString, args) : formatString;

        return $"{DateTime.UtcNow} [{level}] : {message}";
    }
}