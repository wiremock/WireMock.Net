// Copyright © WireMock.Net

using Microsoft.AspNetCore.Http;
using Stef.Validation;
using WireMock.Settings;

namespace WireMock.ResponseProviders;

internal class DynamicAsyncResponseProvider : IResponseProvider
{
    private readonly Func<HttpContext, IRequestMessage, Task<IResponseMessage>> _responseMessageFunc;

    public DynamicAsyncResponseProvider(Func<HttpContext, IRequestMessage, Task<IResponseMessage>> responseMessageFunc)
    {
        _responseMessageFunc = Guard.NotNull(responseMessageFunc);
    }

    /// <inheritdoc />
    public async Task<(IResponseMessage Message, IMapping? Mapping)> ProvideResponseAsync(IMapping mapping, HttpContext context, IRequestMessage requestMessage, WireMockServerSettings settings)
    {
        return (await _responseMessageFunc(context, requestMessage).ConfigureAwait(false), null);
    }
}