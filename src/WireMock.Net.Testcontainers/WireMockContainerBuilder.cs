// Copyright © WireMock.Net

using System;
using System.Linq;
using System.Runtime.InteropServices;
using Docker.DotNet.Models;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using JetBrains.Annotations;
using Stef.Validation;
using WireMock.Net.Testcontainers.Utils;
using WireMock.Util;

namespace WireMock.Net.Testcontainers;

/// <summary>
/// A specific fluent Docker container builder for WireMock.Net
/// </summary>
public sealed class WireMockContainerBuilder : ContainerBuilder<WireMockContainerBuilder, WireMockContainer, WireMockConfiguration>
{
    private const string DefaultLogger = "WireMockConsoleLogger";
    private OSPlatform? _imageOS;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContainerBuilder" /> class.
    /// </summary>
    public WireMockContainerBuilder() : this(new WireMockConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Automatically use the correct image for WireMock.
    /// For Linux this is "sheyenrath/wiremock.net-alpine:latest"
    /// For Windows this is "sheyenrath/wiremock.net-windows:latest"
    /// </summary>
    /// <returns>A configured instance of <see cref="WireMockContainerBuilder"/></returns>
    [PublicAPI]
    public WireMockContainerBuilder WithImage()
    {
        _imageOS ??= TestcontainersUtils.GetImageOSAsync.Value.GetAwaiter().GetResult();
        return WithImage(_imageOS.Value);
    }

    /// <summary>
    /// Automatically use a Linux image for WireMock. This is "sheyenrath/wiremock.net-alpine:latest"
    /// </summary>
    /// <returns>A configured instance of <see cref="WireMockContainerBuilder"/></returns>
    [PublicAPI]
    public WireMockContainerBuilder WithLinuxImage()
    {
        return WithImage(OSPlatform.Linux);
    }

    /// <summary>
    /// Automatically use a Windows image for WireMock. This is "sheyenrath/wiremock.net-windows:latest"
    /// </summary>
    /// <returns>A configured instance of <see cref="WireMockContainerBuilder"/></returns>
    [PublicAPI]
    public WireMockContainerBuilder WithWindowsImage()
    {
        return WithImage(OSPlatform.Windows);
    }

    /// <summary>
    /// Sets a custom WireMock.Net image for which to create the container.
    /// </summary>
    /// <param name="image">The image name.</param>
    /// <returns>A configured instance of <see cref="WireMockContainerBuilder"/></returns>
    [PublicAPI]
    public new WireMockContainerBuilder WithImage(string image)
    {
        return WithCustomImage(image);
    }

    /// <summary>
    /// Sets a custom WireMock.Net image for which to create the container.
    /// </summary>
    /// <param name="image">The image name.</param>
    /// <returns>A configured instance of <see cref="WireMockContainerBuilder"/></returns>
    [PublicAPI]
    public WireMockContainerBuilder WithCustomImage(string image)
    {
        _imageOS ??= TestcontainersUtils.GetImageOSAsync.Value.GetAwaiter().GetResult();
        return base.WithImage(image);
    }

    /// <summary>
    /// Set the admin username and password for the container (basic authentication).
    /// </summary>
    /// <param name="username">The admin username.</param>
    /// <param name="password">The admin password.</param>
    /// <returns>A configured instance of <see cref="WireMockContainerBuilder"/></returns>
    public WireMockContainerBuilder WithAdminUserNameAndPassword(string username, string password)
    {
        Guard.NotNull(username);
        Guard.NotNull(password);

        if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(password))
        {
            return this;
        }

        return Merge(DockerResourceConfiguration, new WireMockConfiguration(username, password))
            .WithCommand($"--AdminUserName {username}", $"--AdminPassword {password}");
    }

    /// <summary>
    /// Use the WireMockNullLogger.
    /// </summary>
    /// <returns>A configured instance of <see cref="WireMockContainerBuilder"/></returns>
    [PublicAPI]
    public WireMockContainerBuilder WithNullLogger()
    {
        return WithCommand("--WireMockLogger WireMockNullLogger");
    }

    /// <summary>
    /// Defines if the static mappings should be read at startup (default set to false).
    /// </summary>
    /// <returns>A configured instance of <see cref="WireMockContainerBuilder"/></returns>
    [PublicAPI]
    public WireMockContainerBuilder WithReadStaticMappings()
    {
        return WithCommand("--ReadStaticMappings true");
    }

    /// <summary>
    /// Watch the static mapping files + folder for changes when running.
    /// </summary>
    /// <param name="includeSubDirectories">Also look in SubDirectories.</param>
    /// <returns>A configured instance of <see cref="WireMockContainerBuilder"/></returns>
    [PublicAPI]
    public WireMockContainerBuilder WithWatchStaticMappings(bool includeSubDirectories)
    {
        DockerResourceConfiguration.WithWatchStaticMappings(includeSubDirectories);
        return
            WithCommand("--ReadStaticMappings true").
            WithCommand("--WatchStaticMappings true").
            WithCommand("--WatchStaticMappingsInSubdirectories", includeSubDirectories);
    }

