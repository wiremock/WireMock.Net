// Copyright © WireMock.Net

namespace WireMock.Owin;

internal interface IMappingMatcher
{
    (MappingMatcherResult? Match, MappingMatcherResult? Partial) FindBestMatch(RequestMessage request);

    IReadOnlyList<MappingMatcherResult> FindNearMisses(IRequestMessage request, int maxResults);
}