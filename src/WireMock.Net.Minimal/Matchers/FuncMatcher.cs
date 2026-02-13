// Copyright © WireMock.Net

using System;
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
    /// <param name="matchBehaviour">The match behaviour.</param>
    public FuncMatcher(Func<string?, bool> func, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        _stringFunc = Guard.NotNull(func);
        MatchBehaviour = matchBehaviour;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FuncMatcher"/> class for byte array matching.
    /// </summary>
    /// <param name="func">The function to check if a byte[] is a match.</param>
    /// <param name="matchBehaviour">The match behaviour.</param>
    public FuncMatcher(Func<byte[]?, bool> func, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
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
                return MatchResult.From(Name, MatchBehaviour, _stringFunc(stringValue));
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
                return MatchResult.From(Name, MatchBehaviour, _bytesFunc(bytesValue));
            }
            catch (Exception ex)
            {
                return MatchResult.From(Name, ex);
            }
        }

        return MatchResult.From(Name, MatchScores.Mismatch);
    }

    /// <inheritdoc />
    public string Name => nameof(FuncMatcher);

    /// <inheritdoc />
    public string GetCSharpCodeArguments()
    {
        var funcType = _stringFunc != null ? "Func<string?, bool>" : "Func<byte[]?, bool>";
        return $"new {Name}" +
               $"(" +
               $"/* {funcType} function */, " +
               $"{MatchBehaviour.GetFullyQualifiedEnumValue()}" +
               $")";
    }
}
