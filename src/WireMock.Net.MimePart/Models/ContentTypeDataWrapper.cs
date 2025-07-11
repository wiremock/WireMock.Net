// Copyright © WireMock.Net

using System.Collections.Generic;
using System.Linq;
using System.Text;
using MimeKit;
using Stef.Validation;
using WireMock.Models.Mime;

namespace WireMock.Models;

/// <summary>
/// A wrapper class that implements the <see cref="IContentTypeData"/> interface by wrapping a <see cref="ContentType"/> object.
/// </summary>
/// <remarks>
/// This class provides a simplified, read-only view of a <see cref="ContentType"/>.
/// </remarks>
public class ContentTypeDataWrapper : IContentTypeData
{
    private readonly ContentType _contentType;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentTypeDataWrapper"/> class.
    /// </summary>
    /// <param name="contentType">The ContentType to wrap.</param>
    public ContentTypeDataWrapper(ContentType contentType)
    {
        _contentType = Guard.NotNull(contentType);

        Parameters = _contentType.Parameters.Select(p => p.ToString()).ToList();
    }

    /// <inheritdoc/>
    public string MediaType => _contentType.MediaType;

    /// <inheritdoc/>
    public string MediaSubtype => _contentType.MediaSubtype;

    /// <inheritdoc/>
    public IList<string> Parameters { get; private set; }

    /// <inheritdoc/>
    public string Boundary => _contentType.Boundary;

    /// <inheritdoc/>
    public string Charset => _contentType.Charset;

    /// <inheritdoc/>
    public Encoding CharsetEncoding => _contentType.CharsetEncoding;

    /// <inheritdoc/>
    public string Format => _contentType.Format;

    /// <inheritdoc/>
    public string MimeType => _contentType.MimeType;

    /// <inheritdoc/>
    public string Name => _contentType.Name;

    /// <inheritdoc/>
    public override string ToString()
    {
        return _contentType.ToString();
    }
}