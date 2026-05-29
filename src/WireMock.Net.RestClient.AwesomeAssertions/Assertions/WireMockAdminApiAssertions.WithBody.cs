// Copyright © WireMock.Net

#pragma warning disable CS1591
using AnyOfTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WireMock.Admin.Requests;
using WireMock.Extensions;
using WireMock.Matchers;
using WireMock.Models;

// ReSharper disable once CheckNamespace
namespace WireMock.Client.AwesomeAssertions;

public partial class WireMockAdminApiAssertions
{
    private const string MessageFormatNoCalls = "Expected {context:wiremockadminapi} to have been called using body {0}{reason}, but no calls were made.";
    private const string MessageFormat = "Expected {context:wiremockadminapi} to have been called using body {0}{reason}, but didn't find it among the body/bodies {1}.";

    [CustomAssertion]
    public AndConstraint<WireMockAdminApiAssertions> WithBody(string body, string because = "", params object[] becauseArgs)
    {
        return WithBody(new WildcardMatcher(body), because, becauseArgs);
    }

    [CustomAssertion]
    public AndConstraint<WireMockAdminApiAssertions> WithBody(IStringMatcher matcher, string because = "", params object[] becauseArgs)
    {
        var (filter, condition) = BuildFilterAndCondition(r => r.Body, matcher);

        return ExecuteAssertionWithBodyStringMatcher(matcher, because, becauseArgs, condition, filter, r => r.Body);
    }

    [CustomAssertion]
    public AndConstraint<WireMockAdminApiAssertions> WithBodyAsJson(object body, IJsonMatcher? jsonMatcher = null, string because = "", params object[] becauseArgs)
    {
        return WithBodyAsJson(jsonMatcher ?? new JsonMatcher(body), because, becauseArgs);
    }

    [CustomAssertion]
    public AndConstraint<WireMockAdminApiAssertions> WithBodyAsJson(string body, IJsonMatcher? jsonMatcher = null, string because = "", params object[] becauseArgs)
    {
        return WithBodyAsJson(jsonMatcher ?? new JsonMatcher(body), because, becauseArgs);
    }

    [CustomAssertion]
    public AndConstraint<WireMockAdminApiAssertions> WithBodyAsJson(IObjectMatcher matcher, string because = "", params object[] becauseArgs)
    {
        var (filter, condition) = BuildFilterAndCondition(r => r.BodyAsJson, matcher);

        return ExecuteAssertionWithBodyAsIObjectMatcher(matcher, because, becauseArgs, condition, filter, r => r.BodyAsJson);
    }

    [CustomAssertion]
    public AndConstraint<WireMockAdminApiAssertions> WithBodyAsBytes(byte[] body, string because = "", params object[] becauseArgs)
    {
        return WithBodyAsBytes(new ExactObjectMatcher(body), because, becauseArgs);
    }

    [CustomAssertion]
    public AndConstraint<WireMockAdminApiAssertions> WithBodyAsBytes(ExactObjectMatcher matcher, string because = "", params object[] becauseArgs)
    {
        var (filter, condition) = BuildFilterAndCondition(r => r.BodyAsBytes, matcher);

        return ExecuteAssertionWithBodyAsIObjectMatcher(matcher, because, becauseArgs, condition, filter, r => r.BodyAsBytes);
    }

    private AndConstraint<WireMockAdminApiAssertions> ExecuteAssertionWithBodyStringMatcher(
        IStringMatcher matcher,
        string because,
        object[] becauseArgs,
        Func<IReadOnlyList<LogRequestModel>, bool> condition,
        Func<IReadOnlyList<LogRequestModel>, IReadOnlyList<LogRequestModel>> filter,
        Func<LogRequestModel, object?> expression
    )
    {
        chain
            .BecauseOf(because, becauseArgs)
            .Given(() => RequestMessages)
            .ForCondition(requests => CallsCount == 0 || requests.Any())
            .FailWith(
                MessageFormatNoCalls,
                FormatBody(matcher.GetPatterns())
            )
            .Then
            .ForCondition(condition)
            .FailWith(
                MessageFormat,
                _ => FormatBody(matcher.GetPatterns()),
                requests => FormatBodies(requests.Select(expression))
            );

        FilterRequestMessages(filter);

        return new AndConstraint<WireMockAdminApiAssertions>(this);
    }

    private AndConstraint<WireMockAdminApiAssertions> ExecuteAssertionWithBodyAsIObjectMatcher(
        IObjectMatcher matcher,
        string because,
        object[] becauseArgs,
        Func<IReadOnlyList<LogRequestModel>, bool> condition,
        Func<IReadOnlyList<LogRequestModel>, IReadOnlyList<LogRequestModel>> filter,
        Func<LogRequestModel, object?> expression
    )
    {
        chain
            .BecauseOf(because, becauseArgs)
            .Given(() => RequestMessages)
            .ForCondition(requests => CallsCount == 0 || requests.Any())
            .FailWith(
                MessageFormatNoCalls,
                FormatBody(matcher.Value)
            )
            .Then
            .ForCondition(condition)
            .FailWith(
                MessageFormat,
                _ => FormatBody(matcher.Value),
                requests => FormatBodies(requests.Select(expression))
            );

        FilterRequestMessages(filter);

        return new AndConstraint<WireMockAdminApiAssertions>(this);
    }

    private static string? FormatBody(object? body)
    {
        if (body == null)
        {
            return null;
        }

        if (body is string str)
        {
            return str;
        }

        if (body is AnyOf<string, StringPattern>[] stringPatterns)
        {
            return FormatBodies(stringPatterns.Select(p => p.GetPattern()));
        }

        if (body is byte[] bytes)
        {
            return $"byte[{bytes.Length}] {{...}}";
        }

        if (body is JToken jToken)
        {
            return jToken.ToString(Formatting.None);
        }

        // System.IO.FileNotFoundException : Could not load file or assembly 'System.Text.Json, Version=10.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51'. The system cannot find the file specified.
        var typeName = body.GetType().FullName;
        if (typeName == "System.Text.Json.JsonElement")
        {
            return ((dynamic)body).GetRawText();
        }

        if (typeName == "System.Text.Json.JsonDocument")
        {
            return ((dynamic)body).RootElement.GetRawText();
        }

        return JToken.FromObject(body).ToString(Formatting.None);
    }

    private static string? FormatBodies(IEnumerable<object?> bodies)
    {
        var valueAsArray = bodies as object[] ?? bodies.ToArray();
        return valueAsArray.Length == 1 ? FormatBody(valueAsArray[0]) : $"[ {string.Join(", ", valueAsArray.Select(FormatBody))} ]";
    }
}
