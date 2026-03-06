// Copyright © WireMock.Net

using Microsoft.AspNetCore.Http;
using Stef.Validation;
using WireMock.Settings;

namespace WireMock.ResponseProviders;

internal class DynamicResponseProvider : IResponseProvider
{
    private readonly Func<HttpContext, IRequestMessage, IResponseMessage> _responseMessageFunc;

    public DynamicResponseProvider(Func<HttpContext, IRequestMessage, IResponseMessage> responseMessageFunc)
    {
        _responseMessageFunc = Guard.NotNull(responseMessageFunc);
    }

    /// <inheritdoc />
    public Task<(IResponseMessage Message, IMapping? Mapping)> ProvideResponseAsync(IMapping mapping, HttpContext context, IRequestMessage requestMessage, WireMockServerSettings settings)
    {
        (IResponseMessage responseMessage, IMapping? mapping) result = (_responseMessageFunc(context, requestMessage), null);
        return Task.FromResult(result);
    }
}