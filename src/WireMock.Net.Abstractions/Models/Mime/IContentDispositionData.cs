// Copyright © WireMock.Net

using System;
using System.Collections.Generic;

namespace WireMock.Models.Mime;

/// <summary>
/// An interface exposing the public, readable properties of a ContentDisposition.
/// </summary>
public interface IContentDispositionData
{
    /// <summary>
    /// Get the disposition.
    /// </summary>
    /// <value>The disposition.</value>
    string Disposition { get; }

    /// <summary>
    /// Get a value indicating whether the <see cref="IMimeEntityData"/> is an attachment.
    /// </summary>
    /// <value><see langword="true" /> if the <see cref="IMimeEntityData"/> is an attachment; otherwise, <see langword="false" />.</value>
    bool IsAttachment { get; }

    /// <summary>
    /// Get the list of parameters on the ContentDisposition.
    /// </summary>
    /// <value>The parameters.</value>
    public IList<string> Parameters { get; }

    /// <summary>
    /// Get the name of the file.
    /// </summary>
    /// <value>The name of the file.</value>
    string FileName { get; }

    /// <summary>
    /// Get the creation-date parameter.
    /// </summary>
    /// <value>The creation date.</value>
    DateTimeOffset? CreationDate { get; }

    /// <summary>
    /// Get the modification-date parameter.
    /// </summary>
    /// <value>The modification date.</value>
    DateTimeOffset? ModificationDate { get; }

    /// <summary>
    /// Get the read-date parameter.
    /// </summary>
    /// <value>The read date.</value>
    DateTimeOffset? ReadDate { get; }

    /// <summary>
    /// Get the size parameter.
    /// </summary>
    /// <value>The size.</value>
    long? Size { get; }
}