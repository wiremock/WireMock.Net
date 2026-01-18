// Copyright © WireMock.Net

using System.Collections;
using Stef.Validation;
using WireMock.Settings;

namespace WireMock.OpenTelemetry;

/// <summary>
/// A static helper class to parse commandline arguments into OpenTelemetryOptions.
/// </summary>
public static class OpenTelemetryOptionsParser
{
    private const string Prefix = "OpenTelemetry";

    /// <summary>
    /// Parse commandline arguments into OpenTelemetryOptions.
    /// </summary>
    /// <param name="args">The commandline arguments</param>
    /// <param name="environment">The environment settings (optional)</param>
    /// <param name="options">The parsed options, or null if OpenTelemetry is not enabled</param>
    /// <returns>Always returns true.</returns>
    public static bool TryParseArguments(string[] args, IDictionary? environment, out OpenTelemetryOptions? options)
    {
        Guard.HasNoNulls(args);

        var parser = new SimpleSettingsParser();
        parser.Parse(args, environment);

        if (!parser.GetBoolValue($"{Prefix}Enabled"))
        {
            options = null;
            return true;
        }

        options = new OpenTelemetryOptions
        {
            ExcludeAdminRequests = parser.GetBoolValue($"{Prefix}ExcludeAdminRequests", defaultValue: true),
            OtlpExporterEndpoint = parser.GetStringValue($"{Prefix}OtlpExporterEndpoint")
        };

        return true;
    }
}
