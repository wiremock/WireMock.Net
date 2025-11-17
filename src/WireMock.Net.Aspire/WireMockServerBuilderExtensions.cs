// Copyright © WireMock.Net

using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Lifecycle;
using Aspire.Hosting.WireMock;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Stef.Validation;
using WireMock.Client.Builders;
using WireMock.Net.Aspire;

// ReSharper disable once CheckNamespace
namespace Aspire.Hosting;

/// <summary>
/// Provides extension methods for adding WireMock.Net Server resources to the application model.
/// </summary>
public static class WireMockServerBuilderExtensions
{
    // Linux only (https://github.com/dotnet/aspire/issues/854)
    private const string DefaultLinuxImage = "sheyenrath/wiremock.net-alpine";
    private const string DefaultLinuxMappingsPath = "/app/__admin/mappings";

    /// <summary>
    /// Adds a WireMock.Net Server resource to the application model.
    /// </summary>
    /// <param name="builder">The <see cref="IDistributedApplicationBuilder"/>.</param>
    /// <param name="name">The name of the resource. This name will be used as the connection string name when referenced in a dependency.</param>
    /// <param name="port">The HTTP port for the WireMock Server.</param>
    /// <returns>A reference to the <see cref="IResourceBuilder{WireMockServerResource}"/>.</returns>
    public static IResourceBuilder<WireMockServerResource> AddWireMock(this IDistributedApplicationBuilder builder, string name, int? port = null)
    {
        Guard.NotNull(builder);
        Guard.NotNullOrWhiteSpace(name);
        Guard.Condition(port, p => p is null or > 0 and <= ushort.MaxValue);

        return builder.AddWireMock(name, callback =>
        {
            callback.HttpPort = port;
        });
    }

    /// <summary>
    /// Adds a WireMock.Net Server resource to the application model.
    /// </summary>
    /// <param name="builder">The <see cref="IDistributedApplicationBuilder"/>.</param>
    /// <param name="name">The name of the resource. This name will be used as the connection string name when referenced in a dependency.</param>
    /// <param name="arguments">The arguments to start the WireMock.Net Server.</param>
    /// <returns>A reference to the <see cref="IResourceBuilder{WireMockServerResource}"/>.</returns>
    public static IResourceBuilder<WireMockServerResource> AddWireMock(this IDistributedApplicationBuilder builder, string name, WireMockServerArguments arguments)
    {
        Guard.NotNull(builder);
        Guard.NotNullOrWhiteSpace(name);
        Guard.NotNull(arguments);

        var wireMockContainerResource = new WireMockServerResource(name, arguments);

        var healthCheckKey = $"{name}_check";
        var healthCheckRegistration = new HealthCheckRegistration(
            healthCheckKey,
            _ => new WireMockHealthCheck(wireMockContainerResource),
            failureStatus: null,
            tags: null);
        builder.Services.AddHealthChecks().Add(healthCheckRegistration);

        var resourceBuilder = builder
            .AddResource(wireMockContainerResource)
            .WithImage(DefaultLinuxImage)
            .WithEnvironment(ctx => ctx.EnvironmentVariables.Add("DOTNET_USE_POLLING_FILE_WATCHER", "1")) // https://khalidabuhakmeh.com/aspnet-docker-gotchas-and-workarounds#configuration-reloads-and-filesystemwatcher
            .WithHttpEndpoint(port: arguments.HttpPort, targetPort: WireMockServerArguments.HttpContainerPort)
            .WithHealthCheck(healthCheckKey)
            .WithWireMockInspectorCommand();

        if (!string.IsNullOrEmpty(arguments.MappingsPath))
        {
            resourceBuilder = resourceBuilder.WithBindMount(arguments.MappingsPath, DefaultLinuxMappingsPath);
        }

        resourceBuilder = resourceBuilder.WithArgs(ctx =>
        {
            foreach (var arg in arguments.GetArgs())
            {
                ctx.Args.Add(arg);
            }
        });

        return resourceBuilder;
    }

    /// <summary>
    /// Adds a WireMock.Net Server resource to the application model.
    /// </summary>
    /// <param name="builder">The <see cref="IDistributedApplicationBuilder"/>.</param>
    /// <param name="name">The name of the resource. This name will be used as the connection string name when referenced in a dependency.</param>
    /// <param name="callback">A callback that allows for setting the <see cref="WireMockServerArguments"/>.</param>
    /// <returns>A reference to the <see cref="IResourceBuilder{WireMockServerResource}"/>.</returns>
    public static IResourceBuilder<WireMockServerResource> AddWireMock(this IDistributedApplicationBuilder builder, string name, Action<WireMockServerArguments> callback)
    {
        Guard.NotNull(builder);
        Guard.NotNullOrWhiteSpace(name);
        Guard.NotNull(callback);

        var arguments = new WireMockServerArguments();
        callback(arguments);

        return builder.AddWireMock(name, arguments);
    }

    /// <summary>
    /// Defines if the static mappings should be read at startup.
    ///
    /// Default set to <c>false</c>.
    /// </summary>
    /// <returns>A reference to the <see cref="IResourceBuilder{WireMockServerResource}"/>.</returns>
    public static IResourceBuilder<WireMockServerResource> WithReadStaticMappings(this IResourceBuilder<WireMockServerResource> wiremock)
    {
        Guard.NotNull(wiremock).Resource.Arguments.ReadStaticMappings = true;
        return wiremock;
    }

