// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using RestEase;
using Stef.Validation;
using WireMock.Client;
using WireMock.Client.Extensions;
using WireMock.Http;
using WireMock.Net.Testcontainers.Utils;
using WireMock.Util;

namespace WireMock.Net.Testcontainers;

/// <summary>
/// A container for running WireMock in a docker environment.
/// </summary>
public sealed class WireMockContainer : DockerContainer
{
    private const int EnhancedFileSystemWatcherTimeoutMs = 2000;
    private const string HealthStatusHealthy = "Healthy";
    private const int MaxHealthCheckRetries = 10;
    internal const int ContainerPort = 80;

    private readonly WireMockConfiguration _configuration;

    private IWireMockAdminApi? _adminApi;
    private EnhancedFileSystemWatcher? _enhancedFileSystemWatcher;
    private IDictionary<int, Uri>? _publicUris;

    /// <summary>
    /// Initializes a new instance of the <see cref="WireMockContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public WireMockContainer(WireMockConfiguration configuration) : base(configuration)
    {
        _configuration = Guard.NotNull(configuration);

        Started += async (sender, eventArgs) => await WireMockContainerStartedAsync(sender, eventArgs);
    }

    /// <summary>
    /// Gets the public Url.
    /// </summary>
    [PublicAPI]
    public string GetPublicUrl() => GetPublicUri().ToString();

