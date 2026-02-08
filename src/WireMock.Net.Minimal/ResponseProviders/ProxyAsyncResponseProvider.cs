// Copyright © WireMock.Net

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Stef.Validation;
using WireMock.Settings;

namespace WireMock.ResponseProviders;

internal class ProxyAsyncResponseProvider : IResponseProvider
{
    private readonly Func<HttpContext, IRequestMessage, WireMockServerSettings, Task<IResponseMessage>> _responseMessageFunc;
    private readonly WireMockServerSettings _settings;

    public ProxyAsyncResponseProvider(Func<HttpContext, IRequestMessage, WireMockServerSettings, Task<IResponseMessage>> responseMessageFunc, WireMockServerSettings settings)
    {
        _responseMessageFunc = Guard.NotNull(responseMessageFunc);
        _settings = Guard.NotNull(settings);
    }

    /// <inheritdoc />
    public async Task<(IResponseMessage Message, IMapping? Mapping)> ProvideResponseAsync(IMapping mapping, HttpContext context, IRequestMessage requestMessage, WireMockServerSettings settings)
    {
        return (await _responseMessageFunc(context, requestMessage, _settings).ConfigureAwait(false), null);
    }
}