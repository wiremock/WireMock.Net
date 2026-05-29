// Copyright © WireMock.Net

using JsonConverter.Abstractions;
using JsonConverter.Newtonsoft.Json;

namespace WireMock.Util;

internal class BodyParserSettings
{
    /// <summary>
    /// The body stream to parse.
    /// </summary>
    public Stream Stream { get; set; } = null!;

    /// <summary>
    /// The (optional) content type of the body.
    /// </summary>
    public string? ContentType { get; set; }

    /// <summary>
    /// The (optional) content encoding of the body.
    /// </summary>
    public string? ContentEncoding { get; set; }

    /// <summary>
    /// Automatically decompress GZip and Deflate encoded content.
    /// </summary>
    public bool DecompressGZipAndDeflate { get; set; } = true;

    /// <summary>
    /// Try to deserialize the body as JSON.
    /// </summary>
    public bool DeserializeJson { get; set; } = true;

    /// <summary>
    /// Try to deserialize the body as FormUrlEncoded.
    /// </summary>
    public bool DeserializeFormUrlEncoded { get; set; } = true;

    /// <summary>
    /// Gets or sets the default JSON converter used for deserialization.
    /// </summary>
    /// <remarks>
    /// Set this property to customize how objects are serialized to and deserialized from JSON during mapping.
    /// Default is <see cref="NewtonsoftJsonConverter"/>.
    /// </remarks>
    public IJsonConverter DefaultJsonConverter { get; set; } = new NewtonsoftJsonConverter();
}