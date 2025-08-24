// Copyright © WireMock.Net

using JetBrains.Annotations;

namespace WireMock.Settings;

/// <summary>
/// ProxyAndRecordSettings
/// </summary>
public class ProxyAndRecordSettings : HttpClientSettings
{
    /// <summary>
    /// Default prefix value for saved mapping file
    /// </summary>
    public const string DefaultPrefixForSavedMappingFile = "Proxy Mapping for ";

    /// <summary>
    /// The URL to proxy.
    /// </summary>
    [PublicAPI]
    public string Url { get; set; } = null!;

    /// <summary>
    /// Save the mapping for each request/response to the internal Mappings.
    /// </summary>
    [PublicAPI]
    public bool SaveMapping { get; set; }

    /// <summary>
    /// Save the mapping for each request/response also to a file. (Note that SaveMapping must also be set to true.)
    /// </summary>
    [PublicAPI]
    public bool SaveMappingToFile { get; set; }

    /// <summary>
    /// Only save request/response to the internal Mappings if the status code is included in this pattern. (Note that SaveMapping must also be set to true.)
    /// The pattern can contain a single value like "200", but also ranges like "2xx", "100,300,600" or "100-299,6xx" are supported.
    ///
    /// Deprecated : use SaveMappingSettings.
    /// </summary>
    [PublicAPI]
    public string SaveMappingForStatusCodePattern
    {
        set
        {
            if (SaveMappingSettings is null)
            {
                SaveMappingSettings = new ProxySaveMappingSettings();
            }

            SaveMappingSettings.StatusCodePattern = value;
        }
    }

    /// <summary>
    /// Additional SaveMappingSettings.
    /// </summary>
    [PublicAPI]
    public ProxySaveMappingSettings? SaveMappingSettings { get; set; }

    /// <summary>
    /// Defines a list from headers which will be excluded from the saved mappings.
    /// </summary>
    [PublicAPI]
    public string[]? ExcludedHeaders { get; set; }

    /// <summary>
    /// Defines a list of params which will be excluded from the saved mappings.
    /// </summary>
    [PublicAPI]
    public string[]? ExcludedParams { get; set; }

    /// <summary>
    /// Defines a list of cookies which will be excluded from the saved mappings.
    /// </summary>
    [PublicAPI]
    public string[]? ExcludedCookies { get; set; }

    /// <summary>
    /// Replace Settings
    /// </summary>
    [PublicAPI]
    public ProxyUrlReplaceSettings? ReplaceSettings { get; set; }

    /// <summary>
    /// Prefer the Proxy Mapping over the saved Mapping (in case SaveMapping is set to <c>true</c>).
    /// </summary>
    //[PublicAPI]
    //public bool PreferProxyMapping { get; set; }

    /// <summary>
    /// When SaveMapping is set to <c>true</c>, this setting can be used to control the behavior of the generated request matchers for the new mapping.
    /// - <c>false</c>, the default matchers will be used.
    /// - <c>true</c>, the defined mappings in the request wil be used for the new mapping.
    ///
    /// Default value is false.
    /// </summary>
    public bool UseDefinedRequestMatchers { get; set; }

    /// <summary>
    /// Append a unique GUID to the filename from the saved mapping file.
    /// </summary>
    public bool AppendGuidToSavedMappingFile { get; set; }

    /// <summary>
    /// Set prefix for saved mapping file.
    /// </summary>
    public string PrefixForSavedMappingFile { get; set; } = DefaultPrefixForSavedMappingFile;

    /// <summary>
    /// Proxy all Api calls, irrespective of any condition
    /// </summary>
    [PublicAPI]
    public bool ProxyAll { get; set; }
}