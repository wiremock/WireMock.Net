// Copyright © WireMock.Net

using WireMock.Extensions;
using WireMock.Util;

namespace WireMock.Matchers;

/// <summary>
/// JsonPartialWildCardMatcher
/// </summary>
public class JsonPartialWildcardMatcher : AbstractJsonPartialMatcher
{
    /// <inheritdoc />
    public override string Name => nameof(JsonPartialWildcardMatcher);

    /// <inheritdoc />
    public JsonPartialWildcardMatcher(string value, bool ignoreCase = false, bool regex = false)
        : base(value, ignoreCase, regex)
    {
    }

    /// <inheritdoc />
    public JsonPartialWildcardMatcher(object value, bool ignoreCase = false, bool regex = false)
        : base(value, ignoreCase, regex)
    {
    }

    /// <inheritdoc />
    public JsonPartialWildcardMatcher(MatchBehaviour matchBehaviour, object value, bool ignoreCase = false, bool regex = false)
        : base(matchBehaviour, value, ignoreCase, regex)
    {
    }

    /// <inheritdoc />
    protected override bool IsMatch(string value, string input)
    {
        var wildcardStringMatcher = new WildcardMatcher(MatchBehaviour.AcceptOnMatch, value, IgnoreCase);
        return wildcardStringMatcher.IsMatch(input).IsPerfect();
    }

    /// <inheritdoc />
    public override string GetCSharpCodeArguments()
    {
        return $"new {typeof(JsonPartialWildcardMatcher).FullName}" +
               $"(" +
               $"{MatchBehaviour.GetFullyQualifiedEnumValue()}, " +
               $"{CSharpFormatter.ConvertToAnonymousObjectDefinition(Value, 3)}, " +
               $"{CSharpFormatter.ToCSharpBooleanLiteral(IgnoreCase)}, " +
               $"{CSharpFormatter.ToCSharpBooleanLiteral(Regex)}" +
               $")";
    }
}