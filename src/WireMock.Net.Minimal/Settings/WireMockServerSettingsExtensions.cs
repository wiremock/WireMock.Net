// Copyright © WireMock.Net

using JetBrains.Annotations;
using Scriban.Runtime;
using Stef.Validation;
using WireMock.Handlers;

namespace WireMock.Settings;

/// <summary>
/// Extension methods for <see cref="WireMockServerSettings"/>.
/// </summary>
public static class WireMockServerSettingsExtensions
{
    /// <summary>
    /// Adds a strongly typed Scriban registration to <see cref="WireMockServerSettings.ScribanRegistrationCallback"/>.
    /// </summary>
    /// <remarks>
    /// Members added to the <see cref="ScriptObject"/> are available as globals in Scriban and Scriban-DotLiquid templates.
    /// When a callback is already configured, it is kept and invoked before <paramref name="registrationCallback"/>.
    /// </remarks>
    /// <param name="settings">The settings.</param>
    /// <param name="registrationCallback">The action which registers functions, objects or variables on the <see cref="ScriptObject"/>.</param>
    /// <returns>The same <see cref="WireMockServerSettings"/> instance.</returns>
    [PublicAPI]
    public static WireMockServerSettings WithScribanRegistration(this WireMockServerSettings settings, Action<ScriptObject, IFileSystemHandler> registrationCallback)
    {
        Guard.NotNull(settings);
        Guard.NotNull(registrationCallback);

        var existingCallback = settings.ScribanRegistrationCallback;
        settings.ScribanRegistrationCallback = (scriptObject, fileSystemHandler) =>
        {
            existingCallback?.Invoke(scriptObject, fileSystemHandler);
            registrationCallback((ScriptObject)scriptObject, fileSystemHandler);
        };

        return settings;
    }
}