    /// <summary>
    /// Specifies the path for the (static) mapping json files.
    /// </summary>
    /// <param name="path">The path</param>
    /// <param name="includeSubDirectories">Also look in SubDirectories.</param>
    /// <returns>A configured instance of <see cref="WireMockContainerBuilder"/></returns>
    [PublicAPI]
    public WireMockContainerBuilder WithMappings(string path, bool includeSubDirectories = false)
    {
        Guard.NotNullOrEmpty(path);

        DockerResourceConfiguration.WithStaticMappingsPath(path);

        return WithWatchStaticMappings(includeSubDirectories);
    }

    /// <summary>
    /// Use Http version 2.
    /// </summary>
    /// <returns>A configured instance of <see cref="WireMockContainerBuilder"/></returns>
    [PublicAPI]
    public WireMockContainerBuilder WithHttp2()
    {
        return WithCommand("--UseHttp2 true");
    }

    /// <summary>
    /// Adds another URL to the WireMock container. By default, the WireMock container will listen on <c>http://*:80</c>.
    ///
    /// This method can be used to also host the WireMock container on another port or protocol (like grpc).
    /// </summary>
    /// <example>grpc://*:9090</example>
    /// <returns>A configured instance of <see cref="WireMockContainerBuilder"/></returns>
    [PublicAPI]
    public WireMockContainerBuilder AddUrl(string url)
    {
        if (!PortUtils.TryExtract(Guard.NotNullOrEmpty(url), out _, out _, out _, out _, out var port))
        {
            throw new ArgumentException("The URL is not valid.", nameof(url));
        }

        DockerResourceConfiguration.WithAdditionalUrl(url);

        return WithPortBinding(port, true);
    }

    /// <summary>
    /// Add a Grpc ProtoDefinition at server-level.
    /// </summary>
    /// <param name="id">Unique identifier for the ProtoDefinition.</param>
    /// <param name="protoDefinition">The ProtoDefinition as text.</param>
    /// <returns><see cref="WireMockContainerBuilder"/></returns>
    [PublicAPI]
    public WireMockContainerBuilder AddProtoDefinition(string id, params string[] protoDefinition)
    {
        Guard.NotNullOrWhiteSpace(id);
        Guard.NotNullOrEmpty(protoDefinition);

        DockerResourceConfiguration.AddProtoDefinition(id, protoDefinition);

        return this;
    }

    private WireMockContainerBuilder WithCommand(string param, bool value)
    {
        return !value ? this : WithCommand($"{param} true");
    }

    private WireMockContainerBuilder(WireMockConfiguration dockerResourceConfiguration) : base(dockerResourceConfiguration)
    {
        DockerResourceConfiguration = dockerResourceConfiguration;
    }

    /// <inheritdoc />
    protected override WireMockConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override WireMockContainer Build()
    {
        var builder = this;

        // In case no image has been set, set the image using internal logic.
        if (DockerResourceConfiguration.Image == null)
        {
            builder = WithImage();
        }

        // In case the _imageOS is not set, determine it from the Image FullName.
        if (_imageOS == null)
        {
            if (builder.DockerResourceConfiguration.Image.FullName.IndexOf("wiremock.net", StringComparison.OrdinalIgnoreCase) < 0)
            {
                throw new InvalidOperationException();
            }

            _imageOS = builder.DockerResourceConfiguration.Image.FullName.IndexOf("windows", StringComparison.OrdinalIgnoreCase) >= 0 ? OSPlatform.Windows : OSPlatform.Linux;
        }

        if (!string.IsNullOrEmpty(builder.DockerResourceConfiguration.StaticMappingsPath))
        {
            builder = builder.WithBindMount(builder.DockerResourceConfiguration.StaticMappingsPath, ContainerInfoProvider.Info[_imageOS.Value].MappingsPath);
        }

        if (builder.DockerResourceConfiguration.AdditionalUrls.Any())
        {
            builder = builder.WithCommand($"--Urls http://*:80 {string.Join(" ", builder.DockerResourceConfiguration.AdditionalUrls)}");
        }

        builder.Validate();

        return new WireMockContainer(builder.DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override WireMockContainerBuilder Init()
    {
        var builder = base.Init();

        var waitForContainerOS = _imageOS == OSPlatform.Windows ? Wait.ForWindowsContainer() : Wait.ForUnixContainer();
        return builder
            .WithPortBinding(WireMockContainer.ContainerPort, true)
            .WithCommand($"--WireMockLogger {DefaultLogger}")
            .WithWaitStrategy(waitForContainerOS.UntilMessageIsLogged("WireMock.Net server running"));
    }

    /// <inheritdoc />
    protected override WireMockContainerBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new WireMockConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override WireMockContainerBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new WireMockConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override WireMockContainerBuilder Merge(WireMockConfiguration oldValue, WireMockConfiguration newValue)
    {
        return new WireMockContainerBuilder(new WireMockConfiguration(oldValue, newValue));
    }

    private WireMockContainerBuilder WithImage(OSPlatform os)
    {
        _imageOS = os;
        return WithImage(ContainerInfoProvider.Info[os].Image);
    }
}