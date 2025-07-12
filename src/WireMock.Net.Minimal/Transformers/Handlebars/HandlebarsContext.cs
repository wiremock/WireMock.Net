// Copyright © WireMock.Net

using System.Text.RegularExpressions;
using HandlebarsDotNet;
using HandlebarsDotNet.Helpers.Extensions;
using Stef.Validation;
using WireMock.Handlers;

namespace WireMock.Transformers.Handlebars;

internal class HandlebarsContext : IHandlebarsContext
{
    private static readonly Regex _tryEvaluateRegex = new(@"\{\{.*?\}\}", RegexOptions.Compiled);

    public IHandlebars Handlebars { get; }

    public IFileSystemHandler FileSystemHandler { get; }

    public HandlebarsContext(IHandlebars handlebars, IFileSystemHandler fileSystemHandler)
    {
        Handlebars = Guard.NotNull(handlebars);
        FileSystemHandler = Guard.NotNull(fileSystemHandler);
    }

    public string ParseAndRender(string text, object model)
    {
        var template = Handlebars.Compile(text);
        return template(model);
    }

    public object? ParseAndEvaluate(string text, object model)
    {
        // Only try to evaluate if the text matches the pattern `{{ xxx }}` exactly once.
        if (_tryEvaluateRegex.Matches(text).Count == 1 && Handlebars.TryEvaluate(text, model, out var result) && result is not UndefinedBindingResult)
        {
            return result;
        }

        return ParseAndRender(text, model);
    }
}