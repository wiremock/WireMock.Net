// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using WireMock.Matchers.Helpers;
using WireMock.Models.Mime;
using WireMock.Util;

namespace WireMock.Matchers;

/// <summary>
/// MimePartMatcher
/// </summary>
public class MimePartMatcher : IMimePartMatcher
{
    private readonly IList<(string Name, Func<IMimePartData, MatchResult> func)> _matcherFunctions;

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

        _matcherFunctions = [];
        if (ContentTypeMatcher != null)
        {
            _matcherFunctions.Add((nameof(ContentTypeMatcher), mp => ContentTypeMatcher.IsMatch(GetContentTypeAsString(mp.ContentType))));
        }

        if (ContentDispositionMatcher != null)
        {
            _matcherFunctions.Add((nameof(ContentDispositionMatcher), mp => ContentDispositionMatcher.IsMatch(mp.ContentDisposition?.ToString()?.Replace("Content-Disposition: ", string.Empty))));
        }

        if (ContentTransferEncodingMatcher != null)
        {
            _matcherFunctions.Add((nameof(ContentTransferEncodingMatcher), mp => ContentTransferEncodingMatcher.IsMatch(mp.ContentTransferEncoding.ToLowerInvariant())));
        }

        if (ContentMatcher != null)
        {
            _matcherFunctions.Add((ContentMatcher.Name, MatchOnContent));
        }
    }

    /// <inheritdoc />
    public MatchResult IsMatch(IMimePartData value)
    {
        var results = new List<MatchResult>();

        foreach (var matcherFunction in _matcherFunctions)
        {
            try
            {
                var matchResult = matcherFunction.func(value);
                results.Add(MatchResult.From(matcherFunction.Name, matchResult.Score));
            }
            catch (Exception ex)
            {
                results.Add(MatchResult.From(matcherFunction.Name, MatchScores.Mismatch, ex));
            }
        }

        return MatchResult.From(nameof(MimePartMatcher), results, MatchOperator.And);
    }

    /// <inheritdoc />
    public string GetCSharpCodeArguments()
    {
        return "NotImplemented";
    }

    private MatchResult MatchOnContent(IMimePartData mimePart)
    {
        var bodyParserSettings = new BodyParserSettings
        {
            Stream = mimePart.Open(),
            ContentType = GetContentTypeAsString(mimePart.ContentType),
            DeserializeJson = true,
            ContentEncoding = null, // mimePart.ContentType?.CharsetEncoding.ToString(),
            DecompressGZipAndDeflate = true
        };

        var bodyData = BodyParser.ParseAsync(bodyParserSettings).ConfigureAwait(false).GetAwaiter().GetResult();
        return BodyDataMatchScoreCalculator.CalculateMatchScore(bodyData, ContentMatcher!);
    }

    private static string? GetContentTypeAsString(IContentTypeData? contentType)
    {
        return contentType?.ToString()?.Replace("Content-Type: ", string.Empty);
    }
}