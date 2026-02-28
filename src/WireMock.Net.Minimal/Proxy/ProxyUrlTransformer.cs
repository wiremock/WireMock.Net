// Copyright © WireMock.Net

using WireMock.Settings;
using WireMock.Transformers;

namespace WireMock.Proxy;

internal static class ProxyUrlTransformer
{
    internal static string Transform(WireMockServerSettings settings, ProxyUrlReplaceSettings replaceSettings, string url)
    {
        if (!replaceSettings.UseTransformer)
        {
            return url.Replace(replaceSettings.OldValue, replaceSettings.NewValue, replaceSettings.IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
        }
        
        var transformer = TransformerFactory.Create(replaceSettings.TransformerType, settings);
        return transformer.Transform(replaceSettings.TransformTemplate, url);
    }
}