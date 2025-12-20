// Copyright © WireMock.Net

using System;
using JsonConverter.Abstractions;
using JsonConverter.Newtonsoft.Json;
using NUnit.Framework;
using WireMock.Admin.Requests;
using WireMock.Logging;

namespace WireMock.Net.NUnit;

/// <summary>
/// When using NUnit, this class enables to log the output from WireMock.Net using the <see cref="TestContext"/>.
/// </summary>
public sealed class TestContextWireMockLogger(IJsonConverter? jsonConverter = null) : IWireMockLogger
{
    private readonly JsonConverterOptions _jsonConverterOptions = new() { WriteIndented = true, IgnoreNullValues = true };
    private readonly IJsonConverter _jsonConverter = jsonConverter ?? new NewtonsoftJsonConverter();

    /// <inheritdoc />
    public void Debug(string formatString, params object[] args)
    {
        TestContext.WriteLine(Format("Debug", formatString, args));
    }

    /// <inheritdoc />
    public void Info(string formatString, params object[] args)
    {
        TestContext.WriteLine(Format("Info", formatString, args));
    }

    /// <inheritdoc />
    public void Warn(string formatString, params object[] args)
    {
        TestContext.WriteLine(Format("Warning", formatString, args));
    }

    /// <inheritdoc />
    public void Error(string formatString, params object[] args)
    {
        TestContext.WriteLine(Format("Error", formatString, args));
    }

    /// <inheritdoc />
    public void Error(string message, Exception exception)
    {
        TestContext.WriteLine(Format("Error", $"{message} {{0}}", exception));

        if (exception is AggregateException ae)
        {
            ae.Handle(ex =>
            {
                TestContext.WriteLine(Format("Error", "Exception {0}", ex));
                return true;
            });
        }
    }

    /// <inheritdoc />
    public void DebugRequestResponse(LogEntryModel logEntryModel, bool isAdminRequest)
    {
        var message = _jsonConverter.Serialize(logEntryModel, _jsonConverterOptions);
        TestContext.WriteLine(Format("DebugRequestResponse", "Admin[{0}] {1}", isAdminRequest, message));
    }

    private static string Format(string level, string formatString, params object[] args)
    {
        var message = args.Length > 0 ? string.Format(formatString, args) : formatString;
        return $"{DateTime.UtcNow} [{level}] : {message}";
    }
}