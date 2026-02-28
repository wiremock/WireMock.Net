// Copyright © WireMock.Net

using System.Linq;
using Newtonsoft.Json.Linq;
using Stef.Validation;
using WireMock.Extensions;
using WireMock.Util;
using JsonUtils = WireMock.Util.JsonUtils;

namespace WireMock.Matchers;

/// <summary>
/// JsonMatcher
/// </summary>
public class JsonMatcher : IJsonMatcher
{
    /// <inheritdoc />
    public virtual string Name => nameof(JsonMatcher);

    /// <inheritdoc />
    public object Value { get; }

    /// <inheritdoc />
    public MatchBehaviour MatchBehaviour { get; }

    /// <inheritdoc cref="IIgnoreCaseMatcher.IgnoreCase"/>
    public bool IgnoreCase { get; }

    /// <summary>
    /// Support Regex
    /// </summary>
    public bool Regex { get; }

    private readonly JToken _valueAsJToken;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonMatcher"/> class.
    /// </summary>
    /// <param name="value">The string value to check for equality.</param>
    /// <param name="ignoreCase">Ignore the case from the PropertyName and PropertyValue (string only).</param>
    /// <param name="regex">Support Regex.</param>
    public JsonMatcher(string value, bool ignoreCase = false, bool regex = false) : this(MatchBehaviour.AcceptOnMatch, value, ignoreCase, regex)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonMatcher"/> class.
    /// </summary>
    /// <param name="value">The object value to check for equality.</param>
    /// <param name="ignoreCase">Ignore the case from the PropertyName and PropertyValue (string only).</param>
    /// <param name="regex">Support Regex.</param>
    public JsonMatcher(object value, bool ignoreCase = false, bool regex = false) : this(MatchBehaviour.AcceptOnMatch, value, ignoreCase, regex)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="value">The value to check for equality.</param>
    /// <param name="ignoreCase">Ignore the case from the PropertyName and PropertyValue (string only).</param>
    /// <param name="regex">Support Regex.</param>
    public JsonMatcher(MatchBehaviour matchBehaviour, object value, bool ignoreCase = false, bool regex = false)
    {
        Guard.NotNull(value);

        MatchBehaviour = matchBehaviour;
        IgnoreCase = ignoreCase;
        Regex = regex;

        Value = value;
        _valueAsJToken = JsonUtils.ConvertValueToJToken(value);
    }

    /// <inheritdoc />
    public MatchResult IsMatch(object? input)
    {
        var score = MatchScores.Mismatch;
        Exception? error = null;

        // When input is null or byte[], return Mismatch.
        if (input != null && input is not byte[])
        {
            try
            {
                var inputAsJToken = JsonUtils.ConvertValueToJToken(input);

                var match = IsMatch(RenameJToken(_valueAsJToken), RenameJToken(inputAsJToken));
                score = MatchScores.ToScore(match);
            }
            catch (Exception ex)
            {
                error = ex;
            }
        }

        return MatchResult.From(Name, MatchBehaviourHelper.Convert(MatchBehaviour, score), error);
    }

    /// <inheritdoc />
    public virtual string GetCSharpCodeArguments()
    {
        return $"new {Name}" +
               $"(" +
               $"{MatchBehaviour.GetFullyQualifiedEnumValue()}, " +
               $"{CSharpFormatter.ConvertToAnonymousObjectDefinition(Value, 3)}, " + 
               $"{CSharpFormatter.ToCSharpBooleanLiteral(IgnoreCase)}, " +
               $"{CSharpFormatter.ToCSharpBooleanLiteral(Regex)}" +
               $")";
    }

    /// <summary>
    /// Compares the input against the matcher value
    /// </summary>
    /// <param name="value">Matcher value</param>
    /// <param name="input">Input value</param>
    /// <returns></returns>
    protected virtual bool IsMatch(JToken value, JToken? input)
    {
        // If equal, return true.
        if (input == value)
        {
            return true;
        }

        // If input is null, return false.
        if (input == null)
        {
            return false;
        }

        // If using Regex and the value is a string, use the MatchRegex method.
        if (Regex && value.Type == JTokenType.String)
        {
            var valueAsString = value.ToString();

            var (valid, result) = RegexUtils.MatchRegex(valueAsString, input.ToString());
            if (valid)
            {
                return result;
            }
        }

        // If the value is a Guid and the input is a string, or vice versa, convert them to strings and compare the string values.
        if ((value.Type == JTokenType.Guid && input.Type == JTokenType.String) || (value.Type == JTokenType.String && input.Type == JTokenType.Guid))
        {
            return JToken.DeepEquals(value.ToString().ToUpperInvariant(), input.ToString().ToUpperInvariant());
        }

        switch (value.Type)
        {
            // If the value is an object, compare all properties.
            case JTokenType.Object:
                var valueProperties = value.ToObject<Dictionary<string, JToken>>() ?? new Dictionary<string, JToken>();
                var inputProperties = input.ToObject<Dictionary<string, JToken>>() ?? new Dictionary<string, JToken>();

                // If the number of properties is different, return false.
                if (valueProperties.Count != inputProperties.Count)
                {
                    return false;
                }

                // Compare all properties. The input must match all properties of the value.
                foreach (var pair in valueProperties)
                {
                    if (!IsMatch(pair.Value, inputProperties[pair.Key]))
                    {
                        return false;
                    }
                }

                return true;

            // If the value is an array, compare all elements.
            case JTokenType.Array:
                var valueArray = value.ToObject<JToken[]>() ?? [];
                var inputArray = input.ToObject<JToken[]>() ?? [];

                // If the number of elements is different, return false.
                if (valueArray.Length != inputArray.Length)
                {
                    return false;
                }

                return !valueArray.Where((valueToken, index) => !IsMatch(valueToken, inputArray[index])).Any();

            default:
                // Use JToken.DeepEquals() for all other types.
                return JToken.DeepEquals(value, input);
        }
    }

    // https://stackoverflow.com/questions/11679804/json-net-rename-properties
    private JToken RenameJToken(JToken input)
    {
        if (!IgnoreCase)
        {
            return input;
        }

        return input switch
        {
            JProperty property => RenameJProperty(property),
            JArray array => RenameJArray(array),
            JObject obj => RenameJObject(obj),
            _ => input
        };
    }

    private JProperty RenameJProperty(JProperty property)
    {
        if (!IgnoreCase)
        {
            return property;
        }

        var propertyValue = property.Value;
        if (propertyValue.Type == JTokenType.String && !Regex)
        {
            var stringValue = propertyValue.Value<string>()!;
            propertyValue = ToUpper(stringValue);
        }

        return new JProperty(ToUpper(property.Name)!, RenameJToken(propertyValue));
    }

    private JArray RenameJArray(JArray array)
    {
        if (Regex)
        {
            return array;
        }

        var renamedValues = array.Select(RenameJToken);
        return new JArray(renamedValues);
    }

    private JObject RenameJObject(JObject obj)
    {
        var renamedProperties = obj.Properties().Select(RenameJProperty);
        return new JObject(renamedProperties);
    }

    private static string? ToUpper(string? input)
    {
        return input?.ToUpperInvariant();
    }
}