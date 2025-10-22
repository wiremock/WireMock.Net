// Copyright © WireMock.Net

using System;
using System.Collections.Concurrent;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using WireMock.Handlers;
using WireMock.Logging;
using WireMock.Matchers;
using WireMock.Types;
using WireMock.Util;
using ClientCertificateMode = Microsoft.AspNetCore.Server.Kestrel.Https.ClientCertificateMode;

//#if !USE_ASPNETCORE
//using Owin;
//#else
//using IAppBuilder = Microsoft.AspNetCore.Builder.IApplicationBuilder;
//using Microsoft.Extensions.DependencyInjection;
//#endif

namespace WireMock.Owin;

internal interface IWireMockMiddlewareOptions
{
    IWireMockLogger Logger { get; set; }

    TimeSpan? RequestProcessingDelay { get; set; }

    IStringMatcher? AuthenticationMatcher { get; set; }

    bool? AllowPartialMapping { get; set; }

    ConcurrentDictionary<Guid, IMapping> Mappings { get; }

    ConcurrentDictionary<string, ScenarioState> Scenarios { get; }

    ConcurrentObservableCollection<LogEntry> LogEntries { get; }

    int? RequestLogExpirationDuration { get; set; }

    int? MaxRequestLogCount { get; set; }

    Action<IApplicationBuilder>? PreWireMockMiddlewareInit { get; set; }

    Action<IApplicationBuilder>? PostWireMockMiddlewareInit { get; set; }

//#if USE_ASPNETCORE
    Action<IServiceCollection>? AdditionalServiceRegistration { get; set; }

    CorsPolicyOptions? CorsPolicyOptions { get; set; }

    ClientCertificateMode ClientCertificateMode { get; set; }

    bool AcceptAnyClientCertificate { get; set; }
//#endif

    IFileSystemHandler? FileSystemHandler { get; set; }

    bool? AllowBodyForAllHttpMethods { get; set; }

    bool? AllowOnlyDefinedHttpStatusCodeInResponse { get; set; }

    bool? DisableJsonBodyParsing { get; set; }

    bool? DisableRequestBodyDecompressing { get; set; }

    bool? HandleRequestsSynchronously { get; set; }

    string? X509StoreName { get; set; }

    string? X509StoreLocation { get; set; }

    string? X509ThumbprintOrSubjectName { get; set; }

    string? X509CertificateFilePath { get; set; }

    /// <summary>
    /// A X.509 certificate instance.
    /// </summary>
    public X509Certificate2? X509Certificate { get; set; }

    string? X509CertificatePassword { get; set; }

    bool CustomCertificateDefined { get; }

    bool? SaveUnmatchedRequests { get; set; }

    bool? DoNotSaveDynamicResponseInLogEntry { get; set; }

    QueryParameterMultipleValueSupport? QueryParameterMultipleValueSupport { get; set; }

    public bool ProxyAll { get; set; }
}