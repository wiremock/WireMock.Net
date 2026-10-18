// Copyright © WireMock.Net

using WireMock.Settings;
using WireMock.Transformers.Handlebars;
using WireMock.Transformers.Scriban;
using WireMock.Types;

namespace WireMock.Transformers;

internal static class TransformerFactory
{
    internal static ITransformer Create(TransformerType transformerType, WireMockServerSettings settings)
    {
        switch (transformerType)
        {
            case TransformerType.Handlebars:
                var factoryHandlebars = new HandlebarsContextFactory(settings);
                return new Transformer(settings, factoryHandlebars);
                
            case TransformerType.Scriban:
            case TransformerType.ScribanDotLiquid:
                var factoryScriban = new ScribanContextFactory(settings, transformerType);
                return new Transformer(settings, factoryScriban);

            default:
                throw new NotSupportedException($"{nameof(TransformerType)} '{transformerType}' is not supported.");
        }
    }
}