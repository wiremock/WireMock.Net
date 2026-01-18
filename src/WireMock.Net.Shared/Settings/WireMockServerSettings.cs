// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using HandlebarsDotNet;
using JetBrains.Annotations;
using Newtonsoft.Json;
using WireMock.Admin.Mappings;
using WireMock.Handlers;
using WireMock.Logging;
using WireMock.Matchers;
using WireMock.RegularExpressions;
using WireMock.Types;
using System.Globalization;
using WireMock.Models;
using Microsoft.Extensions.DependencyInjection;

namespace WireMock.Settings;

/// <summary>
/// WireMockServerSettings
/// </summary>
public class WireMockServerSettings
{
    internal const int DefaultStartTimeout = 10000;

    /// <summary>
    /// Gets or sets the http port.
    /// </summary>
    [PublicAPI]
    public int? Port { get; set; }

    /// <summary>
    /// Gets or sets the use SSL.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    [PublicAPI]
    public bool? UseSSL { get; set; }

    /// <summary>
    /// Defines on which scheme (http/https) to host. (This overrides the <c>UseSSL</c> value).
    /// </summary>
    [PublicAPI]
    public HostingScheme? HostingScheme { get; set; }

    /// <summary>
    /// Gets or sets to use HTTP 2 (used for Grpc).
    /// </summary>
    [PublicAPI]
    public bool? UseHttp2 { get; set; }

    /// <summary>
    /// Gets or sets whether to start admin interface.
    /// </summary>
    [PublicAPI]
    public bool? StartAdminInterface { get; set; }

    /// <summary>
    /// Gets or sets if the static mappings should be read at startup.
    /// </summary>
    [PublicAPI]
    public bool? ReadStaticMappings { get; set; }

    /// <summary>
    /// Watch the static mapping files + folder for changes when running.
    /// </summary>
    [PublicAPI]
    public bool? WatchStaticMappings { get; set; }

    /// <summary>
    /// A value indicating whether subdirectories within the static mappings path should be monitored.
    /// </summary>
    [PublicAPI]
    public bool? WatchStaticMappingsInSubdirectories { get; set; }

    /// <summary>
    /// Gets or sets if the proxy and record settings.
    /// </summary>
    [PublicAPI]
    public ProxyAndRecordSettings? ProxyAndRecordSettings { get; set; }

    /// <summary>
    /// Gets or sets the urls.
    /// </summary>
    [PublicAPI]
    public string[]? Urls { get; set; }

    /// <summary>
    /// StartTimeout
    /// </summary>
    [PublicAPI]
    public int StartTimeout { get; set; } = DefaultStartTimeout;

    /// <summary>
    /// Allow Partial Mapping (default set to false).
    /// </summary>
    [PublicAPI]
    public bool? AllowPartialMapping { get; set; }

    /// <summary>
    /// The username needed for __admin access.
    /// </summary>
    [PublicAPI]
    public string? AdminUsername { get; set; }

    /// <summary>
    /// The password needed for __admin access.
    /// </summary>
    [PublicAPI]
    public string? AdminPassword { get; set; }

    /// <summary>
    /// The AzureAD Tenant needed for __admin access.
    /// </summary>
    [PublicAPI]
    public string? AdminAzureADTenant { get; set; }

    /// <summary>
    /// The AzureAD Audience / Resource for __admin access.
    /// </summary>
    [PublicAPI]
    public string? AdminAzureADAudience { get; set; }

    /// <summary>
    /// The RequestLog expiration in hours (optional).
    /// </summary>
    [PublicAPI]
    public int? RequestLogExpirationDuration { get; set; }

    /// <summary>
    /// The MaxRequestLog count (optional).
    /// </summary>
    [PublicAPI]
    public int? MaxRequestLogCount { get; set; }

