// Copyright © WireMock.Net

using System.Diagnostics;
using System.Net.WebSockets;
using AwesomeAssertions;
using Moq;
using WireMock.Logging;
using WireMock.Matchers.Request;
using WireMock.Models;
using WireMock.Owin.ActivityTracing;
using WireMock.Settings;
using WireMock.Util;
using WireMock.WebSockets;

namespace WireMock.Net.Tests.Owin.ActivityTracing;

public class WireMockActivitySourceTests : IDisposable
{
    private readonly ActivityListener _activityListener;

    public WireMockActivitySourceTests()
    {
        // Set up ActivityListener for tests
        _activityListener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == WireMockActivitySource.SourceName,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData
        };

        ActivitySource.AddActivityListener(_activityListener);
    }

    public void Dispose()
    {
        _activityListener?.Dispose();
    }

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
        activity.GetTagItem(WireMockSemanticConventions.OtelStatusCode).Should().Be("OK");
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
        activity.GetTagItem(WireMockSemanticConventions.OtelStatusCode).Should().Be("ERROR");
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
        var exception = new InvalidOperationException("Yes, Rico; Kaboom.");

        // Act
        WireMockActivitySource.RecordException(activity, exception);

        // Assert
        activity.GetTagItem(WireMockSemanticConventions.OtelStatusCode).Should().Be("ERROR");
        activity.GetTagItem("otel.status_description").Should().Be("Yes, Rico; Kaboom.");
        activity.GetTagItem("exception.type").Should().Be(typeof(InvalidOperationException).FullName);
        activity.GetTagItem("exception.message").Should().Be("Yes, Rico; Kaboom.");
        activity.GetTagItem("exception.stacktrace").Should().NotBeNull();
    }

    [Fact]
    public void StartRequestActivity_ShouldCreateActivity_WithCorrectDisplayName()
    {
        // Arrange
        var requestMethod = "POST";
        var requestPath = "/api/users";

        // Act
        using var activity = WireMockActivitySource.StartRequestActivity(requestMethod, requestPath);

        // Assert
        activity.Should().NotBeNull();
        activity.DisplayName.Should().Be("WireMock POST /api/users");
        activity.Kind.Should().Be(ActivityKind.Server);
    }

    [Fact]
    public void StartWebSocketMessageActivity_ShouldCreateActivity_WithCorrectName()
    {
        // Arrange
        var mappingGuid = Guid.NewGuid();
        var direction = WebSocketMessageDirection.Receive;

        // Act
        using var activity = WireMockActivitySource.StartWebSocketMessageActivity(direction, mappingGuid);

        // Assert
        activity.Should().NotBeNull();
        activity.DisplayName.Should().Be("WireMock WebSocket receive");
        activity.Kind.Should().Be(ActivityKind.Server);
    }

    [Fact]
    public void StartWebSocketMessageActivity_ShouldSetMappingGuidTag()
    {
        // Arrange
        var mappingGuid = Guid.NewGuid();
        var direction = WebSocketMessageDirection.Send;

        // Act
        using var activity = WireMockActivitySource.StartWebSocketMessageActivity(direction, mappingGuid);

        // Assert
        activity.Should().NotBeNull();
        activity.GetTagItem(WireMockSemanticConventions.MappingGuid).Should().Be(mappingGuid.ToString());
    }

    [Fact]
    public void StartWebSocketMessageActivity_ShouldCreateActivityForSendDirection()
    {
        // Arrange
        var mappingGuid = Guid.NewGuid();
        var direction = WebSocketMessageDirection.Send;

        // Act
        using var activity = WireMockActivitySource.StartWebSocketMessageActivity(direction, mappingGuid);

        // Assert
        activity.Should().NotBeNull();
        activity.DisplayName.Should().Be("WireMock WebSocket send");
    }

    [Fact]
    public void StartWebSocketMessageActivity_ShouldCreateActivityWithListenerConfigured()
    {
        // Arrange
        var mappingGuid = Guid.NewGuid();
        var direction = WebSocketMessageDirection.Receive;

        // Act - ActivityListener is configured in test constructor
        using var activity = WireMockActivitySource.StartWebSocketMessageActivity(direction, mappingGuid);

        // Assert - activity should be created since listener is active
        activity.Should().NotBeNull();
        activity.DisplayName.Should().Be("WireMock WebSocket receive");
        activity.GetTagItem(WireMockSemanticConventions.MappingGuid).Should().Be(mappingGuid.ToString());
    }

    [Fact]
    public void EnrichWithWebSocketMessage_ShouldSetMessageTypeTag()
    {
        // Arrange
        using var activity = new Activity("test").Start();
        var messageType = WebSocketMessageType.Text;

        // Act
        WireMockActivitySource.EnrichWithWebSocketMessage(
            activity,
            messageType,
            messageSize: 100,
            endOfMessage: true
        );

        // Assert
        activity.GetTagItem(WireMockSemanticConventions.WebSocketMessageType).Should().Be("Text");
    }

    [Fact]
    public void EnrichWithWebSocketMessage_ShouldSetMessageSizeTag()
    {
        // Arrange
        using var activity = new Activity("test").Start();

        // Act
        WireMockActivitySource.EnrichWithWebSocketMessage(
            activity,
            WebSocketMessageType.Binary,
            messageSize: 256,
            endOfMessage: true
        );

        // Assert
        activity.GetTagItem(WireMockSemanticConventions.WebSocketMessageSize).Should().Be(256);
    }

    [Fact]
    public void EnrichWithWebSocketMessage_ShouldSetEndOfMessageTag()
    {
        // Arrange
        using var activity = new Activity("test").Start();

        // Act
        WireMockActivitySource.EnrichWithWebSocketMessage(
            activity,
            WebSocketMessageType.Text,
            messageSize: 50,
            endOfMessage: false
        );

        // Assert
        activity.GetTagItem(WireMockSemanticConventions.WebSocketEndOfMessage).Should().Be(false);
    }

    [Fact]
    public void EnrichWithWebSocketMessage_ShouldSetOkStatus()
    {
        // Arrange
        using var activity = new Activity("test").Start();

        // Act
        WireMockActivitySource.EnrichWithWebSocketMessage(
            activity,
            WebSocketMessageType.Text,
            messageSize: 100,
            endOfMessage: true
        );

        // Assert
        activity.GetTagItem(WireMockSemanticConventions.OtelStatusCode).Should().Be("OK");
    }

    [Fact]
    public void EnrichWithWebSocketMessage_ShouldRecordTextContent_WhenEnabled()
    {
        // Arrange
        using var activity = new Activity("test").Start();
        var options = new ActivityTracingOptions { RecordRequestBody = true };
        var textContent = "Hello WebSocket";

        // Act
        WireMockActivitySource.EnrichWithWebSocketMessage(
            activity,
            WebSocketMessageType.Text,
            messageSize: textContent.Length,
            endOfMessage: true,
            textContent: textContent,
            options: options
        );

        // Assert
        activity.GetTagItem(WireMockSemanticConventions.WebSocketMessageContent).Should().Be(textContent);
    }

    [Fact]
    public void EnrichWithWebSocketMessage_ShouldNotRecordTextContent_WhenDisabled()
    {
        // Arrange
        using var activity = new Activity("test").Start();
        var options = new ActivityTracingOptions { RecordRequestBody = false };

        // Act
        WireMockActivitySource.EnrichWithWebSocketMessage(
            activity,
            WebSocketMessageType.Text,
            messageSize: 100,
            endOfMessage: true,
            textContent: "Hello WebSocket",
            options: options
        );

        // Assert
        activity.GetTagItem(WireMockSemanticConventions.WebSocketMessageContent).Should().BeNull();
    }

    [Fact]
    public void EnrichWithWebSocketMessage_ShouldNotRecordBinaryContent()
    {
        // Arrange
        using var activity = new Activity("test").Start();
        var options = new ActivityTracingOptions { RecordRequestBody = true };

        // Act
        WireMockActivitySource.EnrichWithWebSocketMessage(
            activity,
            WebSocketMessageType.Binary,
            messageSize: 100,
            endOfMessage: true,
            textContent: "should not record",
            options: options
        );

        // Assert
        activity.GetTagItem(WireMockSemanticConventions.WebSocketMessageContent).Should().BeNull();
    }

    [Fact]
    public void EnrichWithWebSocketMessage_ShouldHandleNullActivity()
    {
        // Arrange & Act - should not throw
        WireMockActivitySource.EnrichWithWebSocketMessage(
            null,
            WebSocketMessageType.Text,
            messageSize: 100,
            endOfMessage: true
        );

        // Assert - no exception thrown
    }

    [Fact]
    public void EnrichWithWebSocketMessage_ShouldHandleClosedMessageType()
    {
        // Arrange
        using var activity = new Activity("test").Start();

        // Act
        WireMockActivitySource.EnrichWithWebSocketMessage(
            activity,
            WebSocketMessageType.Close,
            messageSize: 0,
            endOfMessage: true
        );

        // Assert
        activity.GetTagItem(WireMockSemanticConventions.WebSocketMessageType).Should().Be("Close");
        activity.GetTagItem(WireMockSemanticConventions.WebSocketMessageSize).Should().Be(0);
    }
}