    /// <summary>
    /// Watch the static mapping files + folder for changes when running.
    ///
    /// Default set to <c>false</c>.
    /// </summary>
    /// <returns>A reference to the <see cref="IResourceBuilder{WireMockServerResource}"/>.</returns>
    public static IResourceBuilder<WireMockServerResource> WithWatchStaticMappings(this IResourceBuilder<WireMockServerResource> wiremock)
    {
        Guard.NotNull(wiremock).Resource.Arguments.WatchStaticMappings = true;
        return wiremock;
    }

    /// <summary>
    /// Specifies the path for the (static) mapping json files.
    /// </summary>
    /// <param name="wiremock">The <see cref="IResourceBuilder{WireMockServerResource}"/>.</param>
    /// <param name="mappingsPath">The local path.</param>
    /// <returns>A reference to the <see cref="IResourceBuilder{WireMockServerResource}"/>.</returns>
    public static IResourceBuilder<WireMockServerResource> WithMappingsPath(this IResourceBuilder<WireMockServerResource> wiremock, string mappingsPath)
    {
        Guard.NotNullOrWhiteSpace(mappingsPath);
        Guard.NotNull(wiremock).Resource.Arguments.MappingsPath = mappingsPath;

        return wiremock.WithBindMount(mappingsPath, DefaultLinuxMappingsPath);
    }

    /// <summary>
    /// Set the admin username and password for accessing the admin interface from WireMock.Net via HTTP.
    /// </summary>
    /// <param name="wiremock">The <see cref="IResourceBuilder{WireMockServerResource}"/>.</param>
    /// <param name="username">The admin username.</param>
    /// <param name="password">The admin password.</param>
    /// <returns>A reference to the <see cref="IResourceBuilder{WireMockServerResource}"/>.</returns>
    public static IResourceBuilder<WireMockServerResource> WithAdminUserNameAndPassword(this IResourceBuilder<WireMockServerResource> wiremock, string username, string password)
    {
        Guard.NotNull(wiremock);

        wiremock.Resource.Arguments.AdminUsername = Guard.NotNull(username);
        wiremock.Resource.Arguments.AdminPassword = Guard.NotNull(password);
        return wiremock;
    }

    /// <summary>
    /// Use WireMock Client's AdminApiMappingBuilder to configure the WireMock.Net resource.
    /// </summary>
    /// <param name="wiremock">The <see cref="IResourceBuilder{WireMockServerResource}"/>.</param>
    /// <param name="configure">Delegate that will be invoked to configure the WireMock.Net resource.</param>
    /// <returns></returns>
    public static IResourceBuilder<WireMockServerResource> WithApiMappingBuilder(this IResourceBuilder<WireMockServerResource> wiremock, Func<AdminApiMappingBuilder, Task> configure)
    {
        return wiremock.WithApiMappingBuilder((adminApiMappingBuilder, _) => configure.Invoke(adminApiMappingBuilder));
    }

    /// <summary>
    /// Use WireMock Client's AdminApiMappingBuilder to configure the WireMock.Net resource.
    /// </summary>
    /// <param name="wiremock">The <see cref="IResourceBuilder{WireMockServerResource}"/>.</param>
    /// <param name="configure">Delegate that will be invoked to configure the WireMock.Net resource.</param>
    /// <returns></returns>
    public static IResourceBuilder<WireMockServerResource> WithApiMappingBuilder(this IResourceBuilder<WireMockServerResource> wiremock, Func<AdminApiMappingBuilder, CancellationToken, Task> configure)
    {
        Guard.NotNull(wiremock);

        wiremock.ApplicationBuilder.Services.TryAddLifecycleHook<WireMockServerLifecycleHook>();
        wiremock.Resource.Arguments.ApiMappingBuilder = configure;
        wiremock.Resource.ApiMappingState = WireMockMappingState.NotSubmitted;

        return wiremock;
    }

    /// <summary>
    /// Enables the WireMockInspect, a cross-platform UI app that facilitates WireMock troubleshooting.
    /// This requires installation of the WireMockInspector tool.
    /// <code>
    /// dotnet tool install WireMockInspector --global --no-cache --ignore-failed-sources
    /// </code>
    /// </summary>
    /// <param name="builder">The <see cref="IResourceBuilder{WireMockNetResource}"/>.</param>
    /// <returns></returns>
    public static IResourceBuilder<WireMockServerResource> WithWireMockInspectorCommand(this IResourceBuilder<WireMockServerResource> builder)
    {
        Guard.NotNull(builder);

        CommandOptions commandOptions = new()
        {
            Description = "Requires installation of the WireMockInspector (https://github.com/WireMock-Net/WireMockInspector) tool:\ndotnet tool install WireMockInspector --global --no-cache --ignore-failed-sources",
            UpdateState = OnUpdateResourceState,
            IconName = "BoxSearch",
            IconVariant = IconVariant.Filled
        };

        builder.WithCommand(
            name: "wiremock-inspector",
            displayName: "WireMock Inspector",
            executeCommand: _ => OnRunOpenInspectorCommandAsync(builder),
            commandOptions: commandOptions);

        return builder;
    }

    private static Task<ExecuteCommandResult> OnRunOpenInspectorCommandAsync(IResourceBuilder<WireMockServerResource> builder)
    {
        WireMockInspector.Inspect(builder.Resource.GetEndpoint().Url);

        return Task.FromResult(CommandResults.Success());
    }

    private static ResourceCommandState OnUpdateResourceState(UpdateCommandStateContext context)
    {
        return context.ResourceSnapshot.HealthStatus is HealthStatus.Healthy
            ? ResourceCommandState.Enabled
            : ResourceCommandState.Disabled;
    }
}