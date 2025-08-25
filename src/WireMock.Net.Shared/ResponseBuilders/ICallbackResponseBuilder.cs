// Copyright © WireMock.Net

using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WireMock.ResponseProviders;

namespace WireMock.ResponseBuilders;

/// <summary>
/// The CallbackResponseBuilder interface.
/// </summary>
public interface ICallbackResponseBuilder : IResponseProvider
{
    /// <summary>
    /// The callback builder
    /// </summary>
    /// <returns>The <see cref="IResponseBuilder"/>.</returns>
    [PublicAPI]
    IResponseBuilder WithCallback(Func<IRequestMessage, IResponseMessage> callbackHandler);

    /// <summary>
    /// The async callback builder
    /// </summary>
    /// <returns>The <see cref="IResponseBuilder"/>.</returns>
    [PublicAPI]
    IResponseBuilder WithCallback(Func<IRequestMessage, Task<IResponseMessage>> callbackHandler);
}