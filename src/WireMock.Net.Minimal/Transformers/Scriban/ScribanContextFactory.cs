// Copyright © WireMock.Net

using Scriban.Runtime;
using Stef.Validation;
using WireMock.Settings;
using WireMock.Types;

namespace WireMock.Transformers.Scriban;

internal class ScribanContextFactory : ITransformerContextFactory
{
    private readonly WireMockServerSettings _settings;
    private readonly TransformerType _transformerType;

    public ScribanContextFactory(WireMockServerSettings settings, TransformerType transformerType)
    {
        _settings = Guard.NotNull(settings);
        _transformerType = Guard.Condition(transformerType, t => t is TransformerType.Scriban or TransformerType.ScribanDotLiquid);
    }

    public ITransformerContext Create()
    {
        ScriptObject? customGlobals = null;
        if (_settings.ScribanRegistrationCallback is { } registrationCallback)
        {
            customGlobals = new ScriptObject();
            registrationCallback(customGlobals, _settings.FileSystemHandler);
        }

        return new ScribanContext(_settings.FileSystemHandler, _transformerType, customGlobals);
    }
}