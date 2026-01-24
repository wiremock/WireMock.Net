// Copyright © WireMock.Net

using System.Net.Http.Headers;
using AnyOfTypes;
using Stef.Validation;
using WireMock.Extensions;
using WireMock.Models;
using WireMock.Util;

namespace WireMock.Matchers;

/// <summary>
/// ContentTypeMatcher which accepts also all charsets
/// </summary>
/// <seealso cref="RegexMatcher" />
public class ContentTypeMatcher : WildcardMatcher
{
    private readonly AnyOf<string, StringPattern>[] _patterns;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentTypeMatcher"/> class.
    /// </summary>
    /// <param name="pattern">The pattern.</param>
    /// <param name="ignoreCase">IgnoreCase (default false)</param>
    public ContentTypeMatcher(AnyOf<string, StringPattern> pattern, bool ignoreCase = false) : this([pattern], ignoreCase)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentTypeMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="pattern">The pattern.</param>
    /// <param name="ignoreCase">IgnoreCase (default false)</param>
    public ContentTypeMatcher(MatchBehaviour matchBehaviour, AnyOf<string, StringPattern> pattern, bool ignoreCase = false) : this(matchBehaviour,
        [pattern], ignoreCase)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentTypeMatcher"/> class.
    /// </summary>
    /// <param name="patterns">The patterns.</param>
    /// <param name="ignoreCase">IgnoreCase (default false)</param>
    public ContentTypeMatcher(AnyOf<string, StringPattern>[] patterns, bool ignoreCase = false) : this(MatchBehaviour.AcceptOnMatch, patterns, ignoreCase)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentTypeMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="patterns">The patterns.</param>
    /// <param name="ignoreCase">IgnoreCase (default false)</param>
    public ContentTypeMatcher(MatchBehaviour matchBehaviour, AnyOf<string, StringPattern>[] patterns, bool ignoreCase = false) : base(matchBehaviour, patterns, ignoreCase)
    {
        _patterns = Guard.NotNull(patterns);
    }

    /// <inheritdoc />
    public override MatchResult IsMatch(string? input)
    {
        if (string.IsNullOrEmpty(input) || !MediaTypeHeaderValue.TryParse(input, out var contentType))
        {
            return MatchResult.From(Name, MatchBehaviourHelper.Convert(MatchBehaviour, MatchScores.Mismatch));
        }

        return base.IsMatch(contentType.MediaType);
    }

    /// <inheritdoc />
    public override AnyOf<string, StringPattern>[] GetPatterns()
    {
        return _patterns;
    }

    /// <inheritdoc />
    public override string Name => nameof(ContentTypeMatcher);

    /// <inheritdoc />
    public override string GetCSharpCodeArguments()
    {
        return $"new {Name}" +
               $"(" +
               $"{MatchBehaviour.GetFullyQualifiedEnumValue()}, " +
               $"{MappingConverterUtils.ToCSharpCodeArguments(_patterns)}, " +
               $"{CSharpFormatter.ToCSharpBooleanLiteral(IgnoreCase)}" +
               $")";
    }
}