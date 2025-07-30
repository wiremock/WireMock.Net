// Copyright © WireMock.Net

using Stef.Validation;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.Matchers.Helpers;

internal static class BodyDataMatchScoreCalculator
{
    internal static MatchResult CalculateMatchScore(IBodyData? requestMessage, IMatcher matcher)
    {
        Guard.NotNull(matcher);

        if (requestMessage == null)
        {
            return default;
        }

        if (matcher is NotNullOrEmptyMatcher notNullOrEmptyMatcher)
        {
            switch (requestMessage.DetectedBodyType)
            {
                case BodyType.Json:
                case BodyType.String:
                case BodyType.FormUrlEncoded:
                    return notNullOrEmptyMatcher.IsMatch(requestMessage.BodyAsString);

                case BodyType.Bytes:
                    return notNullOrEmptyMatcher.IsMatch(requestMessage.BodyAsBytes);

                default:
                    return default;
            }
        }

        if (matcher is ExactObjectMatcher { Value: byte[] } exactObjectMatcher)
        {
            // If the body is a byte array, try to match.
            return exactObjectMatcher.IsMatch(requestMessage.BodyAsBytes);
        }

        // Check if the matcher is a IObjectMatcher
        if (matcher is IObjectMatcher objectMatcher)
        {
            // If the body is a JSON object, try to match.
            if (requestMessage.DetectedBodyType == BodyType.Json)
            {
                return objectMatcher.IsMatch(requestMessage.BodyAsJson);
            }

            // If the body is a byte array, try to match.
            if (requestMessage.DetectedBodyType == BodyType.Bytes)
            {
                return objectMatcher.IsMatch(requestMessage.BodyAsBytes);
            }
        }

        // In case the matcher is a IStringMatcher and If  body is a Json or a String, use the BodyAsString to match on.
        if (matcher is IStringMatcher stringMatcher && requestMessage.DetectedBodyType is BodyType.Json or BodyType.String or BodyType.FormUrlEncoded)
        {
            return stringMatcher.IsMatch(requestMessage.BodyAsString);
        }

        // In case the matcher is a IProtoBufMatcher, use the BodyAsBytes to match on.
        if (matcher is IProtoBufMatcher protoBufMatcher)
        {
            return protoBufMatcher.IsMatchAsync(requestMessage.BodyAsBytes).GetAwaiter().GetResult();
        }

        return default;
    }
}
