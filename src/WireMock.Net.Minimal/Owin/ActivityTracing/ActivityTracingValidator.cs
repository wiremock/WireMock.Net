// Copyright © WireMock.Net

#if !ACTIVITY_TRACING_SUPPORTED
using System;
#endif
using WireMock.Settings;

namespace WireMock.Owin.ActivityTracing;

/// <summary>
/// Validator for Activity Tracing configuration.
/// </summary>
internal static class ActivityTracingValidator
{
    /// <summary>
    /// Validates that Activity Tracing is supported on the current framework.
    /// Throws an exception if ActivityTracingOptions is configured on an unsupported framework.
    /// </summary>
    /// <param name="settings">The WireMock server settings to validate.</param>
    /// <exception cref="System.InvalidOperationException">
    /// Thrown when ActivityTracingOptions is configured but the current framework does not support System.Diagnostics.Activity.
    /// </exception>
    public static void ValidateActivityApiPresence(WireMockServerSettings settings)
    {
#if !ACTIVITY_TRACING_SUPPORTED
        if (settings.ActivityTracingOptions is not null)
        {
            throw new InvalidOperationException(
                "Activity Tracing is not supported on this target framework. " +
                "It requires .NET 5.0 or higher which includes System.Diagnostics.Activity support.");
        }
#endif
    }
}
