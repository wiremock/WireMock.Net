// Copyright © WireMock.Net

using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Stef.Validation;
using WireMock.Http;
using WireMock.Matchers;
using WireMock.Serialization;
using WireMock.Settings;
using WireMock.Util;

namespace WireMock.Proxy;

internal class ProxyHelper(WireMockServerSettings settings)
{
    private readonly WireMockServerSettings _settings = Guard.NotNull(settings);
    private readonly ProxyMappingConverter _proxyMappingConverter = new(settings, new GuidUtils(), new DateTimeUtils());

    public async Task<(IResponseMessage Message, IMapping? Mapping)> SendAsync(
        IMapping? mapping,
        ProxyAndRecordSettings proxyAndRecordSettings,
        HttpClient client,
        IRequestMessage requestMessage,
        string url)
    {
        Guard.NotNull(client);
        Guard.NotNull(requestMessage);
        Guard.NotNull(url);

        var originalUri = new Uri(requestMessage.Url);
        var requiredUri = new Uri(url);

        // Create HttpRequestMessage
        var proxyUrl = proxyAndRecordSettings.ReplaceSettings != null ? ProxyUrlTransformer.Transform(_settings, proxyAndRecordSettings.ReplaceSettings, url) : url;
        var httpRequestMessage = HttpRequestMessageHelper.Create(requestMessage, proxyUrl);

        // Call the URL
        var httpResponseMessage = await client.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);

        // Create ResponseMessage
        var bodyHandling = BodyHandling.None;
        if (!_settings.DisableJsonBodyParsing.GetValueOrDefault())
        {
            bodyHandling |= BodyHandling.TryDeserializeJson;
        }

        if (!_settings.DisableRequestBodyDecompressing.GetValueOrDefault())
        {
            bodyHandling |= BodyHandling.DecompressGZipAndDeflate;
        }

        if (!_settings.DisableDeserializeFormUrlEncoded.GetValueOrDefault())
        {
            bodyHandling |= BodyHandling.TryDeserializeFormUrlEncoded;
        }

        var responseMessage = await HttpResponseMessageHelper.CreateAsync(
            httpResponseMessage,
            requiredUri,
            originalUri,
            bodyHandling
        ).ConfigureAwait(false);

        IMapping? newMapping = null;

        var saveMappingSettings = proxyAndRecordSettings.SaveMappingSettings;

        bool save = true;
        if (saveMappingSettings != null)
        {
            save &= Check(saveMappingSettings.StatusCodePattern,
                () => saveMappingSettings.StatusCodePattern != null && HttpStatusRangeParser.IsMatch(saveMappingSettings.StatusCodePattern, responseMessage.StatusCode)
            );

            save &= Check(saveMappingSettings.HttpMethods,
                () => saveMappingSettings.HttpMethods != null && saveMappingSettings.HttpMethods.Value.Contains(requestMessage.Method, StringComparer.OrdinalIgnoreCase)
            );
        }

        if (save && (proxyAndRecordSettings.SaveMapping || proxyAndRecordSettings.SaveMappingToFile))
        {
            newMapping = _proxyMappingConverter.ToMapping(mapping, proxyAndRecordSettings, requestMessage, responseMessage);
        }

        return (responseMessage, newMapping);
    }

    private static bool Check<T>(ProxySaveMappingSetting<T>? saveMappingSetting, Func<bool> action) where T : notnull
    {
        var isMatch = saveMappingSetting is null || action();
        var matchBehaviour = saveMappingSetting?.MatchBehaviour ?? MatchBehaviour.AcceptOnMatch;
        return isMatch == (matchBehaviour == MatchBehaviour.AcceptOnMatch);
    }
}