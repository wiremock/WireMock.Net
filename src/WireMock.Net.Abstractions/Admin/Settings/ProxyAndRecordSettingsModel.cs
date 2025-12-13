// Copyright © WireMock.Net

namespace WireMock.Admin.Settings;

[FluentBuilder.AutoGenerateBuilder]
public class ProxyAndRecordSettingsModel
{
    /// <summary>
    /// The clientCertificate thumbprint or subject name fragment to use.
    /// Example thumbprint : "D2DBF135A8D06ACCD0E1FAD9BFB28678DF7A9818". Example subject name: "www.google.com""
    /// </summary>
    public string? ClientX509Certificate2ThumbprintOrSubjectName { get; set; }

    /// <summary>
    /// Defines the WebProxySettings.
    /// </summary>
    public WebProxySettingsModel? WebProxySettings { get; set; }

    /// <summary>
    /// Proxy requests should follow redirection (30x).
    /// </summary>
    public bool? AllowAutoRedirect { get; set; }

    /// <summary>
    /// The URL to proxy.
    /// </summary>
    public string Url { get; set; } = null!;

    /// <summary>
    /// Save the mapping for each request/response to the internal Mappings.
    /// </summary>
    public bool SaveMapping { get; set; }

    /// <summary>
    /// Save the mapping for each request/response also to a file. (Note that SaveMapping must also be set to true.)
    /// </summary>
    public bool SaveMappingToFile { get; set; }

    /// <summary>
    /// Only save request/response to the internal Mappings if the status code is included in this pattern. (Note that SaveMapping must also be set to true.)
    /// The pattern can contain a single value like "200", but also ranges like "2xx", "100,300,600" or "100-299,6xx" are supported.
    /// </summary>
    public string SaveMappingForStatusCodePattern { get; set; } = "*";

    /// <summary>
    /// Defines a list from headers which will be excluded from the saved mappings.
    /// </summary>
    public string[]? ExcludedHeaders { get; set; }

    /// <summary>
    /// Defines a list of cookies which will be excluded from the saved mappings.
    /// </summary>
    public string[]? ExcludedCookies { get; set; }

    /// <summary>
    /// Prefer the Proxy Mapping over the saved Mapping (in case SaveMapping is set to <c>true</c>).
    /// </summary>
    // public bool PreferProxyMapping { get; set; }

    /// <summary>
    /// When SaveMapping is set to <c>true</c>, this setting can be used to control the behavior of the generated request matchers for the new mapping.
    /// - <c>false</c>, the default matchers will be used.
    /// - <c>true</c>, the defined mappings in the request wil be used for the new mapping.
    ///
    /// Default value is false.
    /// </summary>
    public bool UseDefinedRequestMatchers { get; set; }

    /// <summary>
    /// Append an unique GUID to the filename from the saved mapping file.
    /// </summary>
    public bool AppendGuidToSavedMappingFile { get; set; }

    /// <summary>
    /// Set prefix for saved mapping file.
    /// </summary>
    public string PrefixForSavedMappingFile { get; set; } = "Proxy Mapping for ";

    /// <summary>
    /// Defines the Replace Settings.
    /// </summary>
    public ProxyUrlReplaceSettingsModel? ReplaceSettings { get; set; }

    /// <summary>
    /// Proxy all Api calls, irrespective of any condition
    /// </summary>
    public bool ProxyAll { get; set; }

    /// <summary>
    /// Set to true to disable any body handling when processing responses. (default set to <c>false</c>).
    /// </summary>
    public bool NoBodyHandling { get; set; }
}