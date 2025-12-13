// Copyright © WireMock.Net

using System.IO;

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

    public BodyHandling BodyHandling { get; set; } =
        BodyHandling.TryDeserializeJson |
        BodyHandling.TryDeserializeFormUrlEncoded |
        BodyHandling.DecompressGZipAndDeflate;
}