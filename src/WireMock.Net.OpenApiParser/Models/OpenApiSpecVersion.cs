// Copyright © WireMock.Net

namespace WireMock.Net.OpenApiParser.Models;

/// <summary>
/// Represents versions of OpenAPI specification.
/// </summary>
public enum OpenApiSpecVersion
{
    /// <summary>
    /// Represents OpenAPI V2.0 spec
    /// </summary>
    OpenApi2_0,

    /// <summary>
    /// Represents all patches of OpenAPI V3.0 spec (e.g. 3.0.0, 3.0.1)
    /// </summary> 
    OpenApi3_0,

    /// <summary>
    /// Represents OpenAPI V3.1 spec
    /// </summary>
    OpenApi3_1,

    /// <summary>
    /// Represents OpenAPI V3.2 spec
    /// </summary>
    OpenApi3_2
}