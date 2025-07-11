// Copyright © WireMock.Net

using System;
using MimeKit;
using Stef.Validation;
using WireMock.Models.Mime;

namespace WireMock.Models;

/// <summary>
/// A wrapper class that implements the IContentDispositionData interface
/// by wrapping a ContentDisposition object.
/// </summary>
/// <remarks>
/// This class provides a simplified, read-only view of a ContentDisposition.
/// </remarks>
public class ContentDispositionDataWrapper : IContentDispositionData
{
    private readonly ContentDisposition _contentDisposition;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentDispositionDataWrapper"/> class.
    /// </summary>
    /// <param name="contentDisposition">The ContentDisposition to wrap.</param>
    public ContentDispositionDataWrapper(ContentDisposition contentDisposition)
    {
        _contentDisposition = Guard.NotNull(contentDisposition);
    }

    /// <inheritdoc/>
    public string Disposition => _contentDisposition.Disposition;

    /// <inheritdoc/>
    public bool IsAttachment => _contentDisposition.IsAttachment;

    /// <inheritdoc/>
    public object Parameters => _contentDisposition.Parameters;

    /// <inheritdoc/>
    public string FileName => _contentDisposition.FileName;

    /// <inheritdoc/>
    public DateTimeOffset? CreationDate => _contentDisposition.CreationDate;

    /// <inheritdoc/>
    public DateTimeOffset? ModificationDate => _contentDisposition.ModificationDate;

    /// <inheritdoc/>
    public DateTimeOffset? ReadDate => _contentDisposition.ReadDate;

    /// <inheritdoc/>
    public long? Size => _contentDisposition.Size;

    /// <inheritdoc/>
    public override string ToString()
    {
        return _contentDisposition.ToString();
    }
}
