// Copyright © WireMock.Net

using System;
using System.Collections.Concurrent;
using WireMock.Handlers;
using WireMock.Logging;
using WireMock.Matchers;
using WireMock.Types;
using WireMock.Util;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.DependencyInjection;
using ClientCertificateMode = Microsoft.AspNetCore.Server.Kestrel.Https.ClientCertificateMode;

//#if !USE_ASPNETCORE
//using Owin;
//#else
//using IAppBuilder = Microsoft.AspNetCore.Builder.IApplicationBuilder;
//using Microsoft.Extensions.DependencyInjection;
//#endif

namespace WireMock.Owin;

internal class WireMockMiddlewareOptions : IWireMockMiddlewareOptions
{
    public IWireMockLogger Logger { get; set; }

    public TimeSpan? RequestProcessingDelay { get; set; }

    public IStringMatcher? AuthenticationMatcher { get; set; }

    public bool? AllowPartialMapping { get; set; }

    public ConcurrentDictionary<Guid, IMapping> Mappings { get; } = new ConcurrentDictionary<Guid, IMapping>();

    public ConcurrentDictionary<string, ScenarioState> Scenarios { get; } = new(StringComparer.OrdinalIgnoreCase);

    public ConcurrentObservableCollection<LogEntry> LogEntries { get; } = new();

    public int? RequestLogExpirationDuration { get; set; }

    public int? MaxRequestLogCount { get; set; }

    public Action<IApplicationBuilder>? PreWireMockMiddlewareInit { get; set; }

    public Action<IApplicationBuilder>? PostWireMockMiddlewareInit { get; set; }

//#if USE_ASPNETCORE
    public Action<IServiceCollection>? AdditionalServiceRegistration { get; set; }

    public CorsPolicyOptions? CorsPolicyOptions { get; set; }

    public ClientCertificateMode ClientCertificateMode { get; set; }

    /// <inheritdoc />
    public bool AcceptAnyClientCertificate { get; set; }
    //#endif

    /// <inheritdoc cref="IWireMockMiddlewareOptions.FileSystemHandler"/>
    public IFileSystemHandler? FileSystemHandler { get; set; }

    /// <inheritdoc cref="IWireMockMiddlewareOptions.AllowBodyForAllHttpMethods"/>
    public bool? AllowBodyForAllHttpMethods { get; set; }

    /// <inheritdoc cref="IWireMockMiddlewareOptions.AllowOnlyDefinedHttpStatusCodeInResponse"/>
    public bool? AllowOnlyDefinedHttpStatusCodeInResponse { get; set; }

    /// <inheritdoc cref="IWireMockMiddlewareOptions.DisableJsonBodyParsing"/>
    public bool? DisableJsonBodyParsing { get; set; }

    /// <inheritdoc cref="IWireMockMiddlewareOptions.DisableRequestBodyDecompressing"/>
    public bool? DisableRequestBodyDecompressing { get; set; }

    /// <inheritdoc cref="IWireMockMiddlewareOptions.HandleRequestsSynchronously"/>
    public bool? HandleRequestsSynchronously { get; set; }

    /// <inheritdoc cref="IWireMockMiddlewareOptions.X509StoreName"/>
    public string? X509StoreName { get; set; }

    /// <inheritdoc cref="IWireMockMiddlewareOptions.X509StoreLocation"/>
    public string? X509StoreLocation { get; set; }

    /// <inheritdoc cref="IWireMockMiddlewareOptions.X509ThumbprintOrSubjectName"/>
    public string? X509ThumbprintOrSubjectName { get; set; }

    /// <inheritdoc cref="IWireMockMiddlewareOptions.X509CertificateFilePath"/>
    public string? X509CertificateFilePath { get; set; }

    /// <inheritdoc />
    public X509Certificate2? X509Certificate { get; set; }

    /// <inheritdoc cref="IWireMockMiddlewareOptions.X509CertificatePassword"/>
    public string? X509CertificatePassword { get; set; }

    /// <inheritdoc cref="IWireMockMiddlewareOptions.CustomCertificateDefined"/>
    public bool CustomCertificateDefined =>
        !string.IsNullOrEmpty(X509StoreName) && !string.IsNullOrEmpty(X509StoreLocation) ||
        !string.IsNullOrEmpty(X509CertificateFilePath) ||
        X509Certificate != null;

    /// <inheritdoc cref="IWireMockMiddlewareOptions.SaveUnmatchedRequests"/>
    public bool? SaveUnmatchedRequests { get; set; }

    /// <inheritdoc />
    public bool? DoNotSaveDynamicResponseInLogEntry { get; set; }

    /// <inheritdoc />
    public QueryParameterMultipleValueSupport? QueryParameterMultipleValueSupport { get; set; }

    /// <inheritdoc />
    public bool ProxyAll { get; set; }
}