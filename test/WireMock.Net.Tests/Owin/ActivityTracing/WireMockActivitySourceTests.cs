// Copyright © WireMock.Net

using System.Diagnostics;
using FluentAssertions;
using Moq;
using WireMock.Logging;
using WireMock.Matchers.Request;
using WireMock.Models;
using WireMock.Owin.ActivityTracing;
using WireMock.Settings;
using WireMock.Util;

namespace WireMock.Net.Tests.Owin.ActivityTracing;

public class WireMockActivitySourceTests
{
    [Fact]
    public void EnrichWithRequest_ShouldSetRequestTagsAndBody_WhenEnabled()
    {
        // Arrange
        using var activity = new Activity("test").Start();
        var request = new RequestMessage(
            new UrlDetails("http://localhost/api/orders"),
            "POST",
            "127.0.0.1",
            new BodyData { BodyAsString = "payload" });

        var options = new ActivityTracingOptions
        {
            RecordRequestBody = true
        };

        // Act
        WireMockActivitySource.EnrichWithRequest(activity, request, options);

        // Assert
        activity.GetTagItem(WireMockSemanticConventions.HttpMethod).Should().Be("POST");
        activity.GetTagItem(WireMockSemanticConventions.HttpUrl).Should().Be("http://localhost/api/orders");
        activity.GetTagItem(WireMockSemanticConventions.HttpPath).Should().Be("/api/orders");
        activity.GetTagItem(WireMockSemanticConventions.HttpHost).Should().Be("localhost");
        activity.GetTagItem(WireMockSemanticConventions.ClientAddress).Should().Be("127.0.0.1");
        activity.GetTagItem(WireMockSemanticConventions.RequestBody).Should().Be("payload");
    }

    [Fact]
    public void EnrichWithResponse_ShouldSetStatusAndBody_WhenEnabled()
    {
        // Arrange
        using var activity = new Activity("test").Start();
        var response = new ResponseMessage
        {
            StatusCode = 200,
            BodyData = new BodyData { BodyAsString = "ok" }
        };

        var options = new ActivityTracingOptions
        {
            RecordResponseBody = true
        };

        // Act
        WireMockActivitySource.EnrichWithResponse(activity, response, options);

        // Assert
        activity.GetTagItem(WireMockSemanticConventions.HttpStatusCode).Should().Be(200);
        activity.GetTagItem("otel.status_code").Should().Be("OK");
        activity.GetTagItem(WireMockSemanticConventions.ResponseBody).Should().Be("ok");
    }

    [Fact]
    public void EnrichWithResponse_ShouldSetErrorStatus_ForNonSuccess()
    {
        // Arrange
        using var activity = new Activity("test").Start();
        var response = new ResponseMessage
        {
            StatusCode = 500
        };

        // Act
        WireMockActivitySource.EnrichWithResponse(activity, response, new ActivityTracingOptions());

        // Assert
        activity.GetTagItem(WireMockSemanticConventions.HttpStatusCode).Should().Be(500);
        activity.GetTagItem("otel.status_code").Should().Be("ERROR");
    }

    [Fact]
    public void EnrichWithRequest_ShouldNotRecordBody_WhenDisabled()
    {
        // Arrange
        using var activity = new Activity("test").Start();
        var request = new RequestMessage(
            new UrlDetails("http://localhost/api/orders"),
            "POST",
            "127.0.0.1",
            new BodyData { BodyAsString = "payload" });

        var options = new ActivityTracingOptions
        {
            RecordRequestBody = false
        };

        // Act
        WireMockActivitySource.EnrichWithRequest(activity, request, options);

        // Assert
        activity.GetTagItem(WireMockSemanticConventions.RequestBody).Should().BeNull();
    }

    [Fact]
    public void EnrichWithResponse_ShouldNotRecordBody_WhenDisabled()
    {
        // Arrange
        using var activity = new Activity("test").Start();
        var response = new ResponseMessage
        {
            StatusCode = 200,
            BodyData = new BodyData { BodyAsString = "ok" }
        };

        var options = new ActivityTracingOptions
        {
            RecordResponseBody = false
        };

        // Act
        WireMockActivitySource.EnrichWithResponse(activity, response, options);

        // Assert
        activity.GetTagItem(WireMockSemanticConventions.ResponseBody).Should().BeNull();
    }

    [Fact]
    public void EnrichWithLogEntry_ShouldSkipMatchDetails_WhenDisabled()
    {
        // Arrange
        using var activity = new Activity("test").Start();
        var request = new RequestMessage(
            new UrlDetails("http://localhost/api/orders"),
            "GET",
            "127.0.0.1");
        var response = new ResponseMessage { StatusCode = 200 };

        var matchResult = new Mock<IRequestMatchResult>();
        matchResult.SetupGet(r => r.IsPerfectMatch).Returns(true);
        matchResult.SetupGet(r => r.TotalScore).Returns(1.0);

        var logEntry = new LogEntry
        {
            Guid = Guid.NewGuid(),
            RequestMessage = request,
            ResponseMessage = response,
            RequestMatchResult = matchResult.Object,
            MappingGuid = Guid.NewGuid(),
            MappingTitle = "test-mapping"
        };

        var options = new ActivityTracingOptions
        {
            RecordMatchDetails = false
        };

        // Act
        WireMockActivitySource.EnrichWithLogEntry(activity, logEntry, options);

        // Assert
        activity.GetTagItem(WireMockSemanticConventions.RequestGuid).Should().Be(logEntry.Guid.ToString());
        activity.Tags.Should().NotContain(tag => tag.Key == WireMockSemanticConventions.MappingGuid);
        activity.Tags.Should().NotContain(tag => tag.Key == WireMockSemanticConventions.MappingTitle);
        activity.Tags.Should().NotContain(tag => tag.Key == WireMockSemanticConventions.MatchScore);
    }

    [Fact]
    public void RecordException_ShouldSetExceptionTags()
    {
        // Arrange
        using var activity = new Activity("test").Start();
        var exception = new InvalidOperationException("boom");

        // Act
        WireMockActivitySource.RecordException(activity, exception);

        // Assert
        activity.GetTagItem("otel.status_code").Should().Be("ERROR");
        activity.GetTagItem("otel.status_description").Should().Be("boom");
        activity.GetTagItem("exception.type").Should().Be(typeof(InvalidOperationException).FullName);
        activity.GetTagItem("exception.message").Should().Be("boom");
        activity.GetTagItem("exception.stacktrace").Should().NotBeNull();
    }
}