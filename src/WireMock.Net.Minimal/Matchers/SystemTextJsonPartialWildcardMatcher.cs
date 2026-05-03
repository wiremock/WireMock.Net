// Copyright © WireMock.Net

using WireMock.Extensions;
using WireMock.Util;

namespace WireMock.Matchers;

/// <summary>
/// SystemTextJsonPartialWildcardMatcher - uses System.Text.Json instead of Newtonsoft.Json.
/// </summary>
public class SystemTextJsonPartialWildcardMatcher : AbstractSystemTextJsonPartialMatcher
{
    /// <inheritdoc />
    public override string Name => nameof(SystemTextJsonPartialWildcardMatcher);

    /// <inheritdoc />
    public SystemTextJsonPartialWildcardMatcher(string value, bool ignoreCase = false, bool regex = false)
        : base(value, ignoreCase, regex)
    {
    }

    /// <inheritdoc />
    public SystemTextJsonPartialWildcardMatcher(object value, bool ignoreCase = false, bool regex = false)
        : base(value, ignoreCase, regex)
    {
    }

    /// <inheritdoc />
    public SystemTextJsonPartialWildcardMatcher(MatchBehaviour matchBehaviour, object value, bool ignoreCase = false, bool regex = false)
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
        return $"new {Name}" +
               $"(" +
               $"{MatchBehaviour.GetFullyQualifiedEnumValue()}, " +
               $"{CSharpFormatter.ConvertToAnonymousObjectDefinition(Value, 3)}, " +
               $"{CSharpFormatter.ToCSharpBooleanLiteral(IgnoreCase)}, " +
               $"{CSharpFormatter.ToCSharpBooleanLiteral(Regex)}" +
               $")";
    }
}
