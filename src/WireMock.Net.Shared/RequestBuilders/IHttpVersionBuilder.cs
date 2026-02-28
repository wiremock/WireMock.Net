// Copyright © WireMock.Net

using WireMock.Matchers;

namespace WireMock.RequestBuilders;

/// <summary>
/// The HttpVersionBuilder interface.
/// </summary>
public interface IHttpVersionBuilder : IWebSocketRequestBuilder
{
    /// <summary>
    /// WithHttpVersion
    /// </summary>
    /// <param name="version">The HTTP Version to match.</param>
    /// <param name="matchBehaviour">The match behaviour. (default = "AcceptOnMatch")</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithHttpVersion(string version, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch);
}