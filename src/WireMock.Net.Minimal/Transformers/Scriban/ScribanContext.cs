// Copyright © WireMock.Net

using Scriban;
using Scriban.Runtime;
using Stef.Validation;
using WireMock.Handlers;
using WireMock.Types;

namespace WireMock.Transformers.Scriban;

internal class ScribanContext : ITransformerContext
{
    private static readonly MemberRenamerDelegate MemberRenamer = member => member.Name;

    private readonly TransformerType _transformerType;
    private readonly ScriptObject? _customGlobals;

    public IFileSystemHandler FileSystemHandler { get; }

    public ScribanContext(IFileSystemHandler fileSystemHandler, TransformerType transformerType, ScriptObject? customGlobals = null)
    {
        FileSystemHandler = Guard.NotNull(fileSystemHandler);
        _transformerType = transformerType;
        _customGlobals = customGlobals;
    }

    public string ParseAndRender(string text, object model)
    {
        var isLiquid = _transformerType == TransformerType.ScribanDotLiquid;
        var template = isLiquid ? Template.ParseLiquid(text) : Template.Parse(text);

        if (_customGlobals is null)
        {
            return template.Render(model, MemberRenamer);
        }

        // Same as Template.Render(model, MemberRenamer), but with the custom globals pushed beneath the model so that the model takes precedence.
        var context = isLiquid ? new LiquidTemplateContext() : new TemplateContext();
        context.MemberRenamer = MemberRenamer;
        context.PushGlobal(_customGlobals);

        var modelGlobals = new ScriptObject();
        modelGlobals.Import(model, renamer: MemberRenamer);
        context.PushGlobal(modelGlobals);

        return template.Render(context);
    }

    public object? ParseAndEvaluate(string text, object model)
    {
        // In case of Scriban, call ParseAndRender.
        return ParseAndRender(text, model);
    }
}