// Copyright © WireMock.Net

using Stef.Validation;
using WireMock.Extensions;

namespace WireMock.Matchers;

/// <summary>
/// FuncMatcher - matches using a custom function
/// </summary>
/// <inheritdoc cref="IFuncMatcher"/>
public class FuncMatcher : IFuncMatcher
{
    private readonly Func<string?, bool>? _stringFunc;
    private readonly Func<byte[]?, bool>? _bytesFunc;

    /// <inheritdoc />
    public MatchBehaviour MatchBehaviour { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FuncMatcher"/> class for string matching.
    /// </summary>
    /// <param name="func">The function to check if a string is a match.</param>
    public FuncMatcher(Func<string?, bool> func) : this(MatchBehaviour.AcceptOnMatch, func)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FuncMatcher"/> class for string matching.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="func">The function to check if a string is a match.</param>
    public FuncMatcher(MatchBehaviour matchBehaviour, Func<string?, bool> func)
    {
        _stringFunc = Guard.NotNull(func);
        MatchBehaviour = matchBehaviour;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FuncMatcher"/> class for byte array matching.
    /// </summary>
    /// <param name="func">The function to check if a byte[] is a match.</param>
    public FuncMatcher(Func<byte[]?, bool> func) : this(MatchBehaviour.AcceptOnMatch, func)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FuncMatcher"/> class for byte array matching.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="func">The function to check if a byte[] is a match.</param>
    public FuncMatcher(MatchBehaviour matchBehaviour, Func<byte[]?, bool> func)
    {
        _bytesFunc = Guard.NotNull(func);
        MatchBehaviour = matchBehaviour;
    }

    /// <inheritdoc />
    public MatchResult IsMatch(object? value)
    {
        if (value is string stringValue && _stringFunc != null)
        {
            try
            {
                return CreateMatchResult(_stringFunc(stringValue));
            }
            catch (Exception ex)
            {
                return MatchResult.From(Name, ex);
            }
        }

        if (value is byte[] bytesValue && _bytesFunc != null)
        {
            try
            {
                return CreateMatchResult(_bytesFunc(bytesValue));
            }
            catch (Exception ex)
            {
                return MatchResult.From(Name, ex);
            }
        }

        return MatchResult.From(Name, MatchScores.Mismatch);
    }

    private MatchResult CreateMatchResult(bool isMatch)
    {
        return MatchResult.From(Name, MatchBehaviourHelper.Convert(MatchBehaviour, MatchScores.ToScore(isMatch)));
    }

    /// <inheritdoc />
    public string Name => nameof(FuncMatcher);

    /// <inheritdoc />
    public string GetCSharpCodeArguments()
    {
        var funcType = _stringFunc != null ? "Func<string?, bool>" : "Func<byte[]?, bool>";
        return $"new {Name}" +
               $"(" +
               $"{MatchBehaviour.GetFullyQualifiedEnumValue()}, " +
               $"/* {funcType} function */" +
               $")";
    }
}
