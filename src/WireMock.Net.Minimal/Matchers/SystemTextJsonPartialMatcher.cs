// Copyright © WireMock.Net

using WireMock.Extensions;
using WireMock.Util;

namespace WireMock.Matchers;

/// <summary>
/// SystemTextJsonPartialMatcher - uses System.Text.Json instead of Newtonsoft.Json.
/// </summary>
public class SystemTextJsonPartialMatcher : AbstractSystemTextJsonPartialMatcher
{
    /// <inheritdoc />
    public override string Name => nameof(SystemTextJsonPartialMatcher);

    /// <inheritdoc />
    public SystemTextJsonPartialMatcher(string value, bool ignoreCase = false, bool regex = false)
        : base(value, ignoreCase, regex)
    {
    }

    /// <inheritdoc />
    public SystemTextJsonPartialMatcher(object value, bool ignoreCase = false, bool regex = false)
        : base(value, ignoreCase, regex)
    {
    }

    /// <inheritdoc />
    public SystemTextJsonPartialMatcher(MatchBehaviour matchBehaviour, object value, bool ignoreCase = false, bool regex = false)
        : base(matchBehaviour, value, ignoreCase, regex)
    {
    }

    /// <inheritdoc />
    protected override bool IsMatch(string value, string input)
    {
        var exactStringMatcher = new ExactMatcher(MatchBehaviour.AcceptOnMatch, IgnoreCase, MatchOperator.Or, value);
        return exactStringMatcher.IsMatch(input).IsPerfect();
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
