// Copyright © WireMock.Net

using System.Diagnostics.CodeAnalysis;
using WireMock.Client.Builders;

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
    /// The HTTP port where WireMock.Net is listening.
    /// If not defined, .NET Aspire automatically assigns a random port.
    /// </summary>
    public int? HttpPort { get; set; }

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
    /// Use HTTP 2 (used for Grpc).
    /// </summary>
    public bool UseHttp2 { get; set; }

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

        if (UseHttp2)
        {
            Add(args, "--UseHttp2", "true");
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