// Copyright © WireMock.Net

using WireMock.Matchers;

namespace WireMock.RequestBuilders;

/// <summary>
/// The MethodRequestBuilder interface.
/// </summary>
public interface IMethodRequestBuilder : IHeadersRequestBuilder
{
    /// <summary>
    /// UsingConnect: add HTTP Method matching on `CONNECT` and matchBehaviour (optional).
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder UsingConnect(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch);

    /// <summary>
    /// UsingDelete: add HTTP Method matching on `DELETE` and matchBehaviour (optional).
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder UsingDelete(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch);

    /// <summary>
    /// UsingGet: add HTTP Method matching on `GET` and matchBehaviour (optional).
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder UsingGet(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch);

    /// <summary>
    /// UsingHead: Add HTTP Method matching on `HEAD` and matchBehaviour (optional).
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder UsingHead(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch);

    /// <summary>
    /// UsingPost: add HTTP Method matching on `POST` and matchBehaviour (optional).
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder UsingPost(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch);

    /// <summary>
    /// UsingPatch: add HTTP Method matching on `PATCH` and matchBehaviour (optional).
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder UsingPatch(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch);

    /// <summary>
    /// UsingPut: add HTTP Method matching on `OPTIONS` and matchBehaviour (optional).
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder UsingOptions(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch);

    /// <summary>
    /// UsingPut: add HTTP Method matching on `PUT` and matchBehaviour (optional).
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder UsingPut(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch);

    /// <summary>
    /// UsingTrace: add HTTP Method matching on `TRACE` and matchBehaviour (optional).
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder UsingTrace(MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch);

    /// <summary>
    /// UsingAnyMethod: add HTTP Method matching on any method.
    /// </summary>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder UsingAnyMethod();

    /// <summary>
    /// UsingMethod: add HTTP Method matching on any methods and matchBehaviour.
    /// </summary>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <param name="matchOperator">The <see cref="MatchOperator"/> to use.</param>
    /// <param name="methods">The method or methods.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder UsingMethod(MatchBehaviour matchBehaviour, MatchOperator matchOperator, params string[] methods);

    /// <summary>
    /// UsingMethod: add HTTP Method matching on any methods.
    /// </summary>
    /// <param name="methods">The method or methods.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder UsingMethod(params string[] methods);
}