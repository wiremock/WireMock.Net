// Copyright © WireMock.Net and mock4net by Alexandre Victoor

// This source file is based on mock4net by Alexandre Victoor which is licensed under the Apache 2.0 License.
// For more details see 'mock4net/LICENSE.txt' and 'mock4net/readme.md' in this project root.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime;
using System.Threading;
using AnyOfTypes;
using JetBrains.Annotations;
using JsonConverter.Newtonsoft.Json;
using Newtonsoft.Json;
using Stef.Validation;
using WireMock.Admin.Mappings;
using WireMock.Authentication;
using WireMock.Constants;
using WireMock.Exceptions;
using WireMock.Handlers;
using WireMock.Http;
using WireMock.Logging;
using WireMock.Models;
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
public partial class WireMockServer : IWireMockServer
{
    private const int ServerStartDelayInMs = 100;

    private readonly WireMockServerSettings _settings;
    private readonly IOwinSelfHost? _httpServer;
    private readonly IWireMockMiddlewareOptions _options = new WireMockMiddlewareOptions();
    private readonly MappingConverter _mappingConverter;
    private readonly MatcherMapper _matcherMapper;
    private readonly MappingToFileSaver _mappingToFileSaver;
    private readonly MappingBuilder _mappingBuilder;
    private readonly IGuidUtils _guidUtils = new GuidUtils();
    private readonly IDateTimeUtils _dateTimeUtils = new DateTimeUtils();
    private readonly MappingSerializer _mappingSerializer;

    /// <inheritdoc />
    [PublicAPI]
    public bool IsStarted => _httpServer is { IsStarted: true };

    /// <inheritdoc />
    [PublicAPI]
    public bool IsStartedWithAdminInterface => IsStarted && _settings.StartAdminInterface.GetValueOrDefault();

    /// <inheritdoc />
    [PublicAPI]
    public List<int> Ports { get; }

    /// <inheritdoc />
    [PublicAPI]
    public int Port => Ports?.FirstOrDefault() ?? default;

    /// <inheritdoc />
    [PublicAPI]
    public string[] Urls { get; }

    /// <inheritdoc />
    [PublicAPI]
    public string? Url => Urls?.FirstOrDefault();

    /// <inheritdoc />
    [PublicAPI]
    public string? Consumer { get; private set; }

    /// <inheritdoc />
    [PublicAPI]
    public string? Provider { get; private set; }

    /// <summary>
    /// Gets the mappings.
    /// </summary>
    [PublicAPI]
    public IReadOnlyList<IMapping> Mappings => _options.Mappings.Values.ToArray();

    /// <inheritdoc cref="IWireMockServer.MappingModels" />
    [PublicAPI]
    public IReadOnlyList<MappingModel> MappingModels => ToMappingModels();

    /// <summary>
    /// Gets the scenarios.
    /// </summary>
    [PublicAPI]
    public ConcurrentDictionary<string, ScenarioState> Scenarios => new(_options.Scenarios);

    #region IDisposable Members
    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        _options.LogEntries.CollectionChanged -= LogEntries_CollectionChanged;

        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        DisposeEnhancedFileSystemWatcher();
        _httpServer?.StopAsync();
    }
    #endregion

    #region HttpClient
#if NET5_0_OR_GREATER
    private readonly Lazy<IHttpClientFactory> _lazyHttpClientFactory;

    /// <summary>
    /// Create a <see cref="IHttpClientFactory"/> which can be used to generate a HttpClient to call this instance.
    /// <param name="handlers">
    /// An ordered list of System.Net.Http.DelegatingHandler instances to be invoked
    /// as an System.Net.Http.HttpRequestMessage travels from the System.Net.Http.HttpClient
    /// to the network and an System.Net.Http.HttpResponseMessage travels from the network
    /// back to System.Net.Http.HttpClient. The handlers are invoked in a top-down fashion.
    /// That is, the first entry is invoked first for an outbound request message but
    /// last for an inbound response message.
    /// </param>
    /// </summary>
    [PublicAPI]
    public IHttpClientFactory CreateHttpClientFactory(params DelegatingHandler[] handlers)
    {
        if (!IsStarted)
        {
            throw new InvalidOperationException("Unable to create IHttpClientFactory because the service is not started.");
        }

        return handlers.Length > 0 ? new WireMockHttpClientFactory(this, handlers) : _lazyHttpClientFactory.Value;
    }
