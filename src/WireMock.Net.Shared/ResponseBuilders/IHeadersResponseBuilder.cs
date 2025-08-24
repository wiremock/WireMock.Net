// Copyright © WireMock.Net

using System.Collections.Generic;
using WireMock.Types;

namespace WireMock.ResponseBuilders;

/// <summary>
/// The HeadersResponseBuilder interface.
/// </summary>
public interface IHeadersResponseBuilder : IBodyResponseBuilder
{
    /// <summary>
    /// The WithHeader.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="values">The values.</param>
    /// <returns>The <see cref="IResponseBuilder"/>.</returns>
    IResponseBuilder WithHeader(string name, params string[] values);

    /// <summary>
    /// The WithHeaders.
    /// </summary>
    /// <param name="headers">The headers.</param>
    /// <returns>The <see cref="IResponseBuilder"/>.</returns>
    IResponseBuilder WithHeaders(IDictionary<string, string> headers);

    /// <summary>
    /// The WithHeaders.
    /// </summary>
    /// <param name="headers">The headers.</param>
    /// <returns>The <see cref="IResponseBuilder"/>.</returns>
    IResponseBuilder WithHeaders(IDictionary<string, string[]> headers);

    /// <summary>
    /// The WithHeaders.
    /// </summary>
    /// <param name="headers">The headers.</param>
    /// <returns>The <see cref="IResponseBuilder"/>.</returns>
    IResponseBuilder WithHeaders(IDictionary<string, WireMockList<string>> headers);

    /// <summary>
    /// The WithTrailingHeader.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="values">The values.</param>
    /// <returns>The <see cref="IResponseBuilder"/>.</returns>
    IResponseBuilder WithTrailingHeader(string name, params string[] values);

    /// <summary>
    /// The WithTrailingHeaders.
    /// </summary>
    /// <param name="headers">The headers.</param>
    /// <returns>The <see cref="IResponseBuilder"/>.</returns>
    IResponseBuilder WithTrailingHeaders(IDictionary<string, string> headers);

    /// <summary>
    /// The WithTrailingHeaders.
    /// </summary>
    /// <param name="headers">The headers.</param>
    /// <returns>The <see cref="IResponseBuilder"/>.</returns>
    IResponseBuilder WithTrailingHeaders(IDictionary<string, string[]> headers);

    /// <summary>
    /// The WithTrailingHeaders.
    /// </summary>
    /// <param name="headers">The headers.</param>
    /// <returns>The <see cref="IResponseBuilder"/>.</returns>
    IResponseBuilder WithTrailingHeaders(IDictionary<string, WireMockList<string>> headers);
}