// Copyright © WireMock.Net

using JetBrains.Annotations;
using Microsoft.OpenApi.Reader;
using Stef.Validation;
using WireMock.Net.OpenApiParser.Settings;
using WireMock.Server;

namespace WireMock.Net.OpenApiParser.Extensions;

/// <summary>
/// Some extension methods for <see cref="IWireMockServer"/>.
/// </summary>
public static class WireMockServerExtensions
{
    /// <summary>
    /// Register the mappings via an OpenAPI (swagger) V2/V3/V3.1/V3.2 file.
    /// In case of an error, no mappings are registered and <see cref="OpenApiDiagnostic"/> contains the error details.
    /// </summary>
    /// <param name="server">The WireMockServer instance</param>
    /// <param name="path">Path containing OpenAPI file to parse and use the mappings.</param>
    /// <param name="diagnostic">Returns diagnostic object containing errors detected during parsing</param>
    [PublicAPI]
    public static IWireMockServer WithMappingFromOpenApiFile(this IWireMockServer server, string path, out OpenApiDiagnostic diagnostic)
    {
        return WithMappingFromOpenApiFile(server, path, new WireMockOpenApiParserSettings(), out diagnostic);
    }

    /// <summary>
    /// Register the mappings via an OpenAPI (swagger) V2/V3/V3.1/V3.2 file.
    /// In case of an error, no mappings are registered and <see cref="OpenApiDiagnostic"/> contains the error details.
    /// </summary>
    /// <param name="server">The WireMockServer instance</param>
    /// <param name="path">Path containing OpenAPI file to parse and use the mappings.</param>
    /// <param name="settings">Additional settings</param>
    /// <param name="diagnostic">Returns diagnostic object containing errors detected during parsing</param>
    [PublicAPI]
    public static IWireMockServer WithMappingFromOpenApiFile(this IWireMockServer server, string path, WireMockOpenApiParserSettings settings, out OpenApiDiagnostic diagnostic)
    {
        Guard.NotNull(server);
        Guard.NotNullOrEmpty(path);

        var mappings = new WireMockOpenApiParser().FromFile(path, settings, out diagnostic);

        return server.WithMapping(mappings.ToArray());
    }

    /// <summary>
    /// Register the mappings via an OpenAPI (swagger) V2/V3/V3.1/V3.2 stream.
    /// In case of an error, no mappings are registered and <see cref="OpenApiDiagnostic"/> contains the error details.
    /// </summary>
    /// <param name="server">The WireMockServer instance</param>
    /// <param name="stream">Stream containing OpenAPI description to parse and use the mappings.</param>
    /// <param name="diagnostic">Returns diagnostic object containing errors detected during parsing</param>
    [PublicAPI]
    public static IWireMockServer WithMappingFromOpenApiStream(this IWireMockServer server, Stream stream, out OpenApiDiagnostic diagnostic)
    {
        return WithMappingFromOpenApiStream(server, stream, new WireMockOpenApiParserSettings(), out diagnostic);
    }

    /// <summary>
    /// Register the mappings via an OpenAPI (swagger) V2/V3/V3.1/V3.2 stream.
    /// In case of an error, no mappings are registered and <see cref="OpenApiDiagnostic"/> contains the error details.
    /// </summary>
    /// <param name="server">The WireMockServer instance</param>
    /// <param name="stream">Stream containing OpenAPI description to parse and use the mappings.</param>
    /// <param name="settings">Additional settings</param>
    /// <param name="diagnostic">Returns diagnostic object containing errors detected during parsing</param>
    [PublicAPI]
    public static IWireMockServer WithMappingFromOpenApiStream(this IWireMockServer server, Stream stream, WireMockOpenApiParserSettings settings, out OpenApiDiagnostic diagnostic)
    {
        Guard.NotNull(server);
        Guard.NotNull(stream);
        Guard.NotNull(settings);

        var mappings = new WireMockOpenApiParser().FromStream(stream, settings, out diagnostic);

        return server.WithMapping(mappings.ToArray());
    }

    /// <summary>
    /// Register the mappings via an OpenAPI (swagger) V2/V3/V3.1/V3.2 document.
    /// In case of an error, no mappings are registered and <see cref="OpenApiDiagnostic"/> contains the error details.
    /// </summary>
    /// <param name="server">The WireMockServer instance</param>
    /// <param name="document">The OpenAPI document to use as mappings.</param>
    /// <param name="settings">Additional settings [optional].</param>
    [PublicAPI]
    public static IWireMockServer WithMappingFromOpenApiDocument(this IWireMockServer server, object document, WireMockOpenApiParserSettings? settings = null)
    {
        Guard.NotNull(server);
        Guard.NotNull(document);

        var mappings = new WireMockOpenApiParser().FromDocument(document, settings);

        return server.WithMapping(mappings.ToArray());
    }
}