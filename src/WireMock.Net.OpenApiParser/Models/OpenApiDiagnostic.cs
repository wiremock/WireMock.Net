// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace WireMock.Net.OpenApiParser.Models;

/// <summary>
/// Object containing all diagnostic information related to Open API parsing.
/// </summary>
public class OpenApiDiagnostic
{
    /// <summary>
    /// List of all errors.
    /// </summary>
    public List<OpenApiError> Errors { get; set; } = [];

    /// <summary>
    /// List of all warnings
    /// </summary>
    public List<OpenApiError> Warnings { get; set; } = [];

    /// <summary>
    /// Open API specification version of the document parsed.
    /// </summary>
    public OpenApiSpecVersion SpecificationVersion { get; set; }
}