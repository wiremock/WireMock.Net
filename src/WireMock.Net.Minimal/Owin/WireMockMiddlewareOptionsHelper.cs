// Copyright © WireMock.Net

using System;
using Stef.Validation;
using WireMock.Owin.ActivityTracing;
using WireMock.Settings;

namespace WireMock.Owin;

internal static class WireMockMiddlewareOptionsHelper
{
    public static IWireMockMiddlewareOptions InitFromSettings(
        WireMockServerSettings settings,
        IWireMockMiddlewareOptions? options = null,
        Action<IWireMockMiddlewareOptions>? postConfigure = null
    )
    {
        Guard.NotNull(settings);

        options ??= new WireMockMiddlewareOptions();

        options.ActivityTracingOptions = settings.ActivityTracingOptions;
        options.AllowBodyForAllHttpMethods = settings.AllowBodyForAllHttpMethods;
        options.AllowOnlyDefinedHttpStatusCodeInResponse = settings.AllowOnlyDefinedHttpStatusCodeInResponse;
        options.AllowPartialMapping = settings.AllowPartialMapping;
        options.DisableJsonBodyParsing = settings.DisableJsonBodyParsing;
        options.DisableRequestBodyDecompressing = settings.DisableRequestBodyDecompressing;
        options.DoNotSaveDynamicResponseInLogEntry = settings.DoNotSaveDynamicResponseInLogEntry;
        options.FileSystemHandler = settings.FileSystemHandler;
        options.HandleRequestsSynchronously = settings.HandleRequestsSynchronously;
        options.Logger = settings.Logger;
        options.MaxRequestLogCount = settings.MaxRequestLogCount;
        options.PostWireMockMiddlewareInit = settings.PostWireMockMiddlewareInit;
        options.PreWireMockMiddlewareInit = settings.PreWireMockMiddlewareInit;
        options.QueryParameterMultipleValueSupport = settings.QueryParameterMultipleValueSupport;
        options.RequestLogExpirationDuration = settings.RequestLogExpirationDuration;
        options.SaveUnmatchedRequests = settings.SaveUnmatchedRequests;

#if USE_ASPNETCORE
        options.AdditionalServiceRegistration = settings.AdditionalServiceRegistration;
        options.CorsPolicyOptions = settings.CorsPolicyOptions;
        options.ClientCertificateMode = settings.ClientCertificateMode;
        options.AcceptAnyClientCertificate = settings.AcceptAnyClientCertificate;
#endif

        if (settings.CustomCertificateDefined)
        {
            options.X509StoreName = settings.CertificateSettings!.X509StoreName;
            options.X509StoreLocation = settings.CertificateSettings.X509StoreLocation;
            options.X509ThumbprintOrSubjectName = settings.CertificateSettings.X509StoreThumbprintOrSubjectName;
            options.X509Certificate = settings.CertificateSettings.X509Certificate;
            options.X509CertificateFilePath = settings.CertificateSettings.X509CertificateFilePath;
            options.X509CertificatePassword = settings.CertificateSettings.X509CertificatePassword;
        }

        postConfigure?.Invoke(options);

        return options;
    }
}