// Copyright © WireMock.Net

using WireMock.Matchers.Request;

namespace WireMock.RequestBuilders;

/// <summary>
/// IRequestBuilder
/// </summary>
public interface IRequestBuilder : IClientIPRequestBuilder
{
    public IRequestBuilder Add<T>(T requestMatcher) where T : IRequestMatcher;
}