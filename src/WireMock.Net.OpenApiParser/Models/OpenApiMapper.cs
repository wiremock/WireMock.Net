// Copyright © WireMock.Net

using MicrosoftOpenApiDiagnostic = Microsoft.OpenApi.Reader.OpenApiDiagnostic;

namespace WireMock.Net.OpenApiParser.Models;

internal static class OpenApiMapper
{
    internal static OpenApiDiagnostic? Map(MicrosoftOpenApiDiagnostic? openApiDiagnostic)
    {
        if (openApiDiagnostic == null)
        {
            return null;
        }

        return new OpenApiDiagnostic
        {
            Errors = openApiDiagnostic.Errors.Select(e => new OpenApiError(e.Pointer, e.Message)).ToList(),
            Warnings = openApiDiagnostic.Warnings.Select(e => new OpenApiError(e.Pointer, e.Message)).ToList(),
            SpecificationVersion = (OpenApiSpecVersion)openApiDiagnostic.SpecificationVersion
        };
    }
}