#endif

    /// <summary>
    /// Create a <see cref="HttpClient"/> which can be used to call this instance.
    /// <param name="handlers">
    /// An ordered list of System.Net.Http.DelegatingHandler instances to be invoked
    /// as an System.Net.Http.HttpRequestMessage travels from the System.Net.Http.HttpClient
    /// to the network and an System.Net.Http.HttpResponseMessage travels from the network
    /// back to System.Net.Http.HttpClient. The handlers are invoked in a top-down fashion.
    /// That is, the first entry is invoked first for an outbound request message but
    /// last for an inbound response message.
    /// </param>
    /// </summary>
    [PublicAPI]
    public HttpClient CreateClient(params DelegatingHandler[] handlers)
    {
        if (!IsStarted)
        {
            throw new InvalidOperationException("Unable to create HttpClient because the service is not started.");
        }

        var client = HttpClientFactory2.Create(handlers);
        client.BaseAddress = new Uri(Url!);
        return client;
    }

    /// <summary>
    /// Create a <see cref="HttpClient"/> which can be used to call this instance.
    /// <param name="handlers">
    /// <param name="innerHandler">The inner handler represents the destination of the HTTP message channel.</param>
    /// An ordered list of System.Net.Http.DelegatingHandler instances to be invoked
    /// as an System.Net.Http.HttpRequestMessage travels from the System.Net.Http.HttpClient
    /// to the network and an System.Net.Http.HttpResponseMessage travels from the network
    /// back to System.Net.Http.HttpClient. The handlers are invoked in a top-down fashion.
    /// That is, the first entry is invoked first for an outbound request message but
    /// last for an inbound response message.
    /// </param>
    /// </summary>
    [PublicAPI]
    public HttpClient CreateClient(HttpMessageHandler innerHandler, params DelegatingHandler[] handlers)
    {
        if (!IsStarted)
        {
            throw new InvalidOperationException("Unable to create HttpClient because the service is not started.");
        }

        var client = HttpClientFactory2.Create(innerHandler, handlers);
        client.BaseAddress = new Uri(Url!);
        return client;
    }

    /// <summary>
    /// Create <see cref="HttpClient"/>s (one for each URL) which can be used to call this instance.
    /// <param name="innerHandler">The inner handler represents the destination of the HTTP message channel.</param>
    /// <param name="handlers">
    /// An ordered list of System.Net.Http.DelegatingHandler instances to be invoked
    /// as an System.Net.Http.HttpRequestMessage travels from the System.Net.Http.HttpClient
    /// to the network and an System.Net.Http.HttpResponseMessage travels from the network
    /// back to System.Net.Http.HttpClient. The handlers are invoked in a top-down fashion.
    /// That is, the first entry is invoked first for an outbound request message but
    /// last for an inbound response message.
    /// </param>
    /// </summary>
    [PublicAPI]
    public HttpClient[] CreateClients(HttpMessageHandler innerHandler, params DelegatingHandler[] handlers)
    {
        if (!IsStarted)
        {
            throw new InvalidOperationException("Unable to create HttpClients because the service is not started.");
        }

        return Urls.Select(url =>
        {
            var client = HttpClientFactory2.Create(innerHandler, handlers);
            client.BaseAddress = new Uri(url);
            return client;
        }).ToArray();
    }
    #endregion

    #region Start/Stop
    /// <summary>
    /// Starts this WireMockServer with the specified settings.
    /// </summary>
    /// <param name="settings">The WireMockServerSettings.</param>
    /// <returns>The <see cref="WireMockServer"/>.</returns>
    [PublicAPI]
    public static WireMockServer Start(WireMockServerSettings settings)
    {
        Guard.NotNull(settings);

        return new WireMockServer(settings);
    }

    /// <summary>
    /// Starts this WireMockServer with the specified settings.
    /// </summary>
    /// <param name="action">The action to configure the WireMockServerSettings.</param>
    /// <returns>The <see cref="WireMockServer"/>.</returns>
    [PublicAPI]
    public static WireMockServer Start(Action<WireMockServerSettings> action)
    {
        Guard.NotNull(action);

        var settings = new WireMockServerSettings();

        action(settings);

        return new WireMockServer(settings);
    }

    /// <summary>
    /// Start this WireMockServer.
    /// </summary>
    /// <param name="port">The port.</param>
    /// <param name="useSSL">The SSL support.</param>
    /// <param name="useHttp2">Use HTTP 2 (needed for Grpc).</param>
    /// <returns>The <see cref="WireMockServer"/>.</returns>
    [PublicAPI]
    public static WireMockServer Start(int? port = 0, bool useSSL = false, bool useHttp2 = false)
    {
        return new WireMockServer(new WireMockServerSettings
        {
            Port = port,
            UseSSL = useSSL,
            UseHttp2 = useHttp2
        });
    }

    /// <summary>
    /// Start this WireMockServer.
    /// </summary>
    /// <param name="urls">The urls to listen on.</param>
    /// <returns>The <see cref="WireMockServer"/>.</returns>
    [PublicAPI]
    public static WireMockServer Start(params string[] urls)
    {
        Guard.NotNullOrEmpty(urls);

        return new WireMockServer(new WireMockServerSettings
        {
            Urls = urls
        });
    }

    /// <summary>
    /// Start this WireMockServer with the admin interface.
    /// </summary>
    /// <param name="port">The port.</param>
    /// <param name="useSSL">The SSL support.</param>
    /// <param name="useHttp2">Use HTTP 2 (needed for Grpc).</param>
    /// <returns>The <see cref="WireMockServer"/>.</returns>
    [PublicAPI]
    public static WireMockServer StartWithAdminInterface(int? port = 0, bool useSSL = false, bool useHttp2 = false)
    {
        return new WireMockServer(new WireMockServerSettings
        {
            Port = port,
            UseSSL = useSSL,
            UseHttp2 = useHttp2,
            StartAdminInterface = true
        });
    }

    /// <summary>
    /// Start this WireMockServer with the admin interface.
    /// </summary>
    /// <param name="urls">The urls.</param>
    /// <returns>The <see cref="WireMockServer"/>.</returns>
    [PublicAPI]
    public static WireMockServer StartWithAdminInterface(params string[] urls)
    {
        Guard.NotNullOrEmpty(urls);

        return new WireMockServer(new WireMockServerSettings
        {
            Urls = urls,
            StartAdminInterface = true
        });
    }

    /// <summary>
    /// Start this WireMockServer with the admin interface and read static mappings.
    /// </summary>
    /// <param name="urls">The urls.</param>
    /// <returns>The <see cref="WireMockServer"/>.</returns>
    [PublicAPI]
    public static WireMockServer StartWithAdminInterfaceAndReadStaticMappings(params string[] urls)
    {
        Guard.NotNullOrEmpty(urls);

        return new WireMockServer(new WireMockServerSettings
        {
            Urls = urls,
            StartAdminInterface = true,
            ReadStaticMappings = true
        });
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WireMockServer"/> class.
    /// </summary>
    /// <param name="settings">The settings.</param>
    /// <exception cref="WireMockException">
    /// Service start failed with error: {_httpServer.RunningException.Message}
    /// or
    /// Service start failed with error: {startTask.Exception.Message}
    /// </exception>
    /// <exception cref="TimeoutException">Service start timed out after {TimeSpan.FromMilliseconds(settings.StartTimeout)}</exception>
    protected WireMockServer(WireMockServerSettings settings)
    {
        _settings = Guard.NotNull(settings);

        _mappingSerializer = new MappingSerializer(settings.MappingJsonSerializer ?? new NewtonsoftJsonConverter());

        // Set default values if not provided
        _settings.Logger = settings.Logger ?? new WireMockNullLogger();
        _settings.FileSystemHandler = settings.FileSystemHandler ?? new LocalFileSystemHandler();

        _settings.Logger.Info("By Stef Heyenrath (https://github.com/wiremock/WireMock.Net)");
        _settings.Logger.Debug("Server settings {0}", JsonConvert.SerializeObject(settings, Formatting.Indented));

        HostUrlOptions urlOptions;
        if (settings.Urls != null)
        {
            urlOptions = new HostUrlOptions
            {
                Urls = settings.Urls
            };
        }
        else
        {
            if (settings.HostingScheme is not null)
            {
                urlOptions = new HostUrlOptions
                {
                    HostingScheme = settings.HostingScheme.Value,
                    UseHttp2 = settings.UseHttp2,
                    Port = settings.Port
                };
            }
            else
            {
                urlOptions = new HostUrlOptions
                {
                    HostingScheme = settings.UseSSL == true ? HostingScheme.Https : HostingScheme.Http,
                    UseHttp2 = settings.UseHttp2,
                    Port = settings.Port
                };
            }
        }

        WireMockMiddlewareOptionsHelper.InitFromSettings(settings, _options, o =>
        {
            o.LogEntries.CollectionChanged += LogEntries_CollectionChanged;
        });

        _matcherMapper = new MatcherMapper(_settings);
        _mappingConverter = new MappingConverter(_matcherMapper);
        _mappingToFileSaver = new MappingToFileSaver(_settings, _mappingConverter);
        _mappingBuilder = new MappingBuilder(
            settings,
            _options,
            _mappingConverter,
            _mappingToFileSaver,
            _guidUtils,
            _dateTimeUtils
        );

#if USE_ASPNETCORE
        _options.AdditionalServiceRegistration = _settings.AdditionalServiceRegistration;
        _options.CorsPolicyOptions = _settings.CorsPolicyOptions;
        _options.ClientCertificateMode = _settings.ClientCertificateMode;
        _options.AcceptAnyClientCertificate = _settings.AcceptAnyClientCertificate;

        _httpServer = new AspNetCoreSelfHost(_options, urlOptions);
#else
        _httpServer = new OwinSelfHost(_options, urlOptions);
#endif
        var startTask = _httpServer.StartAsync();

        using (var ctsStartTimeout = new CancellationTokenSource(settings.StartTimeout))
        {
            while (!_httpServer.IsStarted)
            {
                // Throw exception if service start fails
                if (_httpServer.RunningException != null)
                {
                    throw new WireMockException($"Service start failed with error: {_httpServer.RunningException.Message}", _httpServer.RunningException);
                }

                if (ctsStartTimeout.IsCancellationRequested)
                {
                    // In case of an aggregate exception, throw the exception.
                    if (startTask.Exception != null)
                    {
                        throw new WireMockException($"Service start failed with error: {startTask.Exception.Message}", startTask.Exception);
                    }

                    // Else throw TimeoutException
                    throw new TimeoutException($"Service start timed out after {TimeSpan.FromMilliseconds(settings.StartTimeout)}");
                }

                ctsStartTimeout.Token.WaitHandle.WaitOne(ServerStartDelayInMs);
            }

            Urls = _httpServer.Urls.ToArray();
            Ports = _httpServer.Ports;
        }

        InitSettings(settings);

#if NET5_0_OR_GREATER
        _lazyHttpClientFactory = new Lazy<IHttpClientFactory>(() => new WireMockHttpClientFactory(this));
#endif
    }

    /// <inheritdoc cref="IWireMockServer.Stop" />
    [PublicAPI]
    public void Stop()
    {
        var result = _httpServer?.StopAsync();
        result?.Wait(); // wait for stop to actually happen
    }
    #endregion

    /// <inheritdoc cref="IWireMockServer.AddCatchAllMapping" />
    [PublicAPI]
    public void AddCatchAllMapping()
    {
        Given(Request.Create().WithPath("/*").UsingAnyMethod())
            .WithGuid(Guid.Parse("90008000-0000-4444-a17e-669cd84f1f05"))
            .AtPriority(1000)
            .RespondWith(new DynamicResponseProvider(_ => ResponseMessageBuilder.Create(HttpStatusCode.NotFound, WireMockConstants.NoMatchingFound)));
    }

    /// <inheritdoc cref="IWireMockServer.Reset" />
    [PublicAPI]
    public void Reset()
    {
        ResetLogEntries();

        ResetScenarios();

        ResetMappings();
    }

    /// <inheritdoc cref="IWireMockServer.ResetMappings" />
    [PublicAPI]
    public void ResetMappings()
    {
        foreach (var nonAdmin in _options.Mappings.ToArray().Where(m => !m.Value.IsAdminInterface))
        {
            _options.Mappings.TryRemove(nonAdmin.Key, out _);
        }
    }

    /// <inheritdoc cref="IWireMockServer.DeleteMapping" />
    [PublicAPI]
    public bool DeleteMapping(Guid guid)
    {
        // Check a mapping exists with the same GUID, if so, remove it.
        if (_options.Mappings.ContainsKey(guid))
        {
            return _options.Mappings.TryRemove(guid, out _);
        }

        return false;
    }

    private bool DeleteMapping(string path)
    {
        // Check a mapping exists with the same path, if so, remove it.
        var mapping = _options.Mappings.ToArray().FirstOrDefault(entry => string.Equals(entry.Value.Path, path, StringComparison.OrdinalIgnoreCase));
        return DeleteMapping(mapping.Key);
    }

    /// <inheritdoc cref="IWireMockServer.AddGlobalProcessingDelay" />
    [PublicAPI]
    public void AddGlobalProcessingDelay(TimeSpan delay)
    {
        _options.RequestProcessingDelay = delay;
    }

    /// <inheritdoc cref="IWireMockServer.AllowPartialMapping" />
    [PublicAPI]
    public void AllowPartialMapping(bool allow = true)
    {
        _settings.Logger.Info("AllowPartialMapping is set to {0}", allow);
        _options.AllowPartialMapping = allow;
    }

    /// <inheritdoc cref="IWireMockServer.SetAzureADAuthentication(string, string)" />
    [PublicAPI]
    public void SetAzureADAuthentication(string tenant, string audience)
    {
        Guard.NotNull(tenant);
        Guard.NotNull(audience);

#if NETSTANDARD1_3
        throw new NotSupportedException("AzureADAuthentication is not supported for NETStandard 1.3");
#else
        _options.AuthenticationMatcher = new AzureADAuthenticationMatcher(
            new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler(),
            new Microsoft.IdentityModel.Protocols.ConfigurationManager<Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectConfiguration>($"https://login.microsoftonline.com/{tenant}/.well-known/openid-configuration", new Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectConfigurationRetriever()),
            tenant,
            audience);
#endif
    }

    /// <inheritdoc cref="IWireMockServer.SetBasicAuthentication(string, string)" />
    [PublicAPI]
    public void SetBasicAuthentication(string username, string password)
    {
        Guard.NotNull(username);
        Guard.NotNull(password);

        _options.AuthenticationMatcher = new BasicAuthenticationMatcher(username, password);
    }

    /// <inheritdoc cref="IWireMockServer.RemoveAuthentication" />
    [PublicAPI]
    public void RemoveAuthentication()
    {
        _options.AuthenticationMatcher = null;
    }

    /// <inheritdoc cref="IWireMockServer.SetMaxRequestLogCount" />
    [PublicAPI]
    public void SetMaxRequestLogCount(int? maxRequestLogCount)
    {
        _options.MaxRequestLogCount = maxRequestLogCount;
    }

    /// <inheritdoc cref="IWireMockServer.SetRequestLogExpirationDuration" />
    [PublicAPI]
    public void SetRequestLogExpirationDuration(int? requestLogExpirationDuration)
    {
        _options.RequestLogExpirationDuration = requestLogExpirationDuration;
    }

    /// <inheritdoc cref="IWireMockServer.ResetScenarios" />
    [PublicAPI]
    public void ResetScenarios()
    {
        _options.Scenarios.Clear();
    }

    /// <inheritdoc />
    [PublicAPI]
    public bool ResetScenario(string name)
    {
        return _options.Scenarios.ContainsKey(name) && _options.Scenarios.TryRemove(name, out _);
    }

    /// <inheritdoc />
    [PublicAPI]
    public bool SetScenarioState(string name, string? state)
    {
        if (state == null)
        {
            return ResetScenario(name);
        }

        _options.Scenarios.AddOrUpdate(
            name,
            _ => new ScenarioState
            {
                Name = name,
                NextState = state
            },
            (_, current) =>
            {
                current.NextState = state;
                return current;
            }
        );

        return true;
    }

    /// <inheritdoc cref="IWireMockServer.WithMapping(MappingModel[])" />
    [PublicAPI]
    public IWireMockServer WithMapping(params MappingModel[] mappings)
    {
        foreach (var mapping in mappings)
        {
            ConvertMappingAndRegisterAsRespondProvider(mapping, mapping.Guid ?? Guid.NewGuid());
        }

        return this;
    }

    /// <inheritdoc cref="IWireMockServer.WithMapping(string)" />
    [PublicAPI]
    public IWireMockServer WithMapping(string mappings)
    {
        var mappingModels = _mappingSerializer.DeserializeJsonToArray<MappingModel>(mappings);
        foreach (var mappingModel in mappingModels)
        {
            ConvertMappingAndRegisterAsRespondProvider(mappingModel, mappingModel.Guid ?? Guid.NewGuid());
        }

        return this;
    }

    /// <summary>
    /// Add a Grpc ProtoDefinition at server-level.
    /// </summary>
    /// <param name="id">Unique identifier for the ProtoDefinition.</param>
    /// <param name="protoDefinition">The ProtoDefinition as text.</param>
    /// <returns><see cref="WireMockServer"/></returns>
    [PublicAPI]
    public WireMockServer AddProtoDefinition(string id, params string[] protoDefinition)
    {
        Guard.NotNullOrWhiteSpace(id);
        Guard.NotNullOrEmpty(protoDefinition);

        _settings.ProtoDefinitions ??= new Dictionary<string, string[]>();

        if (_settings.ProtoDefinitions.TryGetValue(id, out var existingProtoDefinitions))
        {
            _settings.ProtoDefinitions[id] = existingProtoDefinitions.Union(protoDefinition).ToArray();
        }
        else
        {
            _settings.ProtoDefinitions[id] = protoDefinition;
        }

        return this;
    }

    /// <summary>
    /// Add a GraphQL Schema at server-level.
    /// </summary>
    /// <param name="id">Unique identifier for the GraphQL Schema.</param>
    /// <param name="graphQLSchema">The GraphQL Schema as string or StringPattern.</param>
    /// <param name="customScalars">A dictionary defining the custom scalars used in this schema. [optional]</param>
    /// <returns><see cref="WireMockServer"/></returns>
    [PublicAPI]
    public WireMockServer AddGraphQLSchema(string id, AnyOf<string, StringPattern> graphQLSchema, Dictionary<string, Type>? customScalars = null)
    {
        Guard.NotNullOrWhiteSpace(id);
        Guard.NotNullOrWhiteSpace(graphQLSchema);

        _settings.GraphQLSchemas ??= new Dictionary<string, GraphQLSchemaDetails>();

        _settings.GraphQLSchemas[id] = new GraphQLSchemaDetails
        {
            SchemaAsString = graphQLSchema,
            CustomScalars = customScalars
        };

        return this;
    }

    /// <inheritdoc />
    [PublicAPI]
    public string? MappingToCSharpCode(Guid guid, MappingConverterType converterType)
    {
        return _mappingBuilder.ToCSharpCode(guid, converterType);
    }

    /// <inheritdoc />
    [PublicAPI]
    public string MappingsToCSharpCode(MappingConverterType converterType)
    {
        return _mappingBuilder.ToCSharpCode(converterType);
    }

    private void InitSettings(WireMockServerSettings settings)
    {
        if (settings.AllowBodyForAllHttpMethods == true)
        {
            _settings.Logger.Info("AllowBodyForAllHttpMethods is set to True");
        }

        if (settings.AllowOnlyDefinedHttpStatusCodeInResponse == true)
        {
            _settings.Logger.Info("AllowOnlyDefinedHttpStatusCodeInResponse is set to True");
        }

        if (settings.AllowPartialMapping == true)
        {
            AllowPartialMapping();
        }

        if (settings.StartAdminInterface == true)
        {
            if (!string.IsNullOrEmpty(settings.AdminUsername) && !string.IsNullOrEmpty(settings.AdminPassword))
            {
                SetBasicAuthentication(settings.AdminUsername!, settings.AdminPassword!);
            }

            if (!string.IsNullOrEmpty(settings.AdminAzureADTenant) && !string.IsNullOrEmpty(settings.AdminAzureADAudience))
            {
                SetAzureADAuthentication(settings.AdminAzureADTenant!, settings.AdminAzureADAudience!);
            }

            InitAdmin();
        }

        if (settings.ReadStaticMappings == true)
        {
            ReadStaticMappings();
        }

        if (settings.WatchStaticMappings == true)
        {
            WatchStaticMappings();
        }

        InitProxyAndRecord(settings);

        if (settings.RequestLogExpirationDuration != null)
        {
            SetRequestLogExpirationDuration(settings.RequestLogExpirationDuration);
        }

        if (settings.MaxRequestLogCount != null)
        {
            SetMaxRequestLogCount(settings.MaxRequestLogCount);
        }
    }
}