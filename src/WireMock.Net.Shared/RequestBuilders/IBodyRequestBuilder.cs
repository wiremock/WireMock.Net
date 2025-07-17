// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using WireMock.Matchers;
using WireMock.Util;

namespace WireMock.RequestBuilders;

/// <summary>
/// The BodyRequestBuilder interface.
/// </summary>
public interface IBodyRequestBuilder : IProtoBufRequestBuilder
{
    /// <summary>
    /// WithBody: IMatcher
    /// </summary>
    /// <param name="matcher">The matcher.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithBody(IMatcher matcher);

    /// <summary>
    /// WithBody: IMatcher[]
    /// </summary>
    /// <param name="matchers">The matchers.</param>
    /// <param name="matchOperator">The <see cref="MatchOperator"/> to use.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithBody(IMatcher[] matchers, MatchOperator matchOperator = MatchOperator.Or);

    /// <summary>
    /// WithBody: Body as string
    /// </summary>
    /// <param name="body">The body.</param>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithBody(string body, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch);

    /// <summary>
    /// WithBody: Body as byte[]
    /// </summary>
    /// <param name="body">The body.</param>
    /// <param name="matchBehaviour">The match behaviour.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithBody(byte[] body, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch);

    /// <summary>
    /// WithBody: Body as object
    /// </summary>
    /// <param name="body">The body.</param>
    /// <param name="matchBehaviour">The match behaviour [default is AcceptOnMatch].</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithBody(object body, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch);

    /// <summary>
    /// WithBodyAsJson: A <see cref="JsonMatcher"/> will be used to match this object.
    /// </summary>
    /// <param name="body">The body.</param>
    /// <param name="matchBehaviour">The match behaviour [default is AcceptOnMatch].</param>
    /// <returns>A <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithBodyAsJson(object body, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch);

    /// <summary>
    /// WithBody: func (string)
    /// </summary>
    /// <param name="func">The function.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithBody(Func<string?, bool> func);

    /// <summary>
    /// WithBody: func (byte[])
    /// </summary>
    /// <param name="func">The function.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithBody(Func<byte[]?, bool> func);

    /// <summary>
    /// WithBody: func (json object)
    /// </summary>
    /// <param name="func">The function.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithBody(Func<object?, bool> func);

    /// <summary>
    /// WithBody: func (BodyData object)
    /// </summary>
    /// <param name="func">The function.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithBody(Func<IBodyData?, bool> func);

    /// <summary>
    /// WithBody: Body as form-urlencoded values.
    /// </summary>
    /// <param name="func">The form-urlencoded values.</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithBody(Func<IDictionary<string, string>?, bool> func);

    /// <summary>
    /// WithBodyAsGraphQLSchema: Body as GraphQL schema as a string.
    /// </summary>
    /// <param name="body">The GraphQL schema.</param>
    /// <param name="matchBehaviour">The match behaviour. (Default is <c>MatchBehaviour.AcceptOnMatch</c>).</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    IRequestBuilder WithBodyAsGraphQLSchema(string body, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch);
}