// Copyright © WireMock.Net

using Microsoft.Extensions.Logging;
using RestEase;
using Stef.Validation;
using WireMock.Client;
using WireMock.Client.Extensions;
using WireMock.Net.Aspire;
using WireMock.Util;

// ReSharper disable once CheckNamespace
namespace Aspire.Hosting.ApplicationModel;

/// <summary>
/// A resource that represents a WireMock.Net Server.
/// </summary>
public class WireMockServerResource : ContainerResource, IResourceWithServiceDiscovery
{
    private const int EnhancedFileSystemWatcherTimeoutMs = 2000;

    internal WireMockServerArguments Arguments { get; }
    internal Lazy<IWireMockAdminApi> AdminApi => new(CreateWireMockAdminApi);
    internal WireMockMappingState ApiMappingState { get; set; } = WireMockMappingState.NoMappings;

    private ILogger? _logger;
    private EnhancedFileSystemWatcher? _enhancedFileSystemWatcher;

    /// <summary>
    /// Initializes a new instance of the <see cref="WireMockServerResource"/> class.
    /// </summary>
    /// <param name="name">The name of the resource.</param>
    /// <param name="arguments">The arguments to start the WireMock.Net Server.</param>
    public WireMockServerResource(string name, WireMockServerArguments arguments) : base(name)
    {
        Arguments = Guard.NotNull(arguments);
    }

    /// <summary>
    /// Gets an endpoint reference.
    /// </summary>
    /// <returns>An <see cref="EndpointReference"/> object representing the endpoint reference.</returns>
    public EndpointReference GetEndpoint()
    {
        return new EndpointReference(this, "http");
    }

    internal void SetLogger(ILogger logger)
    {
        _logger = logger;
    }

    internal async Task WaitForHealthAsync(CancellationToken cancellationToken)
    {
        _logger?.LogInformation("Checking Health status from WireMock.Net");
        await AdminApi.Value.WaitForHealthAsync(cancellationToken: cancellationToken);
    }

    internal async Task CallApiMappingBuilderActionAsync(CancellationToken cancellationToken)
    {
        if (Arguments.ApiMappingBuilder == null)
        {
            return;
        }

        _logger?.LogInformation("Calling ApiMappingBuilder to add mappings to WireMock.Net");

        var mappingBuilder = AdminApi.Value.GetMappingBuilder();
        await Arguments.ApiMappingBuilder.Invoke(mappingBuilder, cancellationToken);

        ApiMappingState = WireMockMappingState.Submitted;
    }

    internal async Task CallAddProtoDefinitionsAsync(CancellationToken cancellationToken)
    {
        _logger?.LogInformation("Calling AdminApi to add GRPC ProtoDefinition at server level to WireMock.Net");

        foreach (var (id, protoDefinitions) in Arguments.ProtoDefinitions)
        {
            _logger?.LogInformation("Adding ProtoDefinition {Id}", id);
            foreach (var protoDefinition in protoDefinitions)
            {
                try
                {
                    var status = await AdminApi.Value.AddProtoDefinitionAsync(id, protoDefinition, cancellationToken);
                    _logger?.LogInformation("ProtoDefinition '{Id}' added with status: {Status}.", id, status.Status);
                }
                catch (Exception ex)
                {
                    _logger?.LogWarning(ex, "Error adding ProtoDefinition '{Id}'.", id);
                }
            }
        }

        // Force a reload of static mappings when ProtoDefinitions are added at server-level to fix #1382
        if (Arguments.ProtoDefinitions.Count > 0)
        {
            await ReloadStaticMappingsAsync(default);
        }
    }

    internal void StartWatchingStaticMappings(CancellationToken cancellationToken)
    {
        if (!Arguments.WatchStaticMappings || string.IsNullOrEmpty(Arguments.MappingsPath))
        {
            return;
        }

        cancellationToken.Register(() =>
        {
            if (_enhancedFileSystemWatcher != null)
            {
                _enhancedFileSystemWatcher.EnableRaisingEvents = false;
                _enhancedFileSystemWatcher.Created -= FileCreatedChangedOrDeleted;
                _enhancedFileSystemWatcher.Changed -= FileCreatedChangedOrDeleted;
                _enhancedFileSystemWatcher.Deleted -= FileCreatedChangedOrDeleted;

                _enhancedFileSystemWatcher.Dispose();
                _enhancedFileSystemWatcher = null;
            }
        });

        _logger?.LogInformation("Starting to watch static mappings on path: '{Path}'. ", Arguments.MappingsPath);

        _enhancedFileSystemWatcher = new EnhancedFileSystemWatcher(Arguments.MappingsPath, "*.json", EnhancedFileSystemWatcherTimeoutMs)
        {
            IncludeSubdirectories = true
        };
        _enhancedFileSystemWatcher.Created += FileCreatedChangedOrDeleted;
        _enhancedFileSystemWatcher.Changed += FileCreatedChangedOrDeleted;
        _enhancedFileSystemWatcher.Deleted += FileCreatedChangedOrDeleted;
        _enhancedFileSystemWatcher.EnableRaisingEvents = true;
    }

    private IWireMockAdminApi CreateWireMockAdminApi()
    {
        var adminApi = RestClient.For<IWireMockAdminApi>(GetEndpoint().Url);
        return Arguments.HasBasicAuthentication ?
            adminApi.WithAuthorization(Arguments.AdminUsername!, Arguments.AdminPassword!) :
            adminApi;
    }

    private async void FileCreatedChangedOrDeleted(object sender, FileSystemEventArgs args)
    {
        _logger?.LogInformation("MappingFile created, changed or deleted: '{FullPath}'. Triggering ReloadStaticMappings.", args.FullPath);

        await ReloadStaticMappingsAsync(default);
    }

    private async Task ReloadStaticMappingsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var status = await AdminApi.Value.ReloadStaticMappingsAsync(cancellationToken);
            _logger?.LogInformation("ReloadStaticMappings called with status: {Status}.", status);
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex, "Error calling /__admin/mappings/reloadStaticMappings");
        }
    }
}