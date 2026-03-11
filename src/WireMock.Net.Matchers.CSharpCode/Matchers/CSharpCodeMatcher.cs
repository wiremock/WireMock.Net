// Copyright © WireMock.Net

using System.Reflection;
using System.Text;
using AnyOfTypes;
using Newtonsoft.Json.Linq;
using Stef.Validation;
using WireMock.Exceptions;
using WireMock.Extensions;
using WireMock.Models;
using WireMock.Util;

namespace WireMock.Matchers;

/// <summary>
/// CSharpCode / CS-Script Matcher
/// </summary>
/// <inheritdoc cref="ICSharpCodeMatcher"/>
public class CSharpCodeMatcher : ICSharpCodeMatcher
{
    private const string TemplateForIsMatchWithString = "public class CodeHelper {{ public bool IsMatch(string it) {{ {0} }} }}";

    private const string TemplateForIsMatchWithDynamic = "public class CodeHelper {{ public bool IsMatch(dynamic it) {{ {0} }} }}";

    private readonly string[] _usings =
    [
        "System",
        "System.Linq",
        "System.Collections.Generic",
        "Microsoft.CSharp",
        "Newtonsoft.Json.Linq"
    ];

    /// <inheritdoc />
    public MatchBehaviour MatchBehaviour { get; }

    /// <inheritdoc />
    public object Value { get; }

    private readonly AnyOf<string, StringPattern>[] _patterns;

    /// <summary>
    /// Initializes a new instance of the <see cref="CSharpCodeMatcher"/> class.
    /// </summary>
    /// <param name="patterns">The patterns.</param>
    public CSharpCodeMatcher(params AnyOf<string, StringPattern>[] patterns) : this(MatchBehaviour.AcceptOnMatch, MatchOperator.Or, patterns)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CSharpCodeMatcher"/> class.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="matchOperator">The <see cref="Matchers.MatchOperator"/> to use. (default = "Or")</param>
    /// <param name="patterns">The patterns.</param>
    public CSharpCodeMatcher(MatchBehaviour matchBehaviour, MatchOperator matchOperator = MatchOperator.Or, params AnyOf<string, StringPattern>[] patterns)
    {
        _patterns = Guard.NotNull(patterns);
        MatchBehaviour = matchBehaviour;
        MatchOperator = matchOperator;
        Value = patterns;
    }

    /// <inheritdoc />
    public MatchResult IsMatch(string? input) => IsMatchInternal(input);

    /// <inheritdoc />
    public MatchResult IsMatch(object? input) => IsMatchInternal(input);

    /// <inheritdoc />
    public string GetCSharpCodeArguments()
    {
        return $"new {Name}" +
               $"(" +
               $"{MatchBehaviour.GetFullyQualifiedEnumValue()}, " +
               $"{MatchOperator.GetFullyQualifiedEnumValue()}, " +
               $"{MappingConverterUtils.ToCSharpCodeArguments(_patterns)}" +
               $")";
    }

    private MatchResult IsMatchInternal(object? input)
    {
        var score = MatchScores.Mismatch;
        Exception? exception = null;

        if (input != null)
        {
            try
            {
                score = MatchScores.ToScore(_patterns.Select(pattern => IsMatch(input, pattern.GetPattern())).ToArray(), MatchOperator);
            }
            catch (Exception ex)
            {
                exception = ex;
            }
        }

        return MatchResult.From(Name, MatchBehaviourHelper.Convert(MatchBehaviour, score), exception);
    }

    private bool IsMatch(dynamic input, string pattern)
    {
        var isMatchWithString = input is string;
        var inputValue = isMatchWithString ? input : JObject.FromObject(input);
        var source = GetSourceForIsMatchWithString(pattern, isMatchWithString);

        Assembly assembly;
        try
        {
            assembly = CSScriptLib.CSScript.Evaluator.CompileCode(source);
        }
        catch (Exception ex)
        {
            throw new WireMockException($"CSharpCodeMatcher: Unable to compile code `{source}` for WireMock.CodeHelper", ex);
        }

        dynamic script;
        try
        {
            script = CSScripting.ReflectionExtensions.CreateObject(assembly, "*");
        }
        catch (Exception ex)
        {
            throw new WireMockException("CSharpCodeMatcher: Unable to create object from assembly", ex);
        }

        object? result;
        try
        {
            result = script.IsMatch(inputValue);
        }
        catch (Exception ex)
        {
            throw new WireMockException("CSharpCodeMatcher: Problem calling method 'IsMatch' in WireMock.CodeHelper", ex);
        }

        try
        {
            return (bool)result;
        }
        catch
        {
            throw new WireMockException($"Unable to cast result '{result}' to bool");
        }
    }

    private string GetSourceForIsMatchWithString(string pattern, bool isMatchWithString)
    {
        var template = isMatchWithString ? TemplateForIsMatchWithString : TemplateForIsMatchWithDynamic;

        var stringBuilder = new StringBuilder();
        foreach (var @using in _usings)
        {
            stringBuilder.AppendLine($"using {@using};");
        }
        stringBuilder.AppendLine();
        stringBuilder.AppendFormat(template, pattern);

        return stringBuilder.ToString();
    }

    /// <inheritdoc />
    public AnyOf<string, StringPattern>[] GetPatterns()
    {
        return _patterns;
    }

    /// <inheritdoc />
    public MatchOperator MatchOperator { get; }

    /// <inheritdoc />
    public string Name => nameof(CSharpCodeMatcher);
}