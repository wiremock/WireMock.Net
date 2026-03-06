// Copyright © WireMock.Net

using Stef.Validation;

namespace WireMock.Models;

/// <summary>
/// UrlDetails
/// </summary>
public class UrlDetails
{
    /// <summary>
    /// Gets the url (relative).
    /// </summary>
    public Uri Url { get; }

    /// <summary>
    /// Gets the AbsoluteUrl.
    /// </summary>
    public Uri AbsoluteUrl { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UrlDetails"/> class.
    /// </summary>
    /// <param name="url">The URL.</param>
    public UrlDetails(string url) : this(new Uri(url))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UrlDetails"/> class.
    /// </summary>
    /// <param name="url">The URL.</param>
    public UrlDetails(Uri url) : this(url, url)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UrlDetails"/> class.
    /// </summary>
    /// <param name="absoluteUrl">The absolute URL.</param>
    /// <param name="url">The URL (relative).</param>
    public UrlDetails(Uri absoluteUrl, Uri url)
    {
        AbsoluteUrl = Guard.NotNull(absoluteUrl);
        Url = Guard.NotNull(url);
    }
}