    /// <summary>
    /// Action which is called (with the IAppBuilder or IApplicationBuilder) before the internal WireMockMiddleware is initialized. [Optional]
    /// </summary>
    [PublicAPI]
    [JsonIgnore]
    public Action<object>? PreWireMockMiddlewareInit { get; set; }

    /// <summary>
    /// Action which is called (with the IAppBuilder or IApplicationBuilder) after the internal WireMockMiddleware is initialized. [Optional]
    /// </summary>
    [PublicAPI]
    [JsonIgnore]
    public Action<object>? PostWireMockMiddlewareInit { get; set; }

//#if USE_ASPNETCORE
    /// <summary>
    /// Action which is called with IServiceCollection when ASP.NET Core DI is being configured. [Optional]
    /// </summary>
    [PublicAPI]
    [JsonIgnore]
    public Action<IServiceCollection>? AdditionalServiceRegistration { get; set; }

    /// <summary>
    /// Policies to use when using CORS. By default CORS is disabled. [Optional]
    /// </summary>
    [PublicAPI]
    public CorsPolicyOptions? CorsPolicyOptions { get; set; }
//#endif

    /// <summary>
    /// The IWireMockLogger which logs Debug, Info, Warning or Error
    /// </summary>
    [PublicAPI]
    [JsonIgnore]
    public IWireMockLogger Logger { get; set; } = null!;

    /// <summary>
    /// Handler to interact with the file system to read and write static mapping files.
    /// </summary>
    [PublicAPI]
    [JsonIgnore]
    public IFileSystemHandler FileSystemHandler { get; set; } = null!;

    /// <summary>
    /// Action which can be used to add additional Handlebars registrations. [Optional]
    /// </summary>
    [PublicAPI]
    [JsonIgnore]
    public Action<IHandlebars, IFileSystemHandler>? HandlebarsRegistrationCallback { get; set; }

    /// <summary>
    /// Allow the usage of CSharpCodeMatcher (default is not allowed).
    /// </summary>
    [PublicAPI]
    public bool? AllowCSharpCodeMatcher { get; set; }

    /// <summary>
    /// Allow a Body for all HTTP Methods. (default set to <c>false</c>).
    /// </summary>
    [PublicAPI]
    public bool? AllowBodyForAllHttpMethods { get; set; }

    /// <summary>
    /// Allow only a HttpStatus Code in the response which is defined. (default set to <c>false</c>).
    /// - false : also null, 0, empty or invalid HttpStatus codes are allowed.
    /// - true  : only codes defined in <see cref="System.Net.HttpStatusCode"/> are allowed.
    /// </summary>
    [PublicAPI]
    public bool? AllowOnlyDefinedHttpStatusCodeInResponse { get; set; }

    /// <summary>
    /// Set to true to disable Json deserialization when processing requests. (default set to <c>false</c>).
    /// </summary>
    [PublicAPI]
    public bool? DisableJsonBodyParsing { get; set; }

    /// <summary>
    /// Disable support for GZip and Deflate request body decompression. (default set to <c>false</c>).
    /// </summary>
    [PublicAPI]
    public bool? DisableRequestBodyDecompressing { get; set; }

    /// <summary>
    /// Set to true to disable FormUrlEncoded deserializing when processing requests. (default set to <c>false</c>).
    /// </summary>
    [PublicAPI]
    public bool? DisableDeserializeFormUrlEncoded { get; set; }

    /// <summary>
    /// Handle all requests synchronously. (default set to <c>false</c>).
    /// </summary>
    [PublicAPI]
    public bool? HandleRequestsSynchronously { get; set; }

    /// <summary>
    /// If https is used, these settings can be used to configure the CertificateSettings in case a custom certificate instead the default .NET certificate should be used.
    ///
    /// X509StoreName and X509StoreLocation should be defined
    /// OR
    /// X509CertificateFilePath should be defined
    /// OR
    /// X509Certificate should be defined
    /// </summary>
    [PublicAPI]
    public WireMockCertificateSettings? CertificateSettings { get; set; }

