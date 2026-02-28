// Copyright © WireMock.Net

namespace WireMock.Models.Mime;

/// <summary>
/// A simplified interface exposing the public, readable properties of MimeEntity.
/// </summary>
public interface IMimeEntityData
{
    /// <summary>
    /// Get the list of headers.
    /// </summary>
    /// <value>The list of headers.</value>
    IList<string> Headers { get; }

    /// <summary>
    /// Get the content disposition.
    /// </summary>
    /// <value>The content disposition.</value>
    IContentDispositionData? ContentDisposition { get; }

    /// <summary>
    /// Get the type of the content.
    /// </summary>
    /// <value>The type of the content.</value>
    IContentTypeData? ContentType { get; }

    /// <summary>
    /// Get the base content URI.
    /// </summary>
    /// <value>The base content URI or <see langword="null"/>.</value>
    Uri ContentBase { get; }

    /// <summary>
    /// Get the content location.
    /// </summary>
    /// <value>The content location or <see langword="null"/>.</value>
    Uri ContentLocation { get; }

    /// <summary>
    /// Get the Content-Id.
    /// </summary>
    /// <value>The content identifier.</value>
    string ContentId { get; }

    /// <summary>
    /// Get a value indicating whether this <see cref="IMimeEntityData"/> is an attachment.
    /// </summary>
    /// <value><see langword="true" /> if this <see cref="IMimeEntityData"/> is an attachment; otherwise, <see langword="false" />.</value>
    bool IsAttachment { get; }
}