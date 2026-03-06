// Copyright © WireMock.Net

namespace WireMock.Admin.Mappings;

/// <summary>
/// EncodingModel
/// </summary>
[FluentBuilder.AutoGenerateBuilder]
public class EncodingModel
{
    /// <summary>
    /// Encoding CodePage
    /// </summary>
    public int CodePage { get; set; }

    /// <summary>
    /// Encoding EncodingName
    /// </summary>
    public required string EncodingName { get; set; }

    /// <summary>
    /// Encoding WebName
    /// </summary>
    public required string WebName { get; set; }
}