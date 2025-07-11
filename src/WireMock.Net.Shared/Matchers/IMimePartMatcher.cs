// Copyright © WireMock.Net

using WireMock.Models.Mime;

namespace WireMock.Matchers;

/// <summary>
/// MimePartMatcher
/// </summary>
/// <inheritdoc cref="IMatcher"/>
public interface IMimePartMatcher : IMatcher
{
    /// <summary>
    /// ContentType Matcher (image/png; name=image.png.)
    /// </summary>
    IStringMatcher? ContentTypeMatcher { get; }

    /// <summary>
    /// ContentDisposition Matcher (attachment; filename=image.png)
    /// </summary>
    IStringMatcher? ContentDispositionMatcher { get; }

    /// <summary>
    /// ContentTransferEncoding Matcher (base64)
    /// </summary>
    IStringMatcher? ContentTransferEncodingMatcher { get; }

    /// <summary>
    /// Content Matcher
    /// </summary>
    IMatcher? ContentMatcher { get; }

    /// <summary>
    /// Determines whether the specified MimePart is match.
    /// </summary>
    /// <param name="value">The MimePart.</param>
    /// <returns>A value between 0.0 - 1.0 of the similarity.</returns>
    public MatchResult IsMatch(IMimePartData value);
}