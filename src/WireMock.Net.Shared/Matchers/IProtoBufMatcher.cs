// Copyright © WireMock.Net

using System;
using WireMock.Models;

namespace WireMock.Matchers;

/// <summary>
/// IProtoBufMatcher
/// </summary>
public interface IProtoBufMatcher : IDecodeBytesMatcher, IBytesMatcher
{
    /// <summary>
    /// The Func to define the proto definition as id or texts.
    /// </summary>
    Func<IdOrTexts> ProtoDefinition { get; set; }

    /// <summary>
    /// The full type of the protobuf (request/response) message object. Format is "{package-name}.{type-name}".
    /// </summary>
    string MessageType { get; }

    /// <summary>
    /// The Matcher to use (optional).
    /// </summary>
    IObjectMatcher? Matcher { get; }
}