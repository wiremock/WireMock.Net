// Copyright © WireMock.Net

using System.Net;
using WireMock.Settings;

namespace WireMock.ResponseBuilders;

/// <summary>
/// The StatusCodeResponseBuilder interface.
/// </summary>
public interface IStatusCodeResponseBuilder : IHeadersResponseBuilder
{
    /// <summary>
    /// The with status code.
    /// By default all status codes are allowed, to change this behaviour, see <inheritdoc cref="WireMockServerSettings.AllowOnlyDefinedHttpStatusCodeInResponse"/>.
    /// </summary>
    /// <param name="code">The code.</param>
    /// <returns>The <see cref="IResponseBuilder"/>.</returns>
    IResponseBuilder WithStatusCode(int code);

    /// <summary>
    /// The with status code.
    /// By default all status codes are allowed, to change this behaviour, see <inheritdoc cref="WireMockServerSettings.AllowOnlyDefinedHttpStatusCodeInResponse"/>.
    /// </summary>
    /// <param name="code">The code.</param>
    /// <returns>The <see cref="IResponseBuilder"/>.</returns>
    IResponseBuilder WithStatusCode(string code);

    /// <summary>
    /// The with status code.
    /// By default all status codes are allowed, to change this behaviour, see <inheritdoc cref="WireMockServerSettings.AllowOnlyDefinedHttpStatusCodeInResponse"/>.
    /// </summary>
    /// <param name="code">The code.</param>
    /// <returns>The <see cref="IResponseBuilder"/>.</returns>
    IResponseBuilder WithStatusCode(HttpStatusCode code);

    /// <summary>
    /// The with Success status code (200).
    /// </summary>
    /// <returns>The <see cref="IResponseBuilder"/>.</returns>
    IResponseBuilder WithSuccess();

    /// <summary>
    /// The with NotFound status code (404).
    /// </summary>
    /// <returns>The <see cref="IResponseBuilder"/>.</returns>
    IResponseBuilder WithNotFound();
}