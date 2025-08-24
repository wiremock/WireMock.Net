// Copyright © WireMock.Net

using JetBrains.Annotations;

namespace WireMock.Settings;

/// <summary>
/// ProxySaveMappingSettings
/// </summary>
public class ProxySaveMappingSettings
{
    /// <summary>
    /// Only save request/response to the internal Mappings if the status code is included in this pattern. (Note that SaveMapping must also be set to true.)
    /// The pattern can contain a single value like "200", but also ranges like "2xx", "100,300,600" or "100-299,6xx" are supported.
    /// </summary>
    [PublicAPI]
    public ProxySaveMappingSetting<string>? StatusCodePattern { get; set; } = "*";

    /// <summary>
    /// Only save these Http Methods. (Note that SaveMapping must also be set to true.)
    /// </summary>
    [PublicAPI]
    public ProxySaveMappingSetting<string[]>? HttpMethods { get; set; }
}