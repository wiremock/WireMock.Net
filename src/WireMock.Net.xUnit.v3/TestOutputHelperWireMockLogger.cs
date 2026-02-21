// Copyright © WireMock.Net

using System;
using Newtonsoft.Json;
using Stef.Validation;
using WireMock.Admin.Requests;
using WireMock.Logging;
using Xunit;

namespace WireMock.Net.Xunit;

/// <summary>
/// When using xUnit, this class enables to log the output from WireMock.Net to the <see cref="ITestOutputHelper"/>.
/// </summary>
public sealed class TestOutputHelperWireMockLogger : IWireMockLogger
{
    private readonly ITestOutputHelper _testOutputHelper;

    /// <summary>
    /// Create a new instance on the <see cref="TestOutputHelperWireMockLogger"/>.
    /// </summary>
    /// <param name="testOutputHelper">Represents a class which can be used to provide test output.</param>
    public TestOutputHelperWireMockLogger(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = Guard.NotNull(testOutputHelper);
    }

    /// <inheritdoc />
    public void Debug(string formatString, params object[] args)
    {
        _testOutputHelper.WriteLine(Format("Debug", formatString, args));
    }

    /// <inheritdoc />
    public void Info(string formatString, params object[] args)
    {
        _testOutputHelper.WriteLine(Format("Info", formatString, args));
    }

    /// <inheritdoc />
    public void Warn(string formatString, params object[] args)
    {
        _testOutputHelper.WriteLine(Format("Warning", formatString, args));
    }

    /// <inheritdoc />
    public void Error(string formatString, params object[] args)
    {
        _testOutputHelper.WriteLine(Format("Error", formatString, args));
    }

    /// <inheritdoc />
    public void Error(string message, Exception exception)
    {
        _testOutputHelper.WriteLine(Format("Error", $"{message} {{0}}", exception));

        if (exception is AggregateException ae)
        {
            ae.Handle(ex =>
            {
                _testOutputHelper.WriteLine(Format("Error", "Exception {0}", ex));
                return true;
            });
        }
    }

    /// <inheritdoc />
    public void DebugRequestResponse(LogEntryModel logEntryModel, bool isAdminRequest)
    {
        var message = JsonConvert.SerializeObject(logEntryModel, new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore
        });
        _testOutputHelper.WriteLine(Format("DebugRequestResponse", "Admin[{0}] {1}", isAdminRequest, message));
    }

    private static string Format(string level, string formatString, params object[] args)
    {
        var message = args.Length > 0 ? string.Format(formatString, args) : formatString;
        return $"{DateTime.UtcNow} [{level}] : {message}";
    }
}