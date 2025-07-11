// Copyright © WireMock.Net

using System.Collections.Generic;
using System.Text;

namespace WireMock.Models.Mime;

/// <summary>
/// An interface exposing the public, readable properties of a ContentType
/// with complex types simplified to a generic object.
/// </summary>
public interface IContentTypeData
{
    /// <summary>
    /// Get the type of the media.
    /// </summary>
    /// <value>The type of the media.</value>
    string MediaType { get; }

    /// <summary>
    /// Get the media subtype.
    /// </summary>
    /// <value>The media subtype.</value>
    string MediaSubtype { get; }

    /// <summary>
    /// Get the list of parameters on the ContentType.
    /// </summary>
    /// <value>The parameters.</value>
    IList<string> Parameters { get; }

    /// <summary>
    /// Get the boundary parameter.
    /// </summary>
    /// <value>The boundary.</value>
    string Boundary { get; }

    /// <summary>
    /// Get the charset parameter.
    /// </summary>
    /// <value>The charset.</value>
    string Charset { get; }

    /// <summary>
    /// Get the charset parameter as an Encoding.
    /// </summary>
    /// <value>The charset encoding.</value>
    Encoding CharsetEncoding { get; }

    /// <summary>
    /// Get the format parameter.
    /// </summary>
    /// <value>The format.</value>
    string Format { get; }

    /// <summary>
    /// Get the simple mime-type.
    /// </summary>
    /// <value>The mime-type.</value>
    string MimeType { get; }

    /// <summary>
    /// Get the name parameter.
    /// </summary>
    /// <value>The name.</value>
    string Name { get; }
}
