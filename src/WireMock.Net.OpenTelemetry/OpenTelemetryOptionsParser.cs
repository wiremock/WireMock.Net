// Copyright © WireMock.Net

using System.Collections;
using System.Diagnostics.CodeAnalysis;
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
    public static bool TryParseArguments(string[] args, IDictionary? environment, [NotNullWhen(true)] out OpenTelemetryOptions? options)
    {
        Guard.HasNoNulls(args);

        var parser = new SimpleSettingsParser();
        parser.Parse(args, environment);

        if (!parser.GetBoolValue($"{Prefix}Enabled"))
        {
            options = null;
            return false;
        }

        var activitySourceName = parser.GetStringValue($"{Prefix}ActivitySourceName");
        options = new OpenTelemetryOptions
        {
            ActivitySourceName = string.IsNullOrWhiteSpace(activitySourceName) ? OpenTelemetryOptions.DefaultActivitySourceName : activitySourceName!,
            ExcludeAdminRequests = parser.GetBoolValue($"{Prefix}ExcludeAdminRequests", defaultValue: true),
            OtlpExporterEndpoint = parser.GetStringValue($"{Prefix}OtlpExporterEndpoint")
        };

        return true;
    }
}
