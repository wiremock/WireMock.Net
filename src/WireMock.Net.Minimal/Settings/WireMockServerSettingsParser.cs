// Copyright © WireMock.Net

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using Stef.Validation;
using WireMock.Logging;
using WireMock.Models;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.Settings;

/// <summary>
/// A static helper class to parse commandline arguments into WireMockServerSettings.
/// </summary>
public static class WireMockServerSettingsParser
{
    /// <summary>
    /// Parse commandline arguments into WireMockServerSettings.
    /// </summary>
    /// <param name="args">The commandline arguments</param>
    /// <param name="environment">The environment settings (optional)</param>
    /// <param name="logger">The logger (optional, can be null)</param>
    /// <param name="settings">The parsed settings</param>
    [PublicAPI]
    public static bool TryParseArguments(string[] args, IDictionary? environment, [NotNullWhen(true)] out WireMockServerSettings? settings, IWireMockLogger? logger = null)
    {
        Guard.HasNoNulls(args);

        var parser = new SimpleSettingsParser();
        parser.Parse(args, environment);

        if (parser.GetBoolSwitchValue("help"))
        {
            (logger ?? new WireMockConsoleLogger()).Info("See https://wiremock.org/dotnet/wiremock-commandline-parameters/ for details on all commandline options.");
            settings = null;
            return false;
        }

        settings = new WireMockServerSettings
        {
            AdminAzureADAudience = parser.GetStringValue(nameof(WireMockServerSettings.AdminAzureADAudience)),
            AdminAzureADTenant = parser.GetStringValue(nameof(WireMockServerSettings.AdminAzureADTenant)),
            AdminPassword = parser.GetStringValue(nameof(WireMockServerSettings.AdminPassword)),
            AdminUsername = parser.GetStringValue(nameof(WireMockServerSettings.AdminUsername)),
            AdminPath = parser.GetStringValue(nameof(WireMockServerSettings.AdminPath), "/__admin"),
            AllowBodyForAllHttpMethods = parser.GetBoolValue(nameof(WireMockServerSettings.AllowBodyForAllHttpMethods)),
            AllowCSharpCodeMatcher = parser.GetBoolValue(nameof(WireMockServerSettings.AllowCSharpCodeMatcher)),
            AllowOnlyDefinedHttpStatusCodeInResponse = parser.GetBoolValue(nameof(WireMockServerSettings.AllowOnlyDefinedHttpStatusCodeInResponse)),
            AllowPartialMapping = parser.GetBoolValue(nameof(WireMockServerSettings.AllowPartialMapping)),
            Culture = parser.GetValue(nameof(WireMockServerSettings.Culture), strings => CultureInfoUtils.Parse(strings.FirstOrDefault()), CultureInfo.CurrentCulture),
            DisableJsonBodyParsing = parser.GetBoolValue(nameof(WireMockServerSettings.DisableJsonBodyParsing)),
            DisableRequestBodyDecompressing = parser.GetBoolValue(nameof(WireMockServerSettings.DisableRequestBodyDecompressing)),
            DisableDeserializeFormUrlEncoded = parser.GetBoolValue(nameof(WireMockServerSettings.DisableDeserializeFormUrlEncoded)),
            DoNotSaveDynamicResponseInLogEntry = parser.GetBoolValue(nameof(WireMockServerSettings.DoNotSaveDynamicResponseInLogEntry)),
            GraphQLSchemas = parser.GetObjectValueFromJson<Dictionary<string, GraphQLSchemaDetails>>(nameof(settings.GraphQLSchemas)),
            HandleRequestsSynchronously = parser.GetBoolValue(nameof(WireMockServerSettings.HandleRequestsSynchronously)),
            HostingScheme = parser.GetEnumValue<HostingScheme>(nameof(WireMockServerSettings.HostingScheme)),
            MaxRequestLogCount = parser.GetIntValue(nameof(WireMockServerSettings.MaxRequestLogCount)),
            ProtoDefinitions = parser.GetObjectValueFromJson<Dictionary<string, string[]>>(nameof(settings.ProtoDefinitions)),
            QueryParameterMultipleValueSupport = parser.GetEnumValue<QueryParameterMultipleValueSupport>(nameof(WireMockServerSettings.QueryParameterMultipleValueSupport)),
            ReadStaticMappings = parser.GetBoolValue(nameof(WireMockServerSettings.ReadStaticMappings)),
            RequestLogExpirationDuration = parser.GetIntValue(nameof(WireMockServerSettings.RequestLogExpirationDuration)),
            SaveUnmatchedRequests = parser.GetBoolValue(nameof(WireMockServerSettings.SaveUnmatchedRequests)),
            StartAdminInterface = parser.GetBoolValue(nameof(WireMockServerSettings.StartAdminInterface), true),
            StartTimeout = parser.GetIntValue(nameof(WireMockServerSettings.StartTimeout), WireMockServerSettings.DefaultStartTimeout),
            UseHttp2 = parser.GetBoolValue(nameof(WireMockServerSettings.UseHttp2)),
            UseRegexExtended = parser.GetBoolValue(nameof(WireMockServerSettings.UseRegexExtended), true),
            WatchStaticMappings = parser.GetBoolValue(nameof(WireMockServerSettings.WatchStaticMappings)),
            WatchStaticMappingsInSubdirectories = parser.GetBoolValue(nameof(WireMockServerSettings.WatchStaticMappingsInSubdirectories)),
        };

#if USE_ASPNETCORE
        settings.CorsPolicyOptions = parser.GetEnumValue(nameof(WireMockServerSettings.CorsPolicyOptions), CorsPolicyOptions.None);
        settings.ClientCertificateMode = parser.GetEnumValue(nameof(WireMockServerSettings.ClientCertificateMode), ClientCertificateMode.NoCertificate);
        settings.AcceptAnyClientCertificate = parser.GetBoolValue(nameof(WireMockServerSettings.AcceptAnyClientCertificate));
#endif

        ParseLoggerSettings(settings, logger, parser);
        ParsePortSettings(settings, parser);
        ParseProxyAndRecordSettings(settings, parser);
        ParseCertificateSettings(settings, parser);
        ParseHandlebarsSettings(settings, parser);
        ParseActivityTracingSettings(settings, parser);

        return true;
    }

    private static void ParseLoggerSettings(WireMockServerSettings settings, IWireMockLogger? logger, SimpleSettingsParser parser)
    {
        var loggerType = parser.GetStringValue("WireMockLogger");
        switch (loggerType)
        {
            case nameof(WireMockConsoleLogger):
                settings.Logger = new WireMockConsoleLogger();
                break;

            case nameof(WireMockNullLogger):
                settings.Logger = new WireMockNullLogger();
                break;

            default:
                if (logger != null)
                {
                    settings.Logger = logger;
                }
                break;
        }
    }

    private static void ParseProxyAndRecordSettings(WireMockServerSettings settings, SimpleSettingsParser parser)
    {
        var proxyUrl = parser.GetStringValue("ProxyURL") ?? parser.GetStringValue("ProxyUrl");
        if (!string.IsNullOrEmpty(proxyUrl))
        {
            var proxyAndRecordSettings = new ProxyAndRecordSettings
            {
                AllowAutoRedirect = parser.GetBoolValue(nameof(ProxyAndRecordSettings.AllowAutoRedirect)),
                ClientX509Certificate2ThumbprintOrSubjectName = parser.GetStringValue(nameof(ProxyAndRecordSettings.ClientX509Certificate2ThumbprintOrSubjectName)),
                ExcludedCookies = parser.GetValues(nameof(ProxyAndRecordSettings.ExcludedCookies)),
                ExcludedHeaders = parser.GetValues(nameof(ProxyAndRecordSettings.ExcludedHeaders)),
                // PreferProxyMapping = parser.GetBoolValue(nameof(ProxyAndRecordSettings.PreferProxyMapping)),
                SaveMapping = parser.GetBoolValue(nameof(ProxyAndRecordSettings.SaveMapping)),
                SaveMappingForStatusCodePattern = parser.GetStringValue(nameof(ProxyAndRecordSettings.SaveMappingForStatusCodePattern), "*"),
                SaveMappingToFile = parser.GetBoolValue(nameof(ProxyAndRecordSettings.SaveMappingToFile)),
                UseDefinedRequestMatchers = parser.GetBoolValue(nameof(ProxyAndRecordSettings.UseDefinedRequestMatchers)),
                AppendGuidToSavedMappingFile = parser.GetBoolValue(nameof(ProxyAndRecordSettings.AppendGuidToSavedMappingFile)),
                PrefixForSavedMappingFile = parser.GetStringValue(nameof(ProxyAndRecordSettings.PrefixForSavedMappingFile), ProxyAndRecordSettings.DefaultPrefixForSavedMappingFile),
                Url = proxyUrl!,
                SaveMappingSettings = new ProxySaveMappingSettings
                {
                    StatusCodePattern = parser.GetStringValue(nameof(ProxyAndRecordSettings.SaveMappingForStatusCodePattern), "*"),
                    // HttpMethods = new ProxySaveMappingSetting<string[]>(parser.GetValues("DoNotSaveMappingForHttpMethods", new string[0]), MatchBehaviour.RejectOnMatch)
                },
                ProxyAll = parser.GetBoolValue(nameof(ProxyAndRecordSettings.ProxyAll))
            };

            ParseWebProxyAddressSettings(proxyAndRecordSettings, parser);
            ParseProxyUrlReplaceSettings(proxyAndRecordSettings, parser);

            settings.ProxyAndRecordSettings = proxyAndRecordSettings;
        }
    }

    private static void ParsePortSettings(WireMockServerSettings settings, SimpleSettingsParser parser)
    {
        if (parser.Contains(nameof(WireMockServerSettings.Port)))
        {
            settings.Port = parser.GetIntValue(nameof(WireMockServerSettings.Port));
        }
        else if (settings.HostingScheme is null)
        {
            settings.Urls = parser.GetValues(nameof(WireMockServerSettings.Urls), defaultValue: ["http://*:9091/"]);
        }
    }

    private static void ParseCertificateSettings(WireMockServerSettings settings, SimpleSettingsParser parser)
    {
        var certificateSettings = new WireMockCertificateSettings
        {
            X509StoreName = parser.GetStringValue("X509StoreName"),
            X509StoreLocation = parser.GetStringValue("X509StoreLocation"),
            X509StoreThumbprintOrSubjectName = parser.GetStringValue("X509StoreThumbprintOrSubjectName"),
            X509CertificateFilePath = parser.GetStringValue("X509CertificateFilePath"),
            X509CertificatePassword = parser.GetStringValue("X509CertificatePassword")
        };

        if (certificateSettings.IsDefined)
        {
            settings.CertificateSettings = certificateSettings;
        }
    }

    private static void ParseHandlebarsSettings(WireMockServerSettings settings, SimpleSettingsParser parser)
    {
        if (parser.ContainsAny(nameof(HandlebarsSettings.AllowedCustomHandlebarsHelpers), nameof(HandlebarsSettings.AllowedHandlebarsHelpers)))
        {
            settings.HandlebarsSettings = new HandlebarsSettings
            {
                AllowedCustomHandlebarsHelpers = parser.GetEnumValue(nameof(HandlebarsSettings.AllowedCustomHandlebarsHelpers), CustomHandlebarsHelpers.None),
                AllowedHandlebarsHelpers = parser.GetEnumValues(nameof(HandlebarsSettings.AllowedHandlebarsHelpers), HandlebarsSettings.DefaultAllowedHandlebarsHelpers)
            };
        }
    }

    private static void ParseWebProxyAddressSettings(ProxyAndRecordSettings settings, SimpleSettingsParser parser)
    {
        string? proxyAddress = parser.GetStringValue("WebProxyAddress");
        if (!string.IsNullOrEmpty(proxyAddress))
        {
            settings.WebProxySettings = new WebProxySettings
            {
                Address = proxyAddress!,
                UserName = parser.GetStringValue("WebProxyUserName"),
                Password = parser.GetStringValue("WebProxyPassword")
            };
        }
    }

    private static void ParseProxyUrlReplaceSettings(ProxyAndRecordSettings settings, SimpleSettingsParser parser)
    {
        const string prefix = "ProxyUrlReplace";
        var proxyUrlReplaceOldValue = parser.GetStringValue($"{prefix}OldValue");
        var proxyUrlReplaceNewValue = parser.GetStringValue($"{prefix}NewValue");
        if (!string.IsNullOrEmpty(proxyUrlReplaceOldValue) && proxyUrlReplaceNewValue != null)
        {
            settings.ReplaceSettings = new ProxyUrlReplaceSettings
            {
                OldValue = proxyUrlReplaceOldValue,
                NewValue = proxyUrlReplaceNewValue,
                IgnoreCase = parser.GetBoolValue($"{prefix}IgnoreCase")
            };
            return;
        }

        var transformTemplate = parser.GetStringValue($"{prefix}TransformTemplate");
        if (!string.IsNullOrEmpty(transformTemplate))
        {
            settings.ReplaceSettings = new ProxyUrlReplaceSettings
            {
                TransformTemplate = transformTemplate,
                TransformerType = parser.GetEnumValue($"{prefix}TransformerType", TransformerType.Handlebars)
            };
        }
    }

    private static void ParseActivityTracingSettings(WireMockServerSettings settings, SimpleSettingsParser parser)
    {
        // Only create ActivityTracingOptions if tracing is enabled
        if (parser.GetBoolValue("ActivityTracingEnabled") || parser.GetBoolValue("ActivityTracingOptions__Enabled"))
        {
            settings.ActivityTracingOptions = new ActivityTracingOptions
            {
                ExcludeAdminRequests = GetBoolWithDefault(parser, "ActivityTracingExcludeAdminRequests", "ActivityTracingOptions__ExcludeAdminRequests", defaultValue: true),
                RecordRequestBody = parser.GetBoolValue("ActivityTracingRecordRequestBody") || parser.GetBoolValue("ActivityTracingOptions__RecordRequestBody"),
                RecordResponseBody = parser.GetBoolValue("ActivityTracingRecordResponseBody") || parser.GetBoolValue("ActivityTracingOptions__RecordResponseBody"),
                RecordMatchDetails = GetBoolWithDefault(parser, "ActivityTracingRecordMatchDetails", "ActivityTracingOptions__RecordMatchDetails", defaultValue: true)
            };
        }
    }

    private static bool GetBoolWithDefault(SimpleSettingsParser parser, string key1, string key2, bool defaultValue)
    {
        if (parser.Contains(key1))
        {
            return parser.GetBoolValue(key1);
        }

        if (parser.Contains(key2))
        {
            return parser.GetBoolValue(key2);
        }

        return defaultValue;
    }
}