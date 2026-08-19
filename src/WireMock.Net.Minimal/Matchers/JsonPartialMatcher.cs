// Copyright © WireMock.Net

using WireMock.Extensions;
using WireMock.Util;

namespace WireMock.Matchers;

/// <summary>
/// JsonPartialMatcher
/// </summary>
public class JsonPartialMatcher : AbstractJsonPartialMatcher
{
    /// <inheritdoc />
    public override string Name => nameof(JsonPartialMatcher);

    /// <inheritdoc />
    public JsonPartialMatcher(string value, bool ignoreCase = false, bool regex = false)
        : base(value, ignoreCase, regex)
    {
    }

    /// <inheritdoc />
    public JsonPartialMatcher(object value, bool ignoreCase = false, bool regex = false)
        : base(value, ignoreCase, regex)
    {
    }

    /// <inheritdoc />
    public JsonPartialMatcher(MatchBehaviour matchBehaviour, object value, bool ignoreCase = false, bool regex = false)
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
        return $"new {typeof(JsonPartialMatcher).FullName}" +
               $"(" +
               $"{MatchBehaviour.GetFullyQualifiedEnumValue()}, " +
               $"{CSharpFormatter.ConvertToAnonymousObjectDefinition(Value, 3)}, " +
               $"{CSharpFormatter.ToCSharpBooleanLiteral(IgnoreCase)}, " +
               $"{CSharpFormatter.ToCSharpBooleanLiteral(Regex)}" +
               $")";
    }
}