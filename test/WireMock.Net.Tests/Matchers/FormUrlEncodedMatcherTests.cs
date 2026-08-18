// Copyright © WireMock.Net

using System.Net.Http;
using WireMock.Extensions;
using WireMock.Matchers;

namespace WireMock.Net.Tests.Matchers;

public class FormUrlEncodedMatcherTest
{
    private readonly CancellationToken _ct = TestContext.Current.CancellationToken;

    [Theory]
    [InlineData(true, "*=*")]
    [InlineData(true, "name=John Doe")]
    [InlineData(false, "name=Stef")]
    [InlineData(false, "name=John Doe&name=Stef")]
    [InlineData(true, "name=*")]
    [InlineData(true, "*=John Doe")]
    [InlineData(false, "*=Stef")]
    [InlineData(false, "*=John Doe&*=Stef")]
    [InlineData(true, "email=johndoe@example.com")]
    [InlineData(true, "email=*")]
    [InlineData(true, "*=johndoe@example.com")]
    [InlineData(true, "name=John Doe", "email=johndoe@example.com")]
    [InlineData(true, "name=John Doe", "email=*")]
    [InlineData(true, "name=John Doe&name=Stef", "email=*")]
    [InlineData(true, "name=Stef", "email=*")]
    [InlineData(true, "name=*", "email=*")]
    [InlineData(true, "*=John Doe", "*=johndoe@example.com")]
    [InlineData(true, "*=Stef", "*=johndoe@example.com")]
    [InlineData(true, "name=John Doe&name=Stef", "*=johndoe@example.com")]
    public async Task FormUrlEncodedMatcher_IsMatch_Single_Or(bool expected, params string[] patterns)
    {
        // Arrange
        var content = new FormUrlEncodedContent(
        [
            new KeyValuePair<string, string>("name", "John Doe"),
            new KeyValuePair<string, string>("email", "johndoe@example.com")
        ]);
        var contentAsString = await content.ReadAsStringAsync(_ct);

        var matcher = new FormUrlEncodedMatcher(patterns.ToAnyOfPatterns());

        // Act
        var score = matcher.IsMatch(contentAsString).IsPerfect();

        // Assert
        score.Should().Be(expected);
    }

    [Theory]
    [InlineData(true, "*=*")]
    [InlineData(false, "name=John Doe")]
    [InlineData(false, "name=Stef")]
    [InlineData(true, "name=John Doe&name=Stef")]
    [InlineData(true, "name=*")]
    [InlineData(false, "*=John Doe")]
    [InlineData(false, "*=Stef")]
    [InlineData(true, "*=John Doe&*=Stef")]
    [InlineData(true, "email=johndoe@example.com")]
    [InlineData(true, "email=*")]
    [InlineData(true, "*=johndoe@example.com")]
    [InlineData(true, "name=John Doe", "email=johndoe@example.com")]
    [InlineData(true, "name=John Doe", "email=*")]
    [InlineData(true, "name=John Doe&name=Stef", "email=*")]
    [InlineData(true, "name=Stef", "email=*")]
    [InlineData(true, "name=*", "email=*")]
    [InlineData(true, "*=John Doe", "*=johndoe@example.com")]
    [InlineData(true, "*=Stef", "*=johndoe@example.com")]
    [InlineData(true, "name=John Doe&name=Stef", "*=johndoe@example.com")]
    public async Task FormUrlEncodedMatcher_IsMatch_Multiple_Or(bool expected, params string[] patterns)
    {
        // Arrange
        var content = new FormUrlEncodedContent(
        [
            new KeyValuePair<string, string>("name", "John Doe"),
            new KeyValuePair<string, string>("name", "Stef"),
            new KeyValuePair<string, string>("email", "johndoe@example.com")
        ]);
        var contentAsString = await content.ReadAsStringAsync(_ct);

        var matcher = new FormUrlEncodedMatcher(patterns.ToAnyOfPatterns());

        // Act
        var score = matcher.IsMatch(contentAsString).IsPerfect();

        // Assert
        score.Should().Be(expected);
    }

