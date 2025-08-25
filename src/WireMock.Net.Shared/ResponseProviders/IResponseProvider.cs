// Copyright © WireMock.Net and mock4net by Alexandre Victoor

// This source file is based on mock4net by Alexandre Victoor which is licensed under the Apache 2.0 License.
// For more details see 'mock4net/LICENSE.txt' and 'mock4net/readme.md' in this project root.
using System.Threading.Tasks;
using WireMock.Settings;

namespace WireMock.ResponseProviders;

/// <summary>
/// The Response Provider interface.
/// </summary>
public interface IResponseProvider
{
    /// <summary>
    /// The provide response.
    /// </summary>
    /// <param name="mapping">The used mapping.</param>
    /// <param name="requestMessage">The request.</param>
    /// <param name="settings">The WireMockServerSettings.</param>
    /// <returns>The <see cref="IResponseMessage"/> including a new (optional) <see cref="IMapping"/>.</returns>
    Task<(IResponseMessage Message, IMapping? Mapping)> ProvideResponseAsync(IMapping mapping, IRequestMessage requestMessage, WireMockServerSettings settings);
}