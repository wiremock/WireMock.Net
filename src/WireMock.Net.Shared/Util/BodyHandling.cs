// Copyright © WireMock.Net

using System;

namespace WireMock.Util;

[Flags]
internal enum BodyHandling
{
    /// <summary>
    /// No special handling. Keep the body as is.
    /// </summary>
    None = 0x00,

    /// <summary>
    /// Try to deserialize the body as JSON.
    /// </summary>
    TryDeserializeJson = 0x01,

    /// <summary>
    /// Try to deserialize the body as FormUrlEncoded.
    /// </summary>
    TryDeserializeFormUrlEncoded = 0x02,

    /// <summary>
    /// Automatically decompress GZip and Deflate encoded content.
    /// </summary>
    DecompressGZipAndDeflate = 0x04
}