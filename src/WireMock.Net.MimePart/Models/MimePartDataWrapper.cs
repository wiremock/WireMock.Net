// Copyright © WireMock.Net

using System.Collections.Generic;
using System.IO;
using MimeKit;
using Stef.Validation;

namespace WireMock.Models;

/// <summary>
/// A wrapper class that implements the <see cref="IMimePartData"/>> interface by wrapping an <see cref="IMimePart"/> interface.
/// </summary>
/// <remarks>
/// This class provides a simplified, read-only view of an <see cref="IMimePart"/>.
/// </remarks>
public class MimePartDataWrapper : MimeEntityDataWrapper, IMimePartData
{
    private readonly IMimePart _part;

    /// <summary>
    /// Initializes a new instance of the <see cref="MimePartDataWrapper"/> class.
    /// </summary>
    /// <param name="part">The MIME part to wrap.</param>
    /// <exception cref="System.ArgumentNullException">
    /// <paramref name="part"/> is <see langword="null"/>.
    /// </exception>
    public MimePartDataWrapper(IMimePart part) : base(part)
    {
        _part = Guard.NotNull(part);
    }

    /// <inheritdoc/>
    public string ContentDescription => _part.ContentDescription;

    /// <inheritdoc/>
    public int? ContentDuration => _part.ContentDuration;

    /// <inheritdoc/>
    public string ContentMd5 => _part.ContentMd5;

    /// <inheritdoc/>
    public int ContentTransferEncoding => (int)_part.ContentTransferEncoding;

    /// <inheritdoc/>
    public string FileName => _part.FileName;

    /// <inheritdoc/>
    public IDictionary<string, object?> Content => new Dictionary<string, object?>()
    {
        { nameof(MimePart.Content.Encoding),  _part.Content.Encoding },
        { nameof(MimePart.Content.NewLineFormat),  _part.Content.NewLineFormat },
        { nameof(MimePart.Content.Stream),  _part.Content.Stream }
    };

    /// <inheritdoc/>
    public Stream Open() => _part.Content.Open();
}