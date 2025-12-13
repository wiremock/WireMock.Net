// Copyright © WireMock.Net

using System;
using WireMock.Matchers.Helpers;
using WireMock.Models.Mime;
using WireMock.Util;

namespace WireMock.Matchers;

/// <summary>
/// MimePartMatcher
/// </summary>
public class MimePartMatcher : IMimePartMatcher
{
    private readonly Func<IMimePartData, MatchResult>[] _funcs;

    /// <inheritdoc />
    public string Name => nameof(MimePartMatcher);

    /// <inheritdoc />
    public IStringMatcher? ContentTypeMatcher { get; }

    /// <inheritdoc />
    public IStringMatcher? ContentDispositionMatcher { get; }

    /// <inheritdoc />
    public IStringMatcher? ContentTransferEncodingMatcher { get; }

    /// <inheritdoc />
    public IMatcher? ContentMatcher { get; }

    /// <inheritdoc />
    public MatchBehaviour MatchBehaviour { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MimePartMatcher"/> class.
    /// </summary>
    public MimePartMatcher(
        MatchBehaviour matchBehaviour,
        IStringMatcher? contentTypeMatcher,
        IStringMatcher? contentDispositionMatcher,
        IStringMatcher? contentTransferEncodingMatcher,
        IMatcher? contentMatcher
    )
    {
        MatchBehaviour = matchBehaviour;
        ContentTypeMatcher = contentTypeMatcher;
        ContentDispositionMatcher = contentDispositionMatcher;
        ContentTransferEncodingMatcher = contentTransferEncodingMatcher;
        ContentMatcher = contentMatcher;

        _funcs =
        [
            mp => ContentTypeMatcher?.IsMatch(GetContentTypeAsString(mp.ContentType)) ?? MatchScores.Perfect,
            mp => ContentDispositionMatcher?.IsMatch(mp.ContentDisposition?.ToString()?.Replace("Content-Disposition: ", string.Empty)) ?? MatchScores.Perfect,
            mp => ContentTransferEncodingMatcher?.IsMatch(mp.ContentTransferEncoding.ToLowerInvariant()) ?? MatchScores.Perfect,
            MatchOnContent
        ];
    }

    /// <inheritdoc />
    public MatchResult IsMatch(IMimePartData value)
    {
        var score = MatchScores.Mismatch;
        Exception? exception = null;

        try
        {
            if (Array.TrueForAll(_funcs, func => func(value).IsPerfect()))
            {
                score = MatchScores.Perfect;
            }
        }
        catch (Exception ex)
        {
            exception = ex;
        }

        return new MatchResult(MatchBehaviourHelper.Convert(MatchBehaviour, score), exception);
    }

    /// <inheritdoc />
    public string GetCSharpCodeArguments()
    {
        return "NotImplemented";
    }

    private MatchResult MatchOnContent(IMimePartData mimePart)
    {
        if (ContentMatcher == null)
        {
            return MatchScores.Perfect;
        }

        var bodyParserSettings = new BodyParserSettings
        {
            BodyHandling = BodyHandling.TryDeserializeJson | BodyHandling.DecompressGZipAndDeflate,
            Stream = mimePart.Open(),
            ContentType = GetContentTypeAsString(mimePart.ContentType),
            ContentEncoding = null, // mimePart.ContentType?.CharsetEncoding.ToString(),
        };

        var bodyData = BodyParser.ParseAsync(bodyParserSettings).ConfigureAwait(false).GetAwaiter().GetResult();
        return BodyDataMatchScoreCalculator.CalculateMatchScore(bodyData, ContentMatcher);
    }

    private static string? GetContentTypeAsString(IContentTypeData? contentType)
    {
        return contentType?.ToString()?.Replace("Content-Type: ", string.Empty);
    }
}