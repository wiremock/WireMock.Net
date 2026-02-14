// Copyright © WireMock.Net

using System.Net.Http;
using AnyOfTypes;
using FluentAssertions;
using WireMock.Matchers;
using WireMock.Models;

namespace WireMock.Net.Tests.Matchers;

public class FormUrlEncodedMatcherTest
{
    [Theory]
    [InlineData("*=*")]
    [InlineData("name=John Doe")]
    [InlineData("name=*")]
    [InlineData("*=John Doe")]
    [InlineData("email=johndoe@example.com")]
    [InlineData("email=*")]
    [InlineData("*=johndoe@example.com")]
    [InlineData("name=John Doe", "email=johndoe@example.com")]
    [InlineData("name=John Doe", "email=*")]
    [InlineData("name=*", "email=*")]
    [InlineData("*=John Doe", "*=johndoe@example.com")]
    public async Task FormUrlEncodedMatcher_IsMatch(params string[] patterns)
    {
        // Arrange
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("name", "John Doe"),
            new KeyValuePair<string, string>("email", "johndoe@example.com")
        });
        var contentAsString = await content.ReadAsStringAsync();

        var matcher = new FormUrlEncodedMatcher(patterns.Select(p => new AnyOf<string, StringPattern>(p)).ToArray());

        // Act
        var score = matcher.IsMatch(contentAsString).IsPerfect();

        // Assert
        score.Should().BeTrue();
    }

    [Theory]
    [InlineData(false, "name=John Doe")]
    [InlineData(false, "name=*")]
    [InlineData(false, "*=John Doe")]
    [InlineData(false, "email=johndoe@example.com")]
    [InlineData(false, "email=*")]
    [InlineData(false, "*=johndoe@example.com")]
    [InlineData(true, "name=John Doe", "email=johndoe@example.com")]
    [InlineData(true, "name=John Doe", "email=*")]
    [InlineData(true, "name=*", "email=*")]
    [InlineData(true, "*=John Doe", "*=johndoe@example.com")]
    [InlineData(true, "*=*")]
    public async Task FormUrlEncodedMatcher_IsMatch_And(bool expected, params string[] patterns)
    {
        // Arrange
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("name", "John Doe"),
            new KeyValuePair<string, string>("email", "johndoe@example.com")
        });
        var contentAsString = await content.ReadAsStringAsync();

        var matcher = new FormUrlEncodedMatcher(patterns.Select(p => new AnyOf<string, StringPattern>(p)).ToArray(), true, MatchOperator.And);

        // Act
        var score = matcher.IsMatch(contentAsString).IsPerfect();

        // Assert
        score.Should().Be(expected);
    }

    [Fact]
    public async Task FormUrlEncodedMatcher_IsMatch_And_MatchAllProperties()
    {
        // Arrange
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("name", "John Doe"),
            new KeyValuePair<string, string>("email", "johndoe@example.com")
        });
        var contentAsString = await content.ReadAsStringAsync();

        // The expectation is that the matcher requires all properties to be present in the content.
        var matcher = new FormUrlEncodedMatcher(["name=*", "email=*", "required=*"], matchOperator: MatchOperator.And);

        // Act
        var score = matcher.IsMatch(contentAsString).IsPerfect();

        // Assert
        score.Should().BeFalse();
    }
}