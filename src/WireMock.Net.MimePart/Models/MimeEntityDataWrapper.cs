// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using System.Linq;
using MimeKit;
using Stef.Validation;

namespace WireMock.Models;

/// <summary>
/// A wrapper class that implements the <see cref="IMimeEntityData"/>> interface by wrapping an <see cref="IMimeEntity"/> interface.
/// </summary>
/// <remarks>
/// This class provides a simplified, read-only view of an <see cref="IMimeEntity"/>.
/// </remarks>
public class MimeEntityDataWrapper : IMimeEntityData
{
    private readonly IMimeEntity _entity;

    /// <summary>
    /// Initializes a new instance of the <see cref="MimeEntityDataWrapper"/> class.
    /// </summary>
    /// <param name="entity">The MIME entity to wrap.</param>
    public MimeEntityDataWrapper(IMimeEntity entity)
    {
        _entity = Guard.NotNull(entity);

        Headers = _entity.Headers.Select(h => h.ToString()).ToList();
    }

    /// <inheritdoc/>
    public IList<string> Headers { get; private set; }

    /// <inheritdoc/>
    public string ContentDisposition => _entity.ContentDisposition.ToString();

    /// <inheritdoc/>
    public string ContentType => _entity.ContentType.ToString();

    /// <inheritdoc/>
    public Uri ContentBase => _entity.ContentBase;

    /// <inheritdoc/>
    public Uri ContentLocation => _entity.ContentLocation;

    /// <inheritdoc/>
    public string ContentId => _entity.ContentId;

    /// <inheritdoc/>
    public bool IsAttachment => _entity.IsAttachment;
}
