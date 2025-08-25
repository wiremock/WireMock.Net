// Copyright © WireMock.Net

using WireMock.Matchers.Request;

namespace WireMock.RequestBuilders;

/// <summary>
/// IRequestBuilder
/// </summary>
public interface IRequestBuilder : IClientIPRequestBuilder
{
    /// <summary>
    /// Adds a request matcher to the builder.
    /// </summary>
    /// <typeparam name="T">The type of the request matcher.</typeparam>
    /// <param name="requestMatcher">The request matcher to add.</param>
    /// <returns>The current <see cref="IRequestBuilder"/> instance.</returns>
    IRequestBuilder Add<T>(T requestMatcher) where T : IRequestMatcher;

    /// <summary>
    /// The link back to the Mapping.
    /// </summary>
    IMapping Mapping { get; set; }
}