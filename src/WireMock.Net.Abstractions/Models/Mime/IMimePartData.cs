// Copyright © WireMock.Net

namespace WireMock.Models.Mime;

/// <summary>
/// A simplified interface exposing the public, readable properties of MimePart.
/// </summary>
public interface IMimePartData : IMimeEntityData
{
    /// <summary>
    /// Get the description of the content if available.
    /// </summary>
    /// <value>The description of the content.</value>
    string ContentDescription { get; }

    /// <summary>
    /// Get the duration of the content if available.
    /// </summary>
    /// <value>The duration of the content.</value>
    int? ContentDuration { get; }

    /// <summary>
    /// Get the md5sum of the content.
    /// </summary>
    /// <value>The md5sum of the content.</value>
    string ContentMd5 { get; }

    /// <summary>
    /// Get the content transfer encoding.
    /// </summary>
    /// <value>The content transfer encoding as a string.</value>
    string ContentTransferEncoding { get; }

    /// <summary>
    /// Get the name of the file.
    /// </summary>
    /// <value>The name of the file.</value>
    string FileName { get; }

    /// <summary>
    /// Get the MIME content.
    /// </summary>
    /// <value>The MIME content.</value>
    IDictionary<string, object?> Content { get; }

    /// <summary>
    /// Open the decoded content stream.
    /// </summary>
    /// <remarks>
    /// Provides a means of reading the decoded content without having to first write it to another stream.
    /// </remarks>
    /// <returns>The decoded content stream.</returns>
    Stream Open();
}