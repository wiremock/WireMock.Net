// Copyright © WireMock.Net

using System.Collections.Generic;
using System.Text.RegularExpressions;
using WireMock.Handlers;
using WireMock.Types;

namespace WireMock.Admin.Settings;

/// <summary>
/// Settings
/// </summary>
[FluentBuilder.AutoGenerateBuilder]
public class SettingsModel
{
    /// <summary>
    /// Gets or sets the global delay in milliseconds.
    /// </summary>
    public int? GlobalProcessingDelay { get; set; }

    /// <summary>
    /// Gets or sets if partial mapping is allowed.
    /// </summary>
    public bool? AllowPartialMapping { get; set; }

    /// <summary>
    /// Gets or sets the RequestLog expiration in hours
    /// </summary>
    public int? RequestLogExpirationDuration { get; set; }

    /// <summary>
    /// Gets or sets the MaxRequestLog count.
    /// </summary>
    public int? MaxRequestLogCount { get; set; }

    /// <summary>
    /// Allow a Body for all HTTP Methods. (default set to <c>false</c>).
    /// </summary>
    public bool? AllowBodyForAllHttpMethods { get; set; }

    /// <summary>
    /// Allow only a HttpStatus Code in the response which is defined. (default set to <c>false</c>).
    /// - false : also null, 0, empty or invalid HttpStatus codes are allowed.
    /// - true  : only codes defined in <see cref="System.Net.HttpStatusCode"/> are allowed.
    /// </summary>
    public bool? AllowOnlyDefinedHttpStatusCodeInResponse { get; set; }

    /// <summary>
    /// Set to true to disable Json deserialization when processing requests. (default set to <c>false</c>).
    /// </summary>
    public bool? DisableJsonBodyParsing { get; set; }

    /// <summary>
    /// Disable support for GZip and Deflate request body decompression. (default set to <c>false</c>).
    /// </summary>
    public bool? DisableRequestBodyDecompressing { get; set; }

    /// <summary>
    /// Set to true to disable FormUrlEncoded deserializing when processing requests. (default set to <c>false</c>).
    /// </summary>
    public bool? DisableDeserializeFormUrlEncoded { get; set; }

    /// <summary>
    /// Handle all requests synchronously. (default set to <c>false</c>).
    /// </summary>
    public bool? HandleRequestsSynchronously { get; set; }

    /// <summary>
    /// Use the RegexExtended instead of the default <see cref="Regex"/>.  (default set to <c>true</c>).
    /// </summary>
    public bool? UseRegexExtended { get; set; }

    /// <summary>
    /// Save unmatched requests to a file using the <see cref="IFileSystemHandler"/>. (default set to <c>false</c>).
    /// </summary>
    public bool? SaveUnmatchedRequests { get; set; }

    /// <summary>
    /// Gets or sets if the static mappings should be read at startup.
    /// </summary>
    public bool? ReadStaticMappings { get; set; }

    /// <summary>
    /// Watch the static mapping files + folder for changes when running.
    /// </summary>
    public bool? WatchStaticMappings { get; set; }

    /// <summary>
    /// A value indicating whether subdirectories within the static mappings path should be monitored.
    /// </summary>
    public bool? WatchStaticMappingsInSubdirectories { get; set; }

    /// <summary>
    /// Policies to use when using CORS. By default CORS is disabled. [Optional]
    /// </summary>
    public string? CorsPolicyOptions { get; set; }

    /// <summary>
    /// The proxy and record settings.
    /// </summary>
    public ProxyAndRecordSettingsModel? ProxyAndRecordSettings { get; set; }

    /// <summary>
    /// Defines on which scheme (http/https) to host. (This overrides the <c>UseSSL</c> value).
    /// </summary>
    public HostingScheme? HostingScheme { get; set; }

    /// <summary>
    /// Don't save the response-string in the LogEntry when WithBody(Func{IRequestMessage, string}) or WithBody(Func{IRequestMessage, Task{string}}) is used. (default set to <c>false</c>).
    /// </summary>
    public bool? DoNotSaveDynamicResponseInLogEntry { get; set; }

    /// <summary>
    /// See <seealso cref="QueryParameterMultipleValueSupport"/>.
    ///
    /// Default value = "All".
    /// </summary>
    public QueryParameterMultipleValueSupport? QueryParameterMultipleValueSupport { get; set; }

    /// <summary>
    /// A list of Grpc ProtoDefinitions which can be used.
    /// </summary>
    public Dictionary<string, string[]>? ProtoDefinitions { get; set; }

    /// <summary>
    /// Server client certificate mode
    /// </summary>
    public ClientCertificateMode ClientCertificateMode { get; set; }

    /// <summary>
    /// Whether to accept any client certificate
    /// </summary>
    public bool AcceptAnyClientCertificate { get; set; }
}