    /// <summary>
    /// Defines if custom CertificateSettings are defined
    /// </summary>
    [PublicAPI]
    public bool CustomCertificateDefined => CertificateSettings?.IsDefined == true;

//#if USE_ASPNETCORE
    /// <summary>
    /// Client certificate mode for the server
    /// </summary>
    [PublicAPI]
    public ClientCertificateMode ClientCertificateMode { get; set; }

    /// <summary>
    /// Whether to accept any client certificate
    /// </summary>
    public bool AcceptAnyClientCertificate { get; set; }
//#endif

    /// <summary>
    /// Defines the global IWebhookSettings to use.
    /// </summary>
    [PublicAPI]
    public WebhookSettings? WebhookSettings { get; set; }

    /// <summary>
    /// Use the <see cref="RegexExtended"/> instead of the default <see cref="Regex"/> (default set to <c>true</c>).
    /// </summary>
    [PublicAPI]
    public bool? UseRegexExtended { get; set; } = true;

    /// <summary>
    /// Save unmatched requests to a file using the <see cref="IFileSystemHandler"/> (default set to <c>false</c>).
    /// </summary>
    [PublicAPI]
    public bool? SaveUnmatchedRequests { get; set; }

    /// <summary>
    /// Don't save the response-string in the LogEntry when WithBody(Func{IRequestMessage, string}) or WithBody(Func{IRequestMessage, Task{string}}) is used. (default set to <c>false</c>).
    /// </summary>
    [PublicAPI]
    public bool? DoNotSaveDynamicResponseInLogEntry { get; set; }

    /// <summary>
    /// See <seealso cref="QueryParameterMultipleValueSupport"/>.
    ///
    /// Default value = "All".
    /// </summary>
    [PublicAPI]
    public QueryParameterMultipleValueSupport? QueryParameterMultipleValueSupport { get; set; }

    /// <summary>
    /// Custom matcher mappings for static mappings
    /// </summary>
    [PublicAPI, JsonIgnore]
    public IDictionary<string, Func<MatcherModel, IMatcher>>? CustomMatcherMappings { get; set; }

    /// <summary>
    /// The <see cref="JsonSerializerSettings"/> used when the JSON response is generated.
    /// </summary>
    [PublicAPI, JsonIgnore]
    public JsonSerializerSettings? JsonSerializerSettings { get; set; }

    /// <summary>
    /// The Culture to use.
    /// Currently used for:
    /// - Handlebars Transformer
    /// </summary>
	[JsonIgnore]
    public CultureInfo Culture { get; set; } = CultureInfo.CurrentCulture;

    /// <summary>
    /// A list of Grpc ProtoDefinitions which can be used.
    /// </summary>
    [PublicAPI]
    public Dictionary<string, string[]>? ProtoDefinitions { get; set; }

    /// <summary>
    /// A list of GraphQL Schemas which can be used.
    /// </summary>
    [PublicAPI]
    public Dictionary<string, GraphQLSchemaDetails>? GraphQLSchemas { get; set; }

    /// <summary>
    /// The admin path to use for accessing the Admin REST interface.
    /// If not set <c>__/admin</c> is used.
    /// </summary>
    [PublicAPI]
    public string? AdminPath { get; set; }

    /// <summary>
    /// Defines the additional Handlebars Settings.
    /// </summary>
    [PublicAPI]
    public HandlebarsSettings? HandlebarsSettings { get; set; }

    /// <summary>
    /// Gets or sets the activity tracing options.
    /// When set (not null), WireMock.Net will create System.Diagnostics.Activity objects for request processing.
    /// </summary>
    /// <remarks>
    /// To export traces to an OpenTelemetry collector, install the WireMock.Net.OpenTelemetry package
    /// and configure the exporter using the provided extension methods.
    /// </remarks>
    [PublicAPI]
    public ActivityTracingOptions? ActivityTracingOptions { get; set; }
}