// Copyright © WireMock.Net

using WireMock.Extensions;
using WireMock.Matchers;

// ReSharper disable once CheckNamespace
namespace WireMock.AwesomeAssertions;

#pragma warning disable CS1591
public partial class WireMockAssertions
{
    [CustomAssertion]
    public AndWhichConstraint<WireMockAssertions, string> AtAbsolutePath(string absolutePath, string because = "", params object[] becauseArgs)
    {
        _ = AtAbsolutePath(new ExactMatcher(true, absolutePath), because, becauseArgs);

        return new AndWhichConstraint<WireMockAssertions, string>(this, absolutePath);
    }

    [CustomAssertion]
    public AndWhichConstraint<WireMockAssertions, IStringMatcher> AtAbsolutePath(IStringMatcher absolutePathMatcher, string because = "", params object[] becauseArgs)
    {
        var (filter, condition) = BuildFilterAndCondition(request => absolutePathMatcher.IsPerfectMatch(request.AbsolutePath));

        var absolutePath = absolutePathMatcher.GetPatterns().FirstOrDefault().GetPattern();

        _chain
            .BecauseOf(because, becauseArgs)
            .Given(() => RequestMessages)
            .ForCondition(requests => CallsCount == 0 || requests.Any())
            .FailWith(
                "Expected {context:wiremockserver} to have been called at address matching the absolute path {0}{reason}, but no calls were made.",
                absolutePath
            )
            .Then
            .ForCondition(condition)
            .FailWith(
                "Expected {context:wiremockserver} to have been called at address matching the absolute path {0}{reason}, but didn't find it among the calls to {1}.",
                _ => absolutePath,
                requests => requests.Select(request => request.AbsolutePath)
            );

        FilterRequestMessages(filter);

        return new AndWhichConstraint<WireMockAssertions, IStringMatcher>(this, absolutePathMatcher);
    }

    [CustomAssertion]
    public AndWhichConstraint<WireMockAssertions, string> AtPath(string path, string because = "", params object[] becauseArgs)
    {
        _ = AtPath(new ExactMatcher(true, path), because, becauseArgs);

        return new AndWhichConstraint<WireMockAssertions, string>(this, path);
    }

    [CustomAssertion]
    public AndWhichConstraint<WireMockAssertions, IStringMatcher> AtPath(IStringMatcher pathMatcher, string because = "", params object[] becauseArgs)
    {
        var (filter, condition) = BuildFilterAndCondition(request => pathMatcher.IsPerfectMatch(request.Path));

        var path = pathMatcher.GetPatterns().FirstOrDefault().GetPattern();

        _chain
            .BecauseOf(because, becauseArgs)
            .Given(() => RequestMessages)
            .ForCondition(requests => CallsCount == 0 || requests.Any())
            .FailWith(
                "Expected {context:wiremockserver} to have been called at address matching the path {0}{reason}, but no calls were made.",
                path
            )
            .Then
            .ForCondition(condition)
            .FailWith(
                "Expected {context:wiremockserver} to have been called at address matching the path {0}{reason}, but didn't find it among the calls to {1}.",
                _ => path,
                requests => requests.Select(request => request.Path)
            );

        FilterRequestMessages(filter);

        return new AndWhichConstraint<WireMockAssertions, IStringMatcher>(this, pathMatcher);
    }
}