    /// <summary>
    /// Gets the public Urls as a dictionary with the internal port as the key.
    /// </summary>
    [PublicAPI]
    public IDictionary<int, string> GetPublicUrls() => GetPublicUris().ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString());

    /// <summary>
    /// Gets the mapped public port for the given container port.
    /// </summary>
    [PublicAPI]
    public string GetMappedPublicUrl(int containerPort)
    {
        return GetPublicUris()[containerPort].ToString();
    }

    /// <summary>
    /// Create a RestEase Admin client which can be used to call the admin REST endpoint.
    /// </summary>
    /// <returns>A <see cref="IWireMockAdminApi"/></returns>
    [PublicAPI]
    public IWireMockAdminApi CreateWireMockAdminClient()
    {
        ValidateIfRunning();

        var api = RestClient.For<IWireMockAdminApi>(GetPublicUri());
        return _configuration.HasBasicAuthentication ? api.WithAuthorization(_configuration.Username!, _configuration.Password!) : api;
    }

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
        ValidateIfRunning();

        var client = HttpClientFactory2.Create(handlers);
        client.BaseAddress = GetPublicUri();
        return client;
    }

    /// <summary>
    /// Create a <see cref="HttpClient"/> (one for each URL) which can be used to call this instance.
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
    public HttpClient CreateClient(HttpMessageHandler innerHandler, params DelegatingHandler[] handlers)
    {
        ValidateIfRunning();

        var client = HttpClientFactory2.Create(innerHandler, handlers);
        client.BaseAddress = GetPublicUri();
        return client;
    }

    /// <summary>
    /// Copies a test host directory or file to the container and triggers a reload of the static mappings if required.
    /// </summary>
    /// <param name="source">The source directory or file to be copied.</param>
    /// <param name="target">The target directory path to copy the files to.</param>
    /// <param name="uid">The user ID to set for the copied file or directory. Defaults to 0 (root).</param>
    /// <param name="gid">The group ID to set for the copied file or directory. Defaults to 0 (root).</param>
    /// <param name="fileMode">The POSIX file mode permission.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A task that completes when the directory or file has been copied.</returns>
    public new async Task CopyAsync(string source, string target, uint uid = 0, uint gid = 0, UnixFileModes fileMode = Unix.FileMode644, CancellationToken ct = default)
    {
        await base.CopyAsync(source, target, uid, gid, fileMode, ct);

        if (_configuration.WatchStaticMappings && await PathStartsWithContainerMappingsPath(target))
        {
            await ReloadStaticMappingsAsync(target, ct);
        }
    }

    /// <summary>
    /// Reload the static mappings.
    /// </summary>
    /// <param name="cancellationToken">The optional cancellationToken.</param>
    public async Task ReloadStaticMappingsAsync(CancellationToken cancellationToken = default)
    {
        if (_adminApi == null)
        {
            return;
        }

        try
        {
            var result = await _adminApi.ReloadStaticMappingsAsync(cancellationToken);
            Logger.LogInformation("ReloadStaticMappings result: {Result}", result);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Error calling /__admin/mappings/reloadStaticMappings");
        }
    }

    /// <inheritdoc />
    protected override ValueTask DisposeAsyncCore()
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

        return base.DisposeAsyncCore();
    }

    private static async Task<bool> PathStartsWithContainerMappingsPath(string value)
    {
        var imageOs = await TestcontainersUtils.GetImageOSAsync.Value;

        return value.StartsWith(ContainerInfoProvider.Info[imageOs].MappingsPath);
    }

    private void ValidateIfRunning()
    {
        if (State != TestcontainersStates.Running)
        {
            throw new InvalidOperationException("Unable to create HttpClient because the WireMock.Net is not yet running.");
        }
    }

    private async Task WireMockContainerStartedAsync(object sender, EventArgs e)
    {
        _adminApi = CreateWireMockAdminClient();

        RegisterEnhancedFileSystemWatcher();

        await CallAdditionalActionsAfterStartedAsync();
    }

    private void RegisterEnhancedFileSystemWatcher()
    {
        if (!_configuration.WatchStaticMappings || string.IsNullOrEmpty(_configuration.StaticMappingsPath))
        {
            return;
        }

        _enhancedFileSystemWatcher = new EnhancedFileSystemWatcher(_configuration.StaticMappingsPath!, "*.json", EnhancedFileSystemWatcherTimeoutMs)
        {
            IncludeSubdirectories = _configuration.WatchStaticMappingsInSubdirectories
        };
        _enhancedFileSystemWatcher.Created += FileCreatedChangedOrDeleted;
        _enhancedFileSystemWatcher.Changed += FileCreatedChangedOrDeleted;
        _enhancedFileSystemWatcher.Deleted += FileCreatedChangedOrDeleted;
        _enhancedFileSystemWatcher.EnableRaisingEvents = true;
    }

    private async Task CallAdditionalActionsAfterStartedAsync()
    {
        for (int i = 0; i < MaxHealthCheckRetries; i++)
        {
            if (await IsHealthyAsync())
            {
                break;
            }
        }

        foreach (var kvp in _configuration.ProtoDefinitions)
        {
            Logger.LogInformation("Adding ProtoDefinition {Id}", kvp.Key);
            foreach (var protoDefinition in kvp.Value)
            {
                try
                {
                    var result = await _adminApi!.AddProtoDefinitionAsync(kvp.Key, protoDefinition);
                    Logger.LogInformation("AddProtoDefinition '{Id}' result: {Result}", kvp.Key, result);
                }
                catch (Exception ex)
                {
                    Logger.LogWarning(ex, "Error adding ProtoDefinition '{Id}'.", kvp.Key);
                }
            }
        }

        // Force a reload of static mappings when ProtoDefinitions are added at server-level to fix #1382
        if (_configuration.ProtoDefinitions.Count > 0)
        {
            await ReloadStaticMappingsAsync();
        }
    }

    private async void FileCreatedChangedOrDeleted(object sender, FileSystemEventArgs args)
    {
        try
        {
            await ReloadStaticMappingsAsync(args.FullPath);
            Logger.LogInformation("ReloadStaticMappings triggered from file change: '{FullPath}'.", args.FullPath);
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Error reloading static mappings from '{FullPath}'.", args.FullPath);
        }
    }

    private async Task ReloadStaticMappingsAsync(string path, CancellationToken cancellationToken = default)
    {
        Logger.LogInformation("MappingFile created, changed or deleted: '{Path}'. Triggering ReloadStaticMappings.", path);
        await ReloadStaticMappingsAsync(cancellationToken);
    }

    private Uri GetPublicUri() => GetPublicUris()[ContainerPort];

    private IDictionary<int, Uri> GetPublicUris()
    {
        if (_publicUris != null)
        {
            return _publicUris;
        }

        _publicUris = _configuration.ExposedPorts.Keys
            .Select(int.Parse)
            .ToDictionary(port => port, port => new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(port)).Uri);

        foreach (var url in _configuration.AdditionalUrls)
        {
            if (PortUtils.TryExtract(url, out _, out _, out _, out _, out var port))
            {
                _publicUris[port] = new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(port)).Uri;
            }
        }

        return _publicUris;
    }

    private static async Task<bool> IsHealthyAsync()
    {
        try
        {
            var status = await _adminApi.GetHealthAsync();
            return string.Equals(status, HealthStatusHealthy, StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }
}