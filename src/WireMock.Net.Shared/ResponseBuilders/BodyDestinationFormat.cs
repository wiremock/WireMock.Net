// Copyright © WireMock.Net

namespace WireMock.ResponseBuilders;

/// <summary>
/// Defines the BodyDestinationFormat
/// </summary>
public static class BodyDestinationFormat
{
    /// <summary>
    /// Same as source (no conversion)
    /// </summary>
    public const string SameAsSource = "SameAsSource";

    /// <summary>
    /// Convert to string
    /// </summary>
    public const string String = "String";

    /// <summary>
    /// Convert to bytes
    /// </summary>
    public const string Bytes = "Bytes";

    /// <summary>
    /// Convert to Json object
    /// </summary>
    public const string Json = "Json";
}