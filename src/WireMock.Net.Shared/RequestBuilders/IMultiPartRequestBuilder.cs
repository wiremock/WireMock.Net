// Copyright © WireMock.Net

using WireMock.Matchers;
using WireMock.Matchers.Request;

namespace WireMock.RequestBuilders;

/// <summary>
/// The MultiPartRequestBuilder interface.
/// </summary>
public interface IMultiPartRequestBuilder : IHttpVersionBuilder
{
    /// <summary>
    /// WithMultiPart: IMatcher
    /// </summary>
    /// <param name="matcher">The matcher.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithMultiPart(IMatcher matcher);

    /// <summary>
    /// WithMultiPart: IMatcher[], MatchBehaviour and MatchOperator
    /// </summary>
    /// <param name="matchers">The matchers.</param>
    /// <param name="matchBehaviour">The <see cref="MatchBehaviour"/> to use.</param>
    /// <param name="matchOperator">The <see cref="MatchOperator"/> to use.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithMultiPart(IMatcher[] matchers, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch, MatchOperator matchOperator = MatchOperator.Or);

    /// <summary>
    /// WithMultiPart: MatchBehaviour and IMatcher[]
    /// </summary>
    /// <param name="matchBehaviour">The <see cref="MatchBehaviour"/> to use.</param>
    /// <param name="matchers">The matchers.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithMultiPart(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch, params IMatcher[] matchers);
}