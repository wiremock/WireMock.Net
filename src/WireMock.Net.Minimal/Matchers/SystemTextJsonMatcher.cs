// Copyright © WireMock.Net

using System.Collections;
using System.Text.Json;
using Stef.Validation;
using WireMock.Extensions;
using WireMock.Util;

namespace WireMock.Matchers;

/// <summary>
/// SystemTextJsonMatcher - behaves the same as <see cref="JsonMatcher"/> but uses System.Text.Json instead of Newtonsoft.Json.
/// </summary>
public class SystemTextJsonMatcher : IJsonMatcher
{
    private static readonly JsonSerializerOptions DefaultSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = false
    };

    /// <inheritdoc />
    public virtual string Name => nameof(SystemTextJsonMatcher);

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

    private readonly JsonElement _valueAsJsonElement;

    /// <summary>
    /// Initializes a new instance of the <see cref="SystemTextJsonMatcher"/> class.
    /// </summary>
    /// <param name="value">The string value to check for equality.</param>
    /// <param name="ignoreCase">Ignore the case from the PropertyName and PropertyValue (string only).</param>
    /// <param name="regex">Support Regex.</param>
    public SystemTextJsonMatcher(string value, bool ignoreCase = false, bool regex = false)
        : this(MatchBehaviour.AcceptOnMatch, value, ignoreCase, regex)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SystemTextJsonMatcher"/> class.
    /// </summary>
    /// <param name="value">The object value to check for equality.</param>
    /// <param name="ignoreCase">Ignore the case from the PropertyName and PropertyValue (string only).</param>
    /// <param name="regex">Support Regex.</param>
    public SystemTextJsonMatcher(object value, bool ignoreCase = false, bool regex = false)
        : this(MatchBehaviour.AcceptOnMatch, value, ignoreCase, regex)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SystemTextJsonMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="value">The value to check for equality.</param>
    /// <param name="ignoreCase">Ignore the case from the PropertyName and PropertyValue (string only).</param>
    /// <param name="regex">Support Regex.</param>
    public SystemTextJsonMatcher(MatchBehaviour matchBehaviour, object value, bool ignoreCase = false, bool regex = false)
    {
        Guard.NotNull(value);

        MatchBehaviour = matchBehaviour;
        IgnoreCase = ignoreCase;
        Regex = regex;

        Value = value;
        _valueAsJsonElement = ConvertToJsonElement(value);
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
                var inputAsJsonElement = ConvertToJsonElement(input);

                var match = IsMatch(NormalizeElement(_valueAsJsonElement), NormalizeElement(inputAsJsonElement));
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
    protected virtual bool IsMatch(JsonElement value, JsonElement? input)
    {
        if (input == null)
        {
            return false;
        }

        var inputElement = input.Value;

        // If using Regex and the value is a string, use the MatchRegex method.
        if (Regex && value.ValueKind == JsonValueKind.String)
        {
            var valueAsString = value.GetString()!;
            var inputAsString = inputElement.ValueKind == JsonValueKind.String
                ? inputElement.GetString()!
                : inputElement.GetRawText();

            var (valid, result) = RegexUtils.MatchRegex(valueAsString, inputAsString);
            if (valid)
            {
                return result;
            }
        }

        // If the value is a Guid (string) and input is a string, or vice versa, compare as strings.
        if (value.ValueKind == JsonValueKind.String && inputElement.ValueKind == JsonValueKind.String)
        {
            var valueStr = value.GetString()!;
            var inputStr = inputElement.GetString()!;

            if (Guid.TryParse(valueStr, out var valueGuid) && Guid.TryParse(inputStr, out var inputGuid))
            {
                return valueGuid == inputGuid;
            }
        }

        switch (value.ValueKind)
        {
            case JsonValueKind.Object:
                {
                    if (inputElement.ValueKind != JsonValueKind.Object)
                    {
                        return false;
                    }

                    var valueProperties = value.EnumerateObject().ToDictionary(p => p.Name, p => p.Value);
                    var inputProperties = inputElement.EnumerateObject().ToDictionary(p => p.Name, p => p.Value);

                    if (valueProperties.Count != inputProperties.Count)
                    {
                        return false;
                    }

                    foreach (var pair in valueProperties)
                    {
                        if (!inputProperties.TryGetValue(pair.Key, out var inputPropValue))
                        {
                            return false;
                        }

                        if (!IsMatch(pair.Value, inputPropValue))
                        {
                            return false;
                        }
                    }

                    return true;
                }

            case JsonValueKind.Array:
                {
                    if (inputElement.ValueKind != JsonValueKind.Array)
                    {
                        return false;
                    }

                    var valueArray = value.EnumerateArray().ToArray();
                    var inputArray = inputElement.EnumerateArray().ToArray();

                    if (valueArray.Length != inputArray.Length)
                    {
                        return false;
                    }

                    return !valueArray.Where((valueToken, index) => !IsMatch(valueToken, inputArray[index])).Any();
                }

            default:
                return value.GetRawText() == inputElement.GetRawText();
        }
    }

    private JsonElement NormalizeElement(JsonElement element)
    {
        if (!IgnoreCase)
        {
            return element;
        }

        var normalized = NormalizeValue(element);
        return ConvertToJsonElement(normalized);
    }

    private object NormalizeValue(JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                {
                    var dict = new Dictionary<string, object?>();
                    foreach (var prop in element.EnumerateObject())
                    {
                        var normalizedKey = prop.Name.ToUpperInvariant();
                        dict[normalizedKey] = NormalizeValue(prop.Value);
                    }

                    return dict;
                }

            case JsonValueKind.Array:
                {
                    if (Regex)
                    {
                        return element.EnumerateArray().Select(e => (object)e.GetRawText()).ToArray();
                    }

                    return element.EnumerateArray().Select(NormalizeValue).ToArray();
                }

            case JsonValueKind.String:
                {
                    var str = element.GetString()!;
                    return Regex ? str : str.ToUpperInvariant();
                }

            default:
                return element.GetRawText();
        }
    }

    private static JsonElement ConvertToJsonElement(object value)
    {
        switch (value)
        {
            case JsonElement jsonElement:
                return jsonElement;

            case JsonDocument jsonDocument:
                return jsonDocument.RootElement;

            case string stringValue:
                return JsonDocument.Parse(stringValue).RootElement;

            case IEnumerable enumerableValue when value is not string:
                return JsonSerializer.SerializeToElement(enumerableValue, DefaultSerializerOptions);

            default:
                var json = JsonSerializer.Serialize(value, DefaultSerializerOptions);
                return JsonDocument.Parse(json).RootElement;
        }
    }
}