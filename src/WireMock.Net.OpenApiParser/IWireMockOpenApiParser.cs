// Copyright © WireMock.Net

using Microsoft.OpenApi.Reader;
using WireMock.Admin.Mappings;
using WireMock.Net.OpenApiParser.Settings;

namespace WireMock.Net.OpenApiParser;

/// <summary>
/// Parse a OpenApi/Swagger/V2/V3 or Raml to WireMock MappingModels.
/// </summary>
public interface IWireMockOpenApiParser
{
    /// <summary>
    /// Generate <see cref="IReadOnlyList{MappingModel}"/> from a file-path.
    /// In case of an error, an empty list is returned and <see cref="OpenApiDiagnostic"/> contains the error details.
    /// </summary>
    /// <param name="path">The path to read the OpenApi/Swagger/V2/V3/V31 or Raml file.</param>
    /// <param name="diagnostic">OpenApiDiagnostic output</param>
    /// <returns>MappingModel</returns>
    IReadOnlyList<MappingModel> FromFile(string path, out OpenApiDiagnostic diagnostic);

    /// <summary>
    /// Generate <see cref="IReadOnlyList{MappingModel}"/> from a file-path.
    /// In case of an error, an empty list is returned and <see cref="OpenApiDiagnostic"/> contains the error details.
    /// </summary>
    /// <param name="path">The path to read the OpenApi/Swagger/V2/V3/V31 or Raml file.</param>
    /// <param name="settings">Additional settings</param>
    /// <param name="diagnostic">OpenApiDiagnostic output</param>
    /// <returns>MappingModel</returns>
    IReadOnlyList<MappingModel> FromFile(string path, WireMockOpenApiParserSettings settings, out OpenApiDiagnostic diagnostic);

    /// <summary>
    /// Generate <see cref="IReadOnlyList{MappingModel}"/> from an Microsoft.OpenApi.Models.OpenApiDocument.
    /// </summary>
    /// <param name="document">The source OpenApiDocument</param>
    /// <param name="settings">Additional settings [optional]</param>
    /// <returns>MappingModel</returns>
    IReadOnlyList<MappingModel> FromDocument(object document, WireMockOpenApiParserSettings? settings = null);

    /// <summary>
    /// Generate <see cref="IReadOnlyList{MappingModel}"/> from a <seealso cref="Stream"/>.
    /// In case of an error, an empty list is returned and <see cref="OpenApiDiagnostic"/> contains the error details.
    /// </summary>
    /// <param name="stream">The source stream</param>
    /// <param name="diagnostic">OpenApiDiagnostic output</param>
    /// <returns>MappingModel</returns>
    IReadOnlyList<MappingModel> FromStream(Stream stream, out OpenApiDiagnostic diagnostic);

    /// <summary>
    /// Generate <see cref="IReadOnlyList{MappingModel}"/> from a <seealso cref="Stream"/>.
    /// In case of an error, an empty list is returned and <see cref="OpenApiDiagnostic"/> contains the error details.
    /// </summary>
    /// <param name="stream">The source stream</param>
    /// <param name="settings">Additional settings</param>
    /// <param name="diagnostic">OpenApiDiagnostic output</param>
    /// <returns>MappingModel</returns>
    IReadOnlyList<MappingModel> FromStream(Stream stream, WireMockOpenApiParserSettings settings, out OpenApiDiagnostic diagnostic);

    /// <summary>
    /// Generate <see cref="IReadOnlyList{MappingModel}"/> from a <seealso cref="string"/>.
    /// In case of an error, an empty list is returned and <see cref="OpenApiDiagnostic"/> contains the error details.
    /// </summary>
    /// <param name="text">The source text</param>
    /// <param name="diagnostic">OpenApiDiagnostic output</param>
    /// <returns>MappingModel</returns>
    IReadOnlyList<MappingModel> FromText(string text, out OpenApiDiagnostic diagnostic);

    /// <summary>
    /// Generate <see cref="IReadOnlyList{MappingModel}"/> from a <seealso cref="string"/>.
    /// In case of an error, an empty list is returned and <see cref="OpenApiDiagnostic"/> contains the error details.
    /// </summary>
    /// <param name="text">The source text</param>
    /// <param name="settings">Additional settings</param>
    /// <param name="diagnostic">OpenApiDiagnostic output</param>
    /// <returns>MappingModel</returns>
    IReadOnlyList<MappingModel> FromText(string text, WireMockOpenApiParserSettings settings, out OpenApiDiagnostic diagnostic);
}