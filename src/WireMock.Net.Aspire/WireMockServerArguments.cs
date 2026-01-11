// Copyright © WireMock.Net

using System.Diagnostics.CodeAnalysis;
using Stef.Validation;
using WireMock.Client.Builders;
using WireMock.Util;

// ReSharper disable once CheckNamespace
namespace Aspire.Hosting;

/// <summary>
/// Represents the arguments required to configure and start a WireMock.Net Server.
/// </summary>
public class WireMockServerArguments
{
    internal const int HttpContainerPort = 80;

    /// <summary>
    /// The default HTTP port where WireMock.Net is listening.
    /// </summary>
    public const int DefaultPort = 9091;

    private const string DefaultLogger = "WireMockConsoleLogger";

    /// <summary>
    /// The HTTP ports where WireMock.Net is listening on.
    /// If not defined, .NET Aspire automatically assigns a random port.
    /// </summary>
    public List<int> HttpPorts { get; set; } = [];

    /// <summary>
    /// Additional Urls on which WireMock listens.
    /// </summary>
    public List<string> AdditionalUrls { get; set; } = [];

    /// <summary>
    /// The admin username.
    /// </summary>
    [MemberNotNullWhen(true, nameof(HasBasicAuthentication))]
    public string? AdminUsername { get; set; }

    /// <summary>
    /// The admin password.
    /// </summary>
    [MemberNotNullWhen(true, nameof(HasBasicAuthentication))]
    public string? AdminPassword { get; set; }

    /// <summary>
    /// Defines if the static mappings should be read at startup.
    ///
    /// Default value is <c>false</c>.
    /// </summary>
    public bool ReadStaticMappings { get; set; }

    /// <summary>
    /// Watch the static mapping files + folder for changes when running.
    ///
    /// Default value is <c>false</c>.
    /// </summary>
    public bool WatchStaticMappings { get; set; }

    /// <summary>
    /// Specifies the path for the (static) mapping json files.
    /// </summary>
    public string? MappingsPath { get; set; }

    /// <summary>
    /// Indicates whether the admin interface has Basic Authentication.
    /// </summary>
    public bool HasBasicAuthentication => !string.IsNullOrEmpty(AdminUsername) && !string.IsNullOrEmpty(AdminPassword);

    /// <summary>
    /// Optional delegate that will be invoked to configure the WireMock.Net resource using the <see cref="AdminApiMappingBuilder"/>.
    /// </summary>
    public Func<AdminApiMappingBuilder, CancellationToken, Task>? ApiMappingBuilder { get; set; }

    /// <summary>
    /// Grpc ProtoDefinitions.
    /// </summary>
    public Dictionary<string, string[]> ProtoDefinitions { get; set; } = [];

    /// <summary>
    /// Gets or sets a value indicating whether OpenTelemetry tracing is enabled.
    /// When enabled, WireMock.Net will emit distributed traces for request processing.
    /// Default value is <c>false</c>.
    /// </summary>
    public bool OpenTelemetryEnabled { get; set; }

    /// <summary>
    /// Gets or sets the OTLP exporter endpoint URL.
    /// When set, traces will be exported to this endpoint using the OTLP protocol.
    /// Example: "http://localhost:4317" for gRPC or "http://localhost:4318" for HTTP.
    /// If not set, the OTLP exporter will use the <c>OTEL_EXPORTER_OTLP_ENDPOINT</c> environment variable,
    /// or fall back to the default endpoint (<c>http://localhost:4317</c> for gRPC).
    /// </summary>
    public string? OpenTelemetryOtlpExporterEndpoint { get; set; }

    /// <summary>
    /// Add an additional Urls on which WireMock should listen.
    /// </summary>
    /// <param name="additionalUrls">The additional urls which the WireMock Server should listen on.</param>
    public void WithAdditionalUrls(params string[] additionalUrls)
    {
        foreach (var url in additionalUrls)
        {
            if (!PortUtils.TryExtract(Guard.NotNullOrEmpty(url), out _, out _, out _, out _, out var port))
            {
                throw new ArgumentException($"The URL '{url}' is not valid.");
            }

            AdditionalUrls.Add(Guard.NotNullOrWhiteSpace(url));
            HttpPorts.Add(port);
        }
    }

    /// <summary>
    /// Add a Grpc ProtoDefinition at server-level.
    /// </summary>
    /// <param name="id">Unique identifier for the ProtoDefinition.</param>
    /// <param name="protoDefinitions">The ProtoDefinition as text.</param>
    public void WithProtoDefinition(string id, params string[] protoDefinitions)
    {
        Guard.NotNullOrWhiteSpace(id);
        Guard.NotNullOrEmpty(protoDefinitions);

        ProtoDefinitions[id] = protoDefinitions;
    }

    /// <summary>
    /// Converts the current instance's properties to an array of command-line arguments for starting the WireMock.Net server.
    /// </summary>
    /// <returns>An array of strings representing the command-line arguments.</returns>
    public string[] GetArgs()
    {
        var args = new Dictionary<string, string>();

        Add(args, "--WireMockLogger", DefaultLogger);

        if (HasBasicAuthentication)
        {
            Add(args, "--AdminUserName", AdminUsername!);
            Add(args, "--AdminPassword", AdminPassword!);
        }

        if (ReadStaticMappings)
        {
            Add(args, "--ReadStaticMappings", "true");
        }

        if (WatchStaticMappings)
        {
            Add(args, "--ReadStaticMappings", "true");
            Add(args, "--WatchStaticMappings", "true");
            Add(args, "--WatchStaticMappingsInSubdirectories", "true");
        }

        if (OpenTelemetryEnabled)
        {
            Add(args, "--OpenTelemetryEnabled", "true");
            
            if (!string.IsNullOrEmpty(OpenTelemetryOtlpExporterEndpoint))
            {
                Add(args, "--OpenTelemetryOtlpExporterEndpoint", OpenTelemetryOtlpExporterEndpoint);
            }
        }

        if (AdditionalUrls.Count > 0)
        {
            Add(args, "--Urls", $"http://*:{HttpContainerPort} {string.Join(' ', AdditionalUrls)}");
        }

        return args
            .SelectMany(k => new[] { k.Key, k.Value })
            .ToArray();
    }

    private static void Add(IDictionary<string, string> args, string argument, string value)
    {
        args[argument] = value;
    }

    private static void Add(IDictionary<string, string> args, string argument, Func<string> action)
    {
        args[argument] = action();
    }
}