    [Theory]
    [InlineData(true, "*=*")]
    [InlineData(false, "name=John Doe")]
    [InlineData(false, "name=Stef")]
    [InlineData(false, "name=John Doe&name=Stef")]
    [InlineData(false, "name=*")]
    [InlineData(false, "*=John Doe")]
    [InlineData(false, "*=Stef")]
    [InlineData(false, "*=John Doe&*=Stef")]
    [InlineData(false, "email=johndoe@example.com")]
    [InlineData(false, "email=*")]
    [InlineData(false, "*=johndoe@example.com")]
    [InlineData(true, "name=John Doe", "email=johndoe@example.com")]
    [InlineData(true, "name=John Doe", "email=*")]
    [InlineData(false, "name=John Doe&name=Stef", "email=*")]
    [InlineData(false, "name=Stef", "email=*")]
    [InlineData(true, "name=*", "email=*")]
    [InlineData(true, "*=John Doe", "*=johndoe@example.com")]
    [InlineData(false, "*=Stef", "*=johndoe@example.com")]
    [InlineData(false, "name=John Doe&name=Stef", "*=johndoe@example.com")]
    public async Task FormUrlEncodedMatcher_IsMatch_Single_And(bool expected, params string[] patterns)
    {
        // Arrange
        var content = new FormUrlEncodedContent(
        [
            new KeyValuePair<string, string>("name", "John Doe"),
            new KeyValuePair<string, string>("email", "johndoe@example.com")
        ]);
        var contentAsString = await content.ReadAsStringAsync(_ct);

        var matcher = new FormUrlEncodedMatcher(patterns.ToAnyOfPatterns(), true, MatchOperator.And);

        // Act
        var score = matcher.IsMatch(contentAsString).IsPerfect();

        // Assert
        score.Should().Be(expected);
    }

    [Theory]
    [InlineData(true, "*=*")]
    [InlineData(false, "name=John Doe")]
    [InlineData(false, "name=Stef")]
    [InlineData(false, "name=John Doe&name=Stef")]
    [InlineData(false, "name=*")]
    [InlineData(false, "*=John Doe")]
    [InlineData(false, "*=Stef")]
    [InlineData(false, "*=John Doe&*=Stef")]
    [InlineData(false, "email=johndoe@example.com")]
    [InlineData(false, "email=*")]
    [InlineData(false, "*=johndoe@example.com")]
    [InlineData(false, "name=John Doe", "email=johndoe@example.com")]
    [InlineData(false, "name=John Doe", "email=*")]
    [InlineData(true, "name=John Doe&name=Stef", "email=*")]
    [InlineData(false, "name=Stef", "email=*")]
    [InlineData(true, "name=*", "email=*")]
    [InlineData(false, "*=John Doe", "*=johndoe@example.com")]
    [InlineData(false, "*=Stef", "*=johndoe@example.com")]
    [InlineData(true, "name=John Doe&name=Stef", "*=johndoe@example.com")]
    public async Task FormUrlEncodedMatcher_IsMatch_Multiple_And(bool expected, params string[] patterns)
    {
        // Arrange
        var content = new FormUrlEncodedContent(
        [
            new KeyValuePair<string, string>("name", "John Doe"),
            new KeyValuePair<string, string>("name", "Stef"),
            new KeyValuePair<string, string>("email", "johndoe@example.com")
        ]);
        var contentAsString = await content.ReadAsStringAsync(_ct);

        var matcher = new FormUrlEncodedMatcher(patterns.ToAnyOfPatterns(), true, MatchOperator.And);

        // Act
        var score = matcher.IsMatch(contentAsString).IsPerfect();

        // Assert
        score.Should().Be(expected);
    }

    [Fact]
    public async Task FormUrlEncodedMatcher_IsMatch_And_MatchAllProperties_Test_1()
    {
        // Arrange
        var content = new FormUrlEncodedContent(
        [
            new KeyValuePair<string, string>("name", "John Doe"),
            new KeyValuePair<string, string>("name", "Stef"),
            new KeyValuePair<string, string>("email", "johndoe@example.com")
        ]);
        var contentAsString = await content.ReadAsStringAsync(_ct);

        // The expectation is that the matcher requires all properties to be present in the content.
        var matcher = new FormUrlEncodedMatcher(["name=*", "email=*", "required=*"], matchOperator: MatchOperator.And);

        // Act
        var score = matcher.IsMatch(contentAsString).IsPerfect();

        // Assert
        score.Should().BeFalse();
    }

    [Fact]
    public async Task FormUrlEncodedMatcher_IsMatch_And_MatchAllProperties_Test_2()
    {
        // Arrange
        var content = new FormUrlEncodedContent(
        [
            new KeyValuePair<string, string>("name", "John Doe"),
            new KeyValuePair<string, string>("name", "Stef"),
            new KeyValuePair<string, string>("email", "johndoe@example.com")
        ]);
        var contentAsString = await content.ReadAsStringAsync(_ct);

        var matcher = new FormUrlEncodedMatcher(["name=*", "email=*", "email=x"], matchOperator: MatchOperator.And);

        // Act
        var score = matcher.IsMatch(contentAsString).IsPerfect();

        // Assert
        score.Should().BeFalse();
    }
}