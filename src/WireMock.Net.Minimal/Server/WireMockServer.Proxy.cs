// Copyright © WireMock.Net

using System.Net.Http;
using Microsoft.AspNetCore.Http;
using WireMock.Constants;
using WireMock.Http;
using WireMock.Proxy;
using WireMock.RequestBuilders;
using WireMock.ResponseProviders;
using WireMock.Settings;

namespace WireMock.Server;

public partial class WireMockServer
{
    private HttpClient? _httpClientForProxy;

    private void InitProxyAndRecord(WireMockServerSettings settings)
    {
        if (settings.ProxyAndRecordSettings == null)
        {
            _httpClientForProxy = null;
            DeleteMapping(ProxyMappingGuid);
            return;
        }

        _httpClientForProxy = HttpClientBuilder.Build(settings.ProxyAndRecordSettings);

        var proxyRespondProvider = Given(Request.Create().WithPath("/*").UsingAnyMethod()).WithGuid(ProxyMappingGuid).WithTitle("Default Proxy Mapping on /*");
        if (settings.StartAdminInterface == true)
        {
            proxyRespondProvider.AtPriority(WireMockConstants.ProxyPriority);
        }

        if (settings.ProxyAndRecordSettings.ProxyAll)
        {
            proxyRespondProvider.AtPriority(int.MinValue);
        }

        proxyRespondProvider.RespondWith(new ProxyAsyncResponseProvider(ProxyAndRecordAsync, settings));
    }

    private async Task<IResponseMessage> ProxyAndRecordAsync(HttpContext _, IRequestMessage requestMessage, WireMockServerSettings settings)
    {
        var requestUri = new Uri(requestMessage.Url);
        var proxyUri = new Uri(settings.ProxyAndRecordSettings!.Url);
        var proxyUriWithRequestPathAndQuery = new Uri(proxyUri, requestUri.PathAndQuery);

        var proxyHelper = new ProxyHelper(settings);

        var (responseMessage, mapping) = await proxyHelper.SendAsync(
            null,
            _settings.ProxyAndRecordSettings!,
            _httpClientForProxy!,
            requestMessage,
            proxyUriWithRequestPathAndQuery.AbsoluteUri
        ).ConfigureAwait(false);

        if (mapping != null)
        {
            if (settings.ProxyAndRecordSettings.SaveMapping)
            {
                _options.Mappings.TryAdd(mapping.Guid, mapping);
            }

            if (settings.ProxyAndRecordSettings.SaveMappingToFile)
            {
                _mappingToFileSaver.SaveMappingToFile(mapping);
            }
        }

        return responseMessage;
    }
}