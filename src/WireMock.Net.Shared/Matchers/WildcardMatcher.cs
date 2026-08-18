// Copyright © WireMock.Net

using System.Text.RegularExpressions;
using AnyOfTypes;
using Stef.Validation;
using WireMock.Extensions;
using WireMock.Models;
using WireMock.Util;

namespace WireMock.Matchers;

/// <summary>
/// WildcardMatcher
/// </summary>
/// <seealso cref="RegexMatcher" />
public class WildcardMatcher : RegexMatcher
{
    private readonly AnyOf<string, StringPattern>[] _patterns;

    /// <summary>
    /// Initializes a new instance of the <see cref="WildcardMatcher"/> class.
    /// </summary>
    /// <param name="pattern">The pattern.</param>
    /// <param name="ignoreCase">IgnoreCase</param>
    public WildcardMatcher(AnyOf<string, StringPattern> pattern, bool ignoreCase = false) : this(new[] { pattern }, ignoreCase)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WildcardMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="pattern">The pattern.</param>
    /// <param name="ignoreCase">IgnoreCase</param>
    public WildcardMatcher(MatchBehaviour matchBehaviour, AnyOf<string, StringPattern> pattern, bool ignoreCase = false) : this(matchBehaviour, new[] { pattern }, ignoreCase)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WildcardMatcher"/> class.
    /// </summary>
    /// <param name="patterns">The patterns.</param>
    /// <param name="ignoreCase">IgnoreCase</param>
    public WildcardMatcher(AnyOf<string, StringPattern>[] patterns, bool ignoreCase = false) : this(MatchBehaviour.AcceptOnMatch, patterns, ignoreCase)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WildcardMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="patterns">The patterns.</param>
    /// <param name="ignoreCase">IgnoreCase</param>
    /// <param name="matchOperator">The <see cref="MatchOperator"/> to use. (default = "Or")</param>
    public WildcardMatcher(
        MatchBehaviour matchBehaviour,
        AnyOf<string, StringPattern>[] patterns,
        bool ignoreCase = false,
        MatchOperator matchOperator = MatchOperator.Or) : base(matchBehaviour, CreateArray(patterns), ignoreCase, true, matchOperator)
    {
        _patterns = Guard.NotNull(patterns);
    }

    /// <inheritdoc />
    public override AnyOf<string, StringPattern>[] GetPatterns()
    {
        return _patterns;
    }

    /// <inheritdoc />
    public override string Name => nameof(WildcardMatcher);

    /// <inheritdoc />
    public override string GetCSharpCodeArguments()
    {
        return $"new {Name}" +
               $"(" +
               $"{MatchBehaviour.GetFullyQualifiedEnumValue()}, " +
               $"new AnyOf<string, StringPattern>[] {{ {MappingConverterUtils.ToCSharpCodeArguments(_patterns)} }}, " +
               $"{CSharpFormatter.ToCSharpBooleanLiteral(IgnoreCase)}, " +
               $"{MatchOperator.GetFullyQualifiedEnumValue()}" +
               $")";
    }

    private static AnyOf<string, StringPattern>[] CreateArray(AnyOf<string, StringPattern>[] patterns)
    {
        return patterns
            .Select(pattern => new AnyOf<string, StringPattern>(
                new StringPattern
                {
                    Pattern = "^" + Regex.Escape(pattern.GetPattern()).Replace(@"\*", ".*").Replace(@"\?", ".") + "$",
                    PatternAsFile = pattern.IsSecond ? pattern.Second.PatternAsFile : null
                }))
            .ToArray();
    }
}