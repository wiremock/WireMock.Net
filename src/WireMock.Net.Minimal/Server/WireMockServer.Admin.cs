// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stef.Validation;
using WireMock.Admin.Mappings;
using WireMock.Admin.Scenarios;
using WireMock.Admin.Settings;
using WireMock.Constants;
using WireMock.Http;
using WireMock.Logging;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.Owin;
using WireMock.RequestBuilders;
using WireMock.ResponseProviders;
using WireMock.Serialization;
using WireMock.Settings;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.Server;

/// <summary>
/// The fluent mock server.
/// </summary>
public partial class WireMockServer
{
    private const int EnhancedFileSystemWatcherTimeoutMs = 1000;
    private const string DefaultAdminPathPrefix = "/__admin";
    private const string QueryParamReloadStaticMappings = "reloadStaticMappings";
    private static readonly Guid ProxyMappingGuid = new("e59914fd-782e-428e-91c1-4810ffb86567");
    private static readonly RegexMatcher AdminRequestContentTypeJson = new ContentTypeMatcher(WireMockConstants.ContentTypeJson, true);
    private EnhancedFileSystemWatcher? _enhancedFileSystemWatcher;
    private AdminPaths? _adminPaths;

    private sealed class AdminPaths
    {
        private readonly string _prefix;
        private readonly string _prefixEscaped;

        public AdminPaths(WireMockServerSettings settings)
        {
            _prefix = settings.AdminPath ?? DefaultAdminPathPrefix;
            _prefixEscaped = _prefix.Replace("/", "\\/");
        }

        public string Files => $"{_prefix}/files";
        public string Health => $"{_prefix}/health";
        public string Mappings => $"{_prefix}/mappings";
        public string MappingsCode => $"{_prefix}/mappings/code";
        public string MappingsWireMockOrg => $"{_prefix}mappings/wiremock.org";
        public string Requests => $"{_prefix}/requests";
        public string Settings => $"{_prefix}/settings";
        public string Scenarios => $"{_prefix}/scenarios";
        public string OpenApi => $"{_prefix}/openapi";

        public RegexMatcher MappingsGuidPathMatcher => new($"^{_prefixEscaped}\\/mappings\\/([0-9A-Fa-f]{{8}}[-][0-9A-Fa-f]{{4}}[-][0-9A-Fa-f]{{4}}[-][0-9A-Fa-f]{{4}}[-][0-9A-Fa-f]{{12}})$");
        public RegexMatcher MappingsCodeGuidPathMatcher => new($"^{_prefixEscaped}\\/mappings\\/code\\/([0-9A-Fa-f]{{8}}[-][0-9A-Fa-f]{{4}}[-][0-9A-Fa-f]{{4}}[-][0-9A-Fa-f]{{4}}[-][0-9A-Fa-f]{{12}})$");
        public RegexMatcher RequestsGuidPathMatcher => new($"^{_prefixEscaped}\\/requests\\/([0-9A-Fa-f]{{8}}[-][0-9A-Fa-f]{{4}}[-][0-9A-Fa-f]{{4}}[-][0-9A-Fa-f]{{4}}[-][0-9A-Fa-f]{{12}})$");
        public RegexMatcher ScenariosNameMatcher => new($"^{_prefixEscaped}\\/scenarios\\/.+$");
        public RegexMatcher ScenariosNameWithStateMatcher => new($"^{_prefixEscaped}\\/scenarios\\/.+\\/state$");
        public RegexMatcher ScenariosNameWithResetMatcher => new($"^{_prefixEscaped}\\/scenarios\\/.+\\/reset$");
        public RegexMatcher FilesFilenamePathMatcher => new($"^{_prefixEscaped}\\/files\\/.+$");
        public RegexMatcher ProtoDefinitionsIdPathMatcher => new($"^{_prefixEscaped}\\/protodefinitions\\/.+$");
    }

    #region InitAdmin
    private void InitAdmin()
    {
        _adminPaths = new AdminPaths(_settings);

        // __admin/health
        Given(Request.Create().WithPath(_adminPaths.Health).UsingGet()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(HealthGet));

        // __admin/settings
        Given(Request.Create().WithPath(_adminPaths.Settings).UsingGet()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(SettingsGet));
        Given(Request.Create().WithPath(_adminPaths.Settings).UsingMethod("PUT", "POST").WithHeader(HttpKnownHeaderNames.ContentType, AdminRequestContentTypeJson)).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(SettingsUpdate));

        // __admin/mappings
        Given(Request.Create().WithPath(_adminPaths.Mappings).UsingGet()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(MappingsGet));
        Given(Request.Create().WithPath(_adminPaths.Mappings).UsingPost().WithHeader(HttpKnownHeaderNames.ContentType, AdminRequestContentTypeJson)).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(MappingsPost));
        Given(Request.Create().WithPath(_adminPaths.Mappings).UsingDelete()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(MappingsDelete));

        // __admin/mappings/code
        Given(Request.Create().WithPath(_adminPaths.MappingsCode).UsingGet()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(MappingsCodeGet));

        // __admin/mappings/wiremock.org
        Given(Request.Create().WithPath(_adminPaths.MappingsWireMockOrg).UsingPost().WithHeader(HttpKnownHeaderNames.ContentType, AdminRequestContentTypeJson)).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(MappingsPostWireMockOrg));

        // __admin/mappings/reset
        Given(Request.Create().WithPath(_adminPaths.Mappings + "/reset").UsingPost()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(MappingsReset));

        // __admin/mappings/reloadStaticMappings
        Given(Request.Create().WithPath(_adminPaths.Mappings + "/reloadStaticMappings").UsingPost()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(ReloadStaticMappings));

        // __admin/mappings/{guid}
        Given(Request.Create().WithPath(_adminPaths.MappingsGuidPathMatcher).UsingGet()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(MappingGet));
        Given(Request.Create().WithPath(_adminPaths.MappingsGuidPathMatcher).UsingPut().WithHeader(HttpKnownHeaderNames.ContentType, AdminRequestContentTypeJson)).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(MappingPut));
        Given(Request.Create().WithPath(_adminPaths.MappingsGuidPathMatcher).UsingDelete()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(MappingDelete));

        // __admin/mappings/code/{guid}
        Given(Request.Create().WithPath(_adminPaths.MappingsCodeGuidPathMatcher).UsingGet()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(MappingCodeGet));

        // __admin/mappings/save
        Given(Request.Create().WithPath($"{_adminPaths.Mappings}/save").UsingPost()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(MappingsSave));

        // __admin/mappings/swagger
        Given(Request.Create().WithPath($"{_adminPaths.Mappings}/swagger").UsingGet()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(SwaggerGet));

        // __admin/requests
        Given(Request.Create().WithPath(_adminPaths.Requests).UsingGet()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(RequestsGet));
        Given(Request.Create().WithPath(_adminPaths.Requests).UsingDelete()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(RequestsDelete));

        // __admin/requests/reset
        Given(Request.Create().WithPath(_adminPaths.Requests + "/reset").UsingPost()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(RequestsDelete));

        // __admin/request/{guid}
        Given(Request.Create().WithPath(_adminPaths.RequestsGuidPathMatcher).UsingGet()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(RequestGet));
        Given(Request.Create().WithPath(_adminPaths.RequestsGuidPathMatcher).UsingDelete()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(RequestDelete));

        // __admin/requests/find
        Given(Request.Create().WithPath(_adminPaths.Requests + "/find").UsingPost()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(RequestsFind));
        Given(Request.Create().WithPath(_adminPaths.Requests + "/find").UsingGet().WithParam("mappingGuid", new NotNullOrEmptyMatcher())).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(RequestsFindByMappingGuid));

        // __admin/scenarios
        Given(Request.Create().WithPath(_adminPaths.Scenarios).UsingGet()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(ScenariosGet));
        Given(Request.Create().WithPath(_adminPaths.Scenarios).UsingDelete()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(ScenariosReset));
        Given(Request.Create().WithPath(_adminPaths.ScenariosNameMatcher).UsingDelete()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(ScenarioReset));

        // __admin/scenarios/reset
        Given(Request.Create().WithPath(_adminPaths.Scenarios + "/reset").UsingPost()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(ScenariosReset));
        Given(Request.Create().WithPath(_adminPaths.ScenariosNameWithResetMatcher).UsingPost()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(ScenarioReset));

        // __admin/scenarios/{scenario}/state
        Given(Request.Create().WithPath(_adminPaths.ScenariosNameWithStateMatcher).UsingPut()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(ScenariosSetState));

        // __admin/files/{filename}
        Given(Request.Create().WithPath(_adminPaths.FilesFilenamePathMatcher).UsingPost()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(FilePost));
        Given(Request.Create().WithPath(_adminPaths.FilesFilenamePathMatcher).UsingPut()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(FilePut));
        Given(Request.Create().WithPath(_adminPaths.FilesFilenamePathMatcher).UsingGet()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(FileGet));
        Given(Request.Create().WithPath(_adminPaths.FilesFilenamePathMatcher).UsingHead()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(FileHead));
        Given(Request.Create().WithPath(_adminPaths.FilesFilenamePathMatcher).UsingDelete()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(FileDelete));

        // __admin/openapi
        Given(Request.Create().WithPath($"{_adminPaths.OpenApi}/convert").UsingPost()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(OpenApiConvertToMappings));
        Given(Request.Create().WithPath($"{_adminPaths.OpenApi}/save").UsingPost()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(OpenApiSaveToMappings));

        // __admin/protodefinitions/{id}
        Given(Request.Create().WithPath(_adminPaths.ProtoDefinitionsIdPathMatcher).UsingPost()).AtPriority(WireMockConstants.AdminPriority).RespondWith(new DynamicResponseProvider(ProtoDefinitionAdd));
    }
    #endregion

    #region StaticMappings
    /// <inheritdoc cref="IWireMockServer.SaveStaticMappings" />
    [PublicAPI]
    public void SaveStaticMappings(string? folder = null)
    {
        _mappingBuilder.SaveMappingsToFolder(folder);
    }

    /// <inheritdoc cref="IWireMockServer.ReadStaticMappings" />
    [PublicAPI]
    public void ReadStaticMappings(string? folder = null)
    {
        if (folder == null)
        {
            folder = _settings.FileSystemHandler.GetMappingFolder();
        }

        if (!_settings.FileSystemHandler.FolderExists(folder))
        {
            _settings.Logger.Info("The Static Mapping folder '{0}' does not exist, reading Static MappingFiles will be skipped.", folder);
            return;
        }

        foreach (var filename in _settings.FileSystemHandler.EnumerateFiles(folder, _settings.WatchStaticMappingsInSubdirectories == true).OrderBy(f => f))
        {
            _settings.Logger.Info("Reading Static MappingFile : '{0}'.", filename);

            try
            {
                ReadStaticMappingAndAddOrUpdate(filename);
            }
            catch (Exception exception)
            {
                _settings.Logger.Error($"Static MappingFile : '{filename}' could not be read. This file will be skipped.", exception);
            }
        }
    }

    /// <inheritdoc cref="IWireMockServer.WatchStaticMappings" />
    [PublicAPI]
    public void WatchStaticMappings(string? folder = null)
    {
        if (folder == null)
        {
            folder = _settings.FileSystemHandler.GetMappingFolder();
        }

        if (!_settings.FileSystemHandler.FolderExists(folder))
        {
            return;
        }

        bool includeSubdirectories = _settings.WatchStaticMappingsInSubdirectories == true;
        string includeSubdirectoriesText = includeSubdirectories ? " and Subdirectories" : string.Empty;

        _settings.Logger.Info($"Watching folder '{folder}'{includeSubdirectoriesText} for new, updated and deleted MappingFiles.");

        DisposeEnhancedFileSystemWatcher();
        _enhancedFileSystemWatcher = new EnhancedFileSystemWatcher(folder, "*.json", EnhancedFileSystemWatcherTimeoutMs)
        {
            IncludeSubdirectories = includeSubdirectories
        };
        _enhancedFileSystemWatcher.Created += EnhancedFileSystemWatcherCreated;
        _enhancedFileSystemWatcher.Changed += EnhancedFileSystemWatcherChanged;
        _enhancedFileSystemWatcher.Deleted += EnhancedFileSystemWatcherDeleted;
        _enhancedFileSystemWatcher.EnableRaisingEvents = true;
    }

    /// <inheritdoc cref="IWireMockServer.WatchStaticMappings" />
    [PublicAPI]
    public bool ReadStaticMappingAndAddOrUpdate(string path)
    {
        Guard.NotNull(path);

        string filenameWithoutExtension = Path.GetFileNameWithoutExtension(path);

        if (FileHelper.TryReadMappingFileWithRetryAndDelay(_settings.FileSystemHandler, path, out var value))
        {
            var mappingModels = DeserializeJsonToArray<MappingModel>(value);
            if (mappingModels.Length == 1 && Guid.TryParse(filenameWithoutExtension, out var guidFromFilename))
            {
                ConvertMappingAndRegisterAsRespondProvider(mappingModels[0], guidFromFilename, path);
            }
            else
            {
                ConvertMappingsAndRegisterAsRespondProvider(mappingModels, path);
            }

            return true;
        }

        return false;
    }
    #endregion

    #region Health
    private static IResponseMessage HealthGet(HttpContext _, IRequestMessage requestMessage)
    {
        return new ResponseMessage
        {
            BodyData = new BodyData
            {
                DetectedBodyType = BodyType.String,
                BodyAsString = "Healthy"
            },
            StatusCode = (int)HttpStatusCode.OK,
            Headers = new Dictionary<string, WireMockList<string>> { { HttpKnownHeaderNames.ContentType, new WireMockList<string>(WireMockConstants.ContentTypeTextPlain) } }
        };
    }
    #endregion

    #region Settings
    private IResponseMessage SettingsGet(HttpContext _, IRequestMessage requestMessage)
    {
        var model = new SettingsModel
        {
            AllowBodyForAllHttpMethods = _settings.AllowBodyForAllHttpMethods,
            AllowOnlyDefinedHttpStatusCodeInResponse = _settings.AllowOnlyDefinedHttpStatusCodeInResponse,
            AllowPartialMapping = _settings.AllowPartialMapping,
            DisableDeserializeFormUrlEncoded = _settings.DisableDeserializeFormUrlEncoded,
            DisableJsonBodyParsing = _settings.DisableJsonBodyParsing,
            DisableRequestBodyDecompressing = _settings.DisableRequestBodyDecompressing,
            DoNotSaveDynamicResponseInLogEntry = _settings.DoNotSaveDynamicResponseInLogEntry,
            GlobalProcessingDelay = (int?)_options.RequestProcessingDelay?.TotalMilliseconds,
            // GraphQLSchemas TODO
            HandleRequestsSynchronously = _settings.HandleRequestsSynchronously,
            HostingScheme = _settings.HostingScheme,
            MaxRequestLogCount = _settings.MaxRequestLogCount,
            ProtoDefinitions = _settings.ProtoDefinitions,
            QueryParameterMultipleValueSupport = _settings.QueryParameterMultipleValueSupport,
            ReadStaticMappings = _settings.ReadStaticMappings,
            RequestLogExpirationDuration = _settings.RequestLogExpirationDuration,
            SaveUnmatchedRequests = _settings.SaveUnmatchedRequests,
            UseRegexExtended = _settings.UseRegexExtended,
            WatchStaticMappings = _settings.WatchStaticMappings,
            WatchStaticMappingsInSubdirectories = _settings.WatchStaticMappingsInSubdirectories,

            AcceptAnyClientCertificate = _settings.AcceptAnyClientCertificate,
            ClientCertificateMode = _settings.ClientCertificateMode,
            CorsPolicyOptions = _settings.CorsPolicyOptions?.ToString()
        };

        model.ProxyAndRecordSettings = TinyMapperUtils.Instance.Map(_settings.ProxyAndRecordSettings);

        return ToJson(model);
    }

    private IResponseMessage SettingsUpdate(HttpContext _, IRequestMessage requestMessage)
    {
        var settings = DeserializeObject<SettingsModel>(requestMessage);

        // _settings
        _settings.AllowBodyForAllHttpMethods = settings.AllowBodyForAllHttpMethods;
        _settings.AllowOnlyDefinedHttpStatusCodeInResponse = settings.AllowOnlyDefinedHttpStatusCodeInResponse;
        _settings.AllowPartialMapping = settings.AllowPartialMapping;
        _settings.DisableDeserializeFormUrlEncoded = settings.DisableDeserializeFormUrlEncoded;
        _settings.DisableJsonBodyParsing = settings.DisableJsonBodyParsing;
        _settings.DisableRequestBodyDecompressing = settings.DisableRequestBodyDecompressing;
        _settings.DoNotSaveDynamicResponseInLogEntry = settings.DoNotSaveDynamicResponseInLogEntry;
        _settings.HandleRequestsSynchronously = settings.HandleRequestsSynchronously;
        _settings.MaxRequestLogCount = settings.MaxRequestLogCount;
        _settings.ProtoDefinitions = settings.ProtoDefinitions;
        _settings.ProxyAndRecordSettings = TinyMapperUtils.Instance.Map(settings.ProxyAndRecordSettings);
        _settings.QueryParameterMultipleValueSupport = settings.QueryParameterMultipleValueSupport;
        _settings.ReadStaticMappings = settings.ReadStaticMappings;
        _settings.RequestLogExpirationDuration = settings.RequestLogExpirationDuration;
        _settings.SaveUnmatchedRequests = settings.SaveUnmatchedRequests;
        _settings.UseRegexExtended = settings.UseRegexExtended;
        _settings.WatchStaticMappings = settings.WatchStaticMappings;
        _settings.WatchStaticMappingsInSubdirectories = settings.WatchStaticMappingsInSubdirectories;

        InitSettings(_settings);

//#if USE_ASPNETCORE
        if (Enum.TryParse<CorsPolicyOptions>(settings.CorsPolicyOptions, true, out var corsPolicyOptions))
        {
            _settings.CorsPolicyOptions = corsPolicyOptions;
        }
//#endif

        WireMockMiddlewareOptionsHelper.InitFromSettings(_settings, _options, o =>
        {
            if (settings.GlobalProcessingDelay != null)
            {
                o.RequestProcessingDelay = TimeSpan.FromMilliseconds(settings.GlobalProcessingDelay.Value);
            }

//#if USE_ASPNETCORE
            o.CorsPolicyOptions = corsPolicyOptions;
            o.ClientCertificateMode = (Microsoft.AspNetCore.Server.Kestrel.Https.ClientCertificateMode) _settings.ClientCertificateMode;
            o.AcceptAnyClientCertificate = _settings.AcceptAnyClientCertificate;
//#endif
        });

        return ResponseMessageBuilder.Create(200, "Settings updated");
    }
    #endregion Settings

    #region Mapping/{guid}
    private IResponseMessage MappingGet(HttpContext context, IRequestMessage requestMessage)
    {
        var mapping = FindMappingByGuid(context, requestMessage);
        if (mapping == null)
        {
            _settings.Logger.Warn("HttpStatusCode set to 404 : Mapping not found");
            return ResponseMessageBuilder.Create(HttpStatusCode.NotFound, "Mapping not found");
        }

        var model = _mappingConverter.ToMappingModel(mapping);

        return ToJson(model);
    }

    private IResponseMessage MappingCodeGet(HttpContext context, IRequestMessage requestMessage)
    {
        if (TryParseGuidFromRequestMessage(requestMessage, out var guid))
        {
            var code = _mappingBuilder.ToCSharpCode(guid, GetEnumFromQuery(requestMessage, MappingConverterType.Server));
            if (code is null)
            {
                _settings.Logger.Warn("HttpStatusCode set to 404 : Mapping not found");
                return ResponseMessageBuilder.Create(HttpStatusCode.NotFound, "Mapping not found");
            }

            return ToResponseMessage(code);
        }

        _settings.Logger.Warn("HttpStatusCode set to 400");
        return ResponseMessageBuilder.Create(HttpStatusCode.BadRequest, "GUID is missing");
    }

    private static TEnum GetEnumFromQuery<TEnum>(IRequestMessage requestMessage, TEnum defaultValue)
        where TEnum : struct
    {
        if (requestMessage.QueryIgnoreCase?.TryGetValue(typeof(TEnum).Name, out var values) == true &&
            Enum.TryParse<TEnum>(values.FirstOrDefault(), true, out var parsed))
        {
            return parsed;
        }

        return defaultValue;
    }

    private IMapping? FindMappingByGuid(HttpContext _, IRequestMessage requestMessage)
    {
        return TryParseGuidFromRequestMessage(requestMessage, out var guid) ? Mappings.FirstOrDefault(m => !m.IsAdminInterface && m.Guid == guid) : null;
    }

    private IResponseMessage MappingPut(HttpContext _, IRequestMessage requestMessage)
    {
        if (TryParseGuidFromRequestMessage(requestMessage, out var guid))
        {
            var mappingModel = DeserializeObject<MappingModel>(requestMessage);
            var guidFromPut = ConvertMappingAndRegisterAsRespondProvider(mappingModel, guid);

            return ResponseMessageBuilder.Create(HttpStatusCode.OK, "Mapping added or updated", guidFromPut);
        }

        _settings.Logger.Warn("HttpStatusCode set to 404 : Mapping not found");
        return ResponseMessageBuilder.Create(HttpStatusCode.NotFound, "Mapping not found");
    }

    private IResponseMessage MappingDelete(HttpContext _, IRequestMessage requestMessage)
    {
        if (TryParseGuidFromRequestMessage(requestMessage, out var guid) && DeleteMapping(guid))
        {
            return ResponseMessageBuilder.Create(HttpStatusCode.OK, "Mapping removed", guid);
        }

        _settings.Logger.Warn("HttpStatusCode set to 404 : Mapping not found");
        return ResponseMessageBuilder.Create(HttpStatusCode.NotFound, "Mapping not found");
    }

    private static bool TryParseGuidFromRequestMessage(IRequestMessage requestMessage, out Guid guid)
    {
        var lastPart = requestMessage.Path.Split('/').LastOrDefault();
        return Guid.TryParse(lastPart, out guid);
    }
    #endregion Mapping/{guid}

    #region Mappings
    private IResponseMessage SwaggerGet(HttpContext _, IRequestMessage requestMessage)
    {
        return new ResponseMessage
        {
            BodyData = new BodyData
            {
                DetectedBodyType = BodyType.String,
                BodyAsString = SwaggerMapper.ToSwagger(this)
            },
            StatusCode = (int)HttpStatusCode.OK,
            Headers = new Dictionary<string, WireMockList<string>> { { HttpKnownHeaderNames.ContentType, new WireMockList<string>(WireMockConstants.ContentTypeJson) } }
        };
    }

    private IResponseMessage MappingsSave(HttpContext _, IRequestMessage requestMessage)
    {
        SaveStaticMappings();

        return ResponseMessageBuilder.Create(200, "Mappings saved to disk");
    }

    private MappingModel[] ToMappingModels()
    {
        return _mappingBuilder.GetMappings();
    }

    private IResponseMessage MappingsGet(HttpContext _, IRequestMessage requestMessage)
    {
        return ToJson(ToMappingModels());
    }

    private IResponseMessage MappingsCodeGet(HttpContext _, IRequestMessage requestMessage)
    {
        var converterType = GetEnumFromQuery(requestMessage, MappingConverterType.Server);

        var code = _mappingBuilder.ToCSharpCode(converterType);

        return ToResponseMessage(code);
    }

    private IResponseMessage MappingsPost(HttpContext _, IRequestMessage requestMessage)
    {
        try
        {
            var mappingModels = DeserializeRequestMessageToArray<MappingModel>(requestMessage);
            if (mappingModels.Length == 1)
            {
                var guid = ConvertMappingAndRegisterAsRespondProvider(mappingModels[0]);
                return ResponseMessageBuilder.Create(201, "Mapping added", guid);
            }

            ConvertMappingsAndRegisterAsRespondProvider(mappingModels);

            return ResponseMessageBuilder.Create(201, "Mappings added");
        }
        catch (ArgumentException a)
        {
            _settings.Logger.Error("HttpStatusCode set to 400 {0}", a);
            return ResponseMessageBuilder.Create(400, a.Message);
        }
        catch (Exception e)
        {
            _settings.Logger.Error("HttpStatusCode set to 500 {0}", e);
            return ResponseMessageBuilder.Create(500, e.ToString());
        }
    }

    private IResponseMessage MappingsDelete(HttpContext _, IRequestMessage requestMessage)
    {
        if (!string.IsNullOrEmpty(requestMessage.Body))
        {
            var deletedGuids = MappingsDeleteMappingFromBody(requestMessage);
            if (deletedGuids != null)
            {
                return ResponseMessageBuilder.Create(200, $"Mappings deleted. Affected GUIDs: [{string.Join(", ", deletedGuids.ToArray())}]");
            }

            // return bad request
            return ResponseMessageBuilder.Create(400, "Poorly formed mapping JSON.");
        }

        ResetMappings();

        ResetScenarios();

        return ResponseMessageBuilder.Create(200, "Mappings deleted");
    }

    private IEnumerable<Guid>? MappingsDeleteMappingFromBody(IRequestMessage requestMessage)
    {
        var deletedGuids = new List<Guid>();

        try
        {
            var mappingModels = DeserializeRequestMessageToArray<MappingModel>(requestMessage);
            foreach (var guid in mappingModels.Where(mm => mm.Guid.HasValue).Select(mm => mm.Guid!.Value))
            {
                if (DeleteMapping(guid))
                {
                    deletedGuids.Add(guid);
                }
                else
                {
                    _settings.Logger.Debug($"Did not find/delete mapping with GUID: {guid}.");
                }
            }
        }
        catch (ArgumentException a)
        {
            _settings.Logger.Error("ArgumentException: {0}", a);
            return null;
        }
        catch (Exception e)
        {
            _settings.Logger.Error("Exception: {0}", e);
            return null;
        }

        return deletedGuids;
    }

    private IResponseMessage MappingsReset(HttpContext _, IRequestMessage requestMessage)
    {
        ResetMappings();

        ResetScenarios();

        var message = "Mappings reset";
        if (requestMessage.Query != null &&
            requestMessage.Query.ContainsKey(QueryParamReloadStaticMappings) &&
            bool.TryParse(requestMessage.Query[QueryParamReloadStaticMappings].ToString(), out var reloadStaticMappings) &&
            reloadStaticMappings)
        {
            ReadStaticMappings();
            message += " and static mappings reloaded";
        }

        return ResponseMessageBuilder.Create(200, message);
    }

    private IResponseMessage ReloadStaticMappings(HttpContext _, IRequestMessage __)
    {
        ReadStaticMappings();

        return ResponseMessageBuilder.Create(200, "Static Mappings reloaded");
    }
    #endregion Mappings

    #region Request/{guid}
    private IResponseMessage RequestGet(HttpContext _, IRequestMessage requestMessage)
    {
        if (TryParseGuidFromRequestMessage(requestMessage, out var guid))
        {
            var entry = LogEntries.SingleOrDefault(r => !r.RequestMessage.Path.StartsWith("/__admin/") && r.Guid == guid);
            if (entry is { })
            {
                var model = new LogEntryMapper(_options).Map(entry);
                return ToJson(model);
            }
        }

        _settings.Logger.Warn("HttpStatusCode set to 404 : Request not found");
        return ResponseMessageBuilder.Create(HttpStatusCode.NotFound, "Request not found");
    }

    private IResponseMessage RequestDelete(HttpContext _, IRequestMessage requestMessage)
    {
        if (TryParseGuidFromRequestMessage(requestMessage, out var guid) && DeleteLogEntry(guid))
        {
            return ResponseMessageBuilder.Create(200, "Request removed");
        }

        _settings.Logger.Warn("HttpStatusCode set to 404 : Request not found");
        return ResponseMessageBuilder.Create(HttpStatusCode.NotFound, "Request not found");
    }
    #endregion Request/{guid}

    #region Requests
    private IResponseMessage RequestsGet(HttpContext _, IRequestMessage requestMessage)
    {
        var logEntryMapper = new LogEntryMapper(_options);
        var result = LogEntries
            .Where(r => !r.RequestMessage.Path.StartsWith("/__admin/"))
            .Select(logEntryMapper.Map);

        return ToJson(result);
    }

    private IResponseMessage RequestsDelete(HttpContext _, IRequestMessage requestMessage)
    {
        ResetLogEntries();

        return ResponseMessageBuilder.Create(200, "Requests deleted");
    }
    #endregion Requests

    #region Requests/find
    private IResponseMessage RequestsFind(HttpContext _, IRequestMessage requestMessage)
    {
        var requestModel = DeserializeObject<RequestModel>(requestMessage);

        var request = (Request)InitRequestBuilder(requestModel);

        var dict = new Dictionary<ILogEntry, RequestMatchResult>();
        foreach (var logEntry in LogEntries.Where(le => !le.RequestMessage.Path.StartsWith("/__admin/")))
        {
            var requestMatchResult = new RequestMatchResult();
            if (request.GetMatchingScore(logEntry.RequestMessage, requestMatchResult) > MatchScores.AlmostPerfect)
            {
                dict.Add(logEntry, requestMatchResult);
            }
        }

        var logEntryMapper = new LogEntryMapper(_options);
        var result = dict.OrderBy(x => x.Value.AverageTotalScore).Select(x => x.Key).Select(logEntryMapper.Map);

        return ToJson(result);
    }

    private IResponseMessage RequestsFindByMappingGuid(HttpContext _, IRequestMessage requestMessage)
    {
        if (requestMessage.Query != null &&
            requestMessage.Query.TryGetValue("mappingGuid", out var value) &&
            Guid.TryParse(value.ToString(), out var mappingGuid)
        )
        {
            var logEntries = LogEntries.Where(le => !le.RequestMessage.Path.StartsWith("/__admin/") && le.MappingGuid == mappingGuid);
            var logEntryMapper = new LogEntryMapper(_options);
            var result = logEntries.Select(logEntryMapper.Map);
            return ToJson(result);
        }

        return ResponseMessageBuilder.Create(HttpStatusCode.BadRequest);
    }
    #endregion Requests/find

    #region Scenarios
    private IResponseMessage ScenariosGet(HttpContext _, IRequestMessage requestMessage)
    {
        var scenariosStates = Scenarios.Values.Select(s => new ScenarioStateModel
        {
            Name = s.Name,
            NextState = s.NextState,
            Started = s.Started,
            Finished = s.Finished,
            Counter = s.Counter
        });

        return ToJson(scenariosStates, true);
    }

    private IResponseMessage ScenariosReset(HttpContext _, IRequestMessage requestMessage)
    {
        ResetScenarios();

        return ResponseMessageBuilder.Create(200, "Scenarios reset");
    }

    private IResponseMessage ScenarioReset(HttpContext _, IRequestMessage requestMessage)
    {
        var name = string.Equals(HttpRequestMethod.DELETE, requestMessage.Method, StringComparison.OrdinalIgnoreCase) ?
            requestMessage.Path.Substring(_adminPaths!.Scenarios.Length + 1) :
            requestMessage.Path.Split('/').ReverseEx().Skip(1).First();

        return ResetScenario(name) ?
            ResponseMessageBuilder.Create(200, "Scenario reset") :
            ResponseMessageBuilder.Create(HttpStatusCode.NotFound, $"No scenario found by name '{name}'.");
    }

    private IResponseMessage ScenariosSetState(HttpContext _, IRequestMessage requestMessage)
    {
        var name = requestMessage.Path.Split('/').ReverseEx().Skip(1).First();
        if (!_options.Scenarios.ContainsKey(name))
        {
            ResponseMessageBuilder.Create(HttpStatusCode.NotFound, $"No scenario found by name '{name}'.");
        }

        var update = DeserializeObject<ScenarioStateUpdateModel>(requestMessage);

        return SetScenarioState(name, update.State) ?
            ResponseMessageBuilder.Create(200, $"Scenario state set to '{update.State}'") :
            ResponseMessageBuilder.Create(HttpStatusCode.NotFound, $"No scenario found by name '{name}'.");
    }
    #endregion

    #region Pact
    /// <summary>
    /// Save the mappings as a Pact Json file V2.
    /// </summary>
    /// <param name="folder">The folder to save the pact file.</param>
    /// <param name="filename">The filename for the .json file [optional].</param>
    [PublicAPI]
    public void SavePact(string folder, string? filename = null)
    {
        var (filenameUpdated, bytes) = PactMapper.ToPact(this, filename);
        _settings.FileSystemHandler.WriteFile(folder, filenameUpdated, bytes);
    }

    /// <summary>
    /// Save the mappings as a Pact Json file V2.
    /// </summary>
    /// <param name="stream">The (file) stream.</param>
    [PublicAPI]
    public void SavePact(Stream stream)
    {
        var (_, bytes) = PactMapper.ToPact(this);
        using var writer = new BinaryWriter(stream);
        writer.Write(bytes);

        if (stream.CanSeek)
        {
            stream.Seek(0, SeekOrigin.Begin);
        }
    }

    /// <summary>
    /// This stores details about the consumer of the interaction.
    /// </summary>
    /// <param name="consumer">the consumer</param>
    [PublicAPI]
    public WireMockServer WithConsumer(string consumer)
    {
        Consumer = consumer;
        return this;
    }

    /// <summary>
    /// This stores details about the provider of the interaction.
    /// </summary>
    /// <param name="provider">the provider</param>
    [PublicAPI]
    public WireMockServer WithProvider(string provider)
    {
        Provider = provider;
        return this;
    }
    #endregion

    private void DisposeEnhancedFileSystemWatcher()
    {
        if (_enhancedFileSystemWatcher != null)
        {
            _enhancedFileSystemWatcher.EnableRaisingEvents = false;

            _enhancedFileSystemWatcher.Created -= EnhancedFileSystemWatcherCreated;
            _enhancedFileSystemWatcher.Changed -= EnhancedFileSystemWatcherChanged;
            _enhancedFileSystemWatcher.Deleted -= EnhancedFileSystemWatcherDeleted;

            _enhancedFileSystemWatcher.Dispose();
        }
    }

    private void EnhancedFileSystemWatcherCreated(object sender, FileSystemEventArgs args)
    {
        _settings.Logger.Info("MappingFile created : '{0}', reading file.", args.FullPath);
        if (!ReadStaticMappingAndAddOrUpdate(args.FullPath))
        {
            _settings.Logger.Error("Unable to read MappingFile '{0}'.", args.FullPath);
        }
    }

    private void EnhancedFileSystemWatcherChanged(object sender, FileSystemEventArgs args)
    {
        _settings.Logger.Info("MappingFile updated : '{0}', reading file.", args.FullPath);
        if (!ReadStaticMappingAndAddOrUpdate(args.FullPath))
        {
            _settings.Logger.Error("Unable to read MappingFile '{0}'.", args.FullPath);
        }
    }

    private void EnhancedFileSystemWatcherDeleted(object sender, FileSystemEventArgs args)
    {
        _settings.Logger.Info("MappingFile deleted : '{0}'", args.FullPath);
        var filenameWithoutExtension = Path.GetFileNameWithoutExtension(args.FullPath);

        if (Guid.TryParse(filenameWithoutExtension, out var guidFromFilename))
        {
            DeleteMapping(guidFromFilename);
        }
        else
        {
            DeleteMapping(args.FullPath);
        }
    }

    private static Encoding? ToEncoding(EncodingModel? encodingModel)
    {
        return encodingModel != null ? Encoding.GetEncoding(encodingModel.CodePage) : null;
    }

    private static ResponseMessage ToJson<T>(T result, bool keepNullValues = false, object? statusCode = null)
    {
        return new ResponseMessage
        {
            BodyData = new BodyData
            {
                DetectedBodyType = BodyType.String,
                BodyAsString = JsonConvert.SerializeObject(result, keepNullValues ? JsonSerializationConstants.JsonSerializerSettingsIncludeNullValues : JsonSerializationConstants.JsonSerializerSettingsDefault)
            },
            StatusCode = statusCode ?? (int)HttpStatusCode.OK,
            Headers = new Dictionary<string, WireMockList<string>> { { HttpKnownHeaderNames.ContentType, new WireMockList<string>(WireMockConstants.ContentTypeJson) } }
        };
    }

    private static ResponseMessage ToResponseMessage(string text)
    {
        return new ResponseMessage
        {
            BodyData = new BodyData
            {
                DetectedBodyType = BodyType.String,
                BodyAsString = text
            },
            StatusCode = (int)HttpStatusCode.OK,
            Headers = new Dictionary<string, WireMockList<string>> { { HttpKnownHeaderNames.ContentType, new WireMockList<string>(WireMockConstants.ContentTypeTextPlain) } }
        };
    }

    private static T DeserializeObject<T>(IRequestMessage requestMessage) where T : new()
    {
        switch (requestMessage.BodyData?.DetectedBodyType)
        {
            case BodyType.String:
            case BodyType.FormUrlEncoded:
                return JsonUtils.DeserializeObject<T>(requestMessage.BodyData.BodyAsString!);

            case BodyType.Json when requestMessage.BodyData?.BodyAsJson != null:
                return ((JObject)requestMessage.BodyData.BodyAsJson).ToObject<T>()!;

            default:
                throw new NotSupportedException();
        }
    }

    private static T[] DeserializeRequestMessageToArray<T>(IRequestMessage requestMessage)
    {
        if (requestMessage.BodyData?.DetectedBodyType == BodyType.Json && requestMessage.BodyData.BodyAsJson != null)
        {
            var bodyAsJson = requestMessage.BodyData.BodyAsJson;

            return DeserializeObjectToArray<T>(bodyAsJson);
        }

        throw new NotSupportedException();
    }

    private static T[] DeserializeJsonToArray<T>(string value)
    {
        return DeserializeObjectToArray<T>(JsonUtils.DeserializeObject(value));
    }

    private static T[] DeserializeObjectToArray<T>(object value)
    {
        if (value is JArray jArray)
        {
            return jArray.ToObject<T[]>()!;
        }

        var singleResult = ((JObject)value).ToObject<T>();
        return [singleResult!];
    }
}