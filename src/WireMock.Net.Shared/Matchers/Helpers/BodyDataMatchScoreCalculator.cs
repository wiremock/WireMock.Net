// Copyright © WireMock.Net

using Stef.Validation;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.Matchers.Helpers;

internal static class BodyDataMatchScoreCalculator
{
    internal static MatchResult CalculateMatchScore(IBodyData? bodyData, IMatcher matcher)
    {
        Guard.NotNull(matcher);

        if (bodyData == null)
        {
            return default;
        }

        if (matcher is NotNullOrEmptyMatcher notNullOrEmptyMatcher)
        {
            switch (bodyData.DetectedBodyType)
            {
                case BodyType.Json:
                case BodyType.String:
                case BodyType.FormUrlEncoded:
                    return notNullOrEmptyMatcher.IsMatch(bodyData.BodyAsString);

                case BodyType.Bytes:
                    return notNullOrEmptyMatcher.IsMatch(bodyData.BodyAsBytes);

                default:
                    return default;
            }
        }

        if (matcher is ExactObjectMatcher { Value: byte[] } exactObjectMatcher)
        {
            // If the body is a byte array, try to match.
            return exactObjectMatcher.IsMatch(bodyData.BodyAsBytes);
        }

        // Check if the matcher is a IObjectMatcher
        if (matcher is IObjectMatcher objectMatcher)
        {
            // If the body is a JSON object, try to match.
            if (bodyData.DetectedBodyType == BodyType.Json)
            {
                return objectMatcher.IsMatch(bodyData.BodyAsJson);
            }

            // If the body is a byte array, try to match.
            if (bodyData.DetectedBodyType == BodyType.Bytes)
            {
                return objectMatcher.IsMatch(bodyData.BodyAsBytes);
            }
        }

        // In case the matcher is a IStringMatcher and if body is a Json or a String, use the BodyAsString to match on.
        if (matcher is IStringMatcher stringMatcher && bodyData.DetectedBodyType is BodyType.Json or BodyType.String or BodyType.FormUrlEncoded)
        {
            return stringMatcher.IsMatch(bodyData.BodyAsString);
        }

        // In case the matcher is a IProtoBufMatcher, use the BodyAsBytes to match on.
        if (matcher is IProtoBufMatcher protoBufMatcher)
        {
            return protoBufMatcher.IsMatchAsync(bodyData.BodyAsBytes).GetAwaiter().GetResult();
        }

        return default;
    }
}