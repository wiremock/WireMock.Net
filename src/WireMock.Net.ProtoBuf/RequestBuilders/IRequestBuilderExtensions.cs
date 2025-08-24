// Copyright © WireMock.Net

using System;
using System.Collections.Generic;
using WireMock.Matchers;
using WireMock.Matchers.Request;
using WireMock.Models;

namespace WireMock.RequestBuilders;

/// <summary>
/// IRequestBuilderExtensions extensions for ProtoBuf.
/// </summary>
// ReSharper disable once InconsistentNaming
public static class IRequestBuilderExtensions
{
    /// <summary>
    /// WithBodyAsProtoBuf
    /// </summary>
    /// <param name="requestBuilder">The <see cref="IRequestBuilder"/>.</param>
    /// <param name="protoDefinition">The proto definition as text.</param>
    /// <param name="messageType">The full type of the protobuf (request/response) message object. Format is "{package-name}.{type-name}".</param>
    /// <param name="matchBehaviour">The match behaviour. (default = "AcceptOnMatch")</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    public static IRequestBuilder WithBodyAsProtoBuf(this IRequestBuilder requestBuilder, string protoDefinition, string messageType, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        return requestBuilder.WithBodyAsProtoBuf([protoDefinition], messageType, matchBehaviour);
    }

    /// <summary>
    /// WithBodyAsProtoBuf
    /// </summary>
    /// <param name="requestBuilder">The <see cref="IRequestBuilder"/>.</param>
    /// <param name="protoDefinition">The proto definition as text.</param>
    /// <param name="messageType">The full type of the protobuf (request/response) message object. Format is "{package-name}.{type-name}".</param>
    /// <param name="matcher">The matcher to use to match the ProtoBuf as (json) object.</param>
    /// <param name="matchBehaviour">The match behaviour. (default = "AcceptOnMatch")</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    public static IRequestBuilder WithBodyAsProtoBuf(this IRequestBuilder requestBuilder, string protoDefinition, string messageType, IObjectMatcher matcher, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        return requestBuilder.WithBodyAsProtoBuf([protoDefinition], messageType, matcher, matchBehaviour);
    }

    /// <summary>
    /// WithBodyAsProtoBuf
    /// </summary>
    /// <param name="requestBuilder">The <see cref="IRequestBuilder"/>.</param>
    /// <param name="protoDefinitions">The proto definitions as text.</param>
    /// <param name="messageType">The full type of the protobuf (request/response) message object. Format is "{package-name}.{type-name}".</param>
    /// <param name="matchBehaviour">The match behaviour. (default = "AcceptOnMatch")</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    public static IRequestBuilder WithBodyAsProtoBuf(this IRequestBuilder requestBuilder, IReadOnlyList<string> protoDefinitions, string messageType, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        return requestBuilder.Add(new RequestMessageProtoBufMatcher(matchBehaviour, () => new IdOrTexts(null, protoDefinitions), messageType));
    }

    /// <summary>
    /// WithBodyAsProtoBuf
    /// </summary>
    /// <param name="requestBuilder">The <see cref="IRequestBuilder"/>.</param>
    /// <param name="protoDefinitions">The proto definitions as text.</param>
    /// <param name="messageType">The full type of the protobuf (request/response) message object. Format is "{package-name}.{type-name}".</param>
    /// <param name="matcher">The matcher to use to match the ProtoBuf as (json) object.</param>
    /// <param name="matchBehaviour">The match behaviour. (default = "AcceptOnMatch")</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    public static IRequestBuilder WithBodyAsProtoBuf(this IRequestBuilder requestBuilder, IReadOnlyList<string> protoDefinitions, string messageType, IObjectMatcher matcher, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        return requestBuilder.Add(new RequestMessageProtoBufMatcher(matchBehaviour, () => new IdOrTexts(null, protoDefinitions), messageType, matcher));
    }

    /// <summary>
    /// WithBodyAsProtoBuf
    /// </summary>
    /// <param name="requestBuilder">The <see cref="IRequestBuilder"/>.</param>
    /// <param name="messageType">The full type of the protobuf (request/response) message object. Format is "{package-name}.{type-name}".</param>
    /// <param name="matchBehaviour">The match behaviour. (default = "AcceptOnMatch")</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    public static IRequestBuilder WithBodyAsProtoBuf(this IRequestBuilder requestBuilder, string messageType, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        return requestBuilder.Add(new RequestMessageProtoBufMatcher(matchBehaviour, ProtoDefinitionFunc(requestBuilder), messageType));
    }

    /// <summary>
    /// WithBodyAsProtoBuf
    /// </summary>
    /// <param name="requestBuilder">The <see cref="IRequestBuilder"/>.</param>
    /// <param name="messageType">The full type of the protobuf (request/response) message object. Format is "{package-name}.{type-name}".</param>
    /// <param name="matcher">The matcher to use to match the ProtoBuf as (json) object.</param>
    /// <param name="matchBehaviour">The match behaviour. (default = "AcceptOnMatch")</param>
    /// <returns>The <see cref="IRequestBuilder"/>.</returns>
    public static IRequestBuilder WithBodyAsProtoBuf(this IRequestBuilder requestBuilder, string messageType, IObjectMatcher matcher, MatchBehaviour matchBehaviour = MatchBehaviour.AcceptOnMatch)
    {
        return requestBuilder.Add(new RequestMessageProtoBufMatcher(matchBehaviour, ProtoDefinitionFunc(requestBuilder), messageType, matcher));
    }

    private static Func<IdOrTexts> ProtoDefinitionFunc(IRequestBuilder requestBuilder)
    {
        return () =>
        {
            if (requestBuilder.Mapping.ProtoDefinition == null)
            {
                throw new InvalidOperationException($"No ProtoDefinition defined on mapping '{requestBuilder.Mapping.Guid}'. Please use the WireMockServerSettings to define ProtoDefinitions.");
            }

            return requestBuilder.Mapping.ProtoDefinition.Value;
        };
    }
}