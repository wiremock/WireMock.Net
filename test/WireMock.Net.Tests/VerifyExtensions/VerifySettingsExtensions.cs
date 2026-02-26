// Copyright © WireMock.Net

namespace WireMock.Net.Tests.VerifyExtensions;

internal static class VerifySettingsExtensions
{
    public static void Init(this VerifySettings verifySettings)
    {
        verifySettings.DontScrubDateTimes();
        verifySettings.DontScrubDateTimes();
        // verifySettings.UseDirectory($"{typeof(T).Name}.Verify");

        VerifyNewtonsoftJson.Enable(verifySettings);
    }
}