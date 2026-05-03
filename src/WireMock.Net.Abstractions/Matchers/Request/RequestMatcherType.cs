// Copyright © WireMock.Net

namespace WireMock.Matchers.Request;

/// <summary>
/// List of predefined request matcher types.
/// </summary>
public enum RequestMatcherType
{
    /// <summary>
    /// RequestMessageBodyMatcher
    /// </summary>
    Body = 0,

    /// <summary>
    /// RequestMessageBodyMatcher{T}
    /// </summary>
    BodyOfT = 1,

    /// <summary>
    /// RequestMessageClientIPMatcher
    /// </summary>
    ClientIP = 2,

    /// <summary>
    /// RequestMessageCookieMatcher
    /// </summary>
    Cookie = 3,

    /// <summary>
    /// RequestMessageGraphQLMatcher
    /// </summary>
    GraphQL = 4,

    /// <summary>
    /// RequestMessageHeaderMatcher
    /// </summary>
    Header = 5,

    /// <summary>
    /// RequestMessageHttpVersionMatcher
    /// </summary>
    HttpVersion = 6,

    /// <summary>
    /// RequestMessageMethodMatcher
    /// </summary>
    Method = 7,

    /// <summary>
    /// RequestMessageMultiPartMatcher
    /// </summary>
    MultiPart = 8,

    /// <summary>
    /// RequestMessageParamMatcher
    /// </summary>
    Param = 9,

    /// <summary>
    /// RequestMessagePathMatcher
    /// </summary>
    Path = 10,

    /// <summary>
    /// RequestMessageProtoBufMatcher
    /// </summary>
    ProtoBuf = 11,

    /// <summary>
    /// RequestMessageScenarioAndStateMatcher
    /// </summary>
    ScenarioAndState = 12,

    /// <summary>
    /// RequestMessageUrlMatcher
    /// </summary>
    Url = 13,

    /// <summary>
    /// RequestMessageCompositeMatcher
    /// </summary>
    Composite = 14
}