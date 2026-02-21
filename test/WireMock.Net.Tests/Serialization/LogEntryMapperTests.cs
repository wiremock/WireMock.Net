// Copyright © WireMock.Net

using WireMock.Logging;
using WireMock.Models;
using WireMock.Net.Tests.VerifyExtensions;
using WireMock.Owin;
using WireMock.ResponseBuilders;
using WireMock.Serialization;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.Net.Tests.Serialization;

public class LogEntryMapperTests
{
    private static readonly VerifySettings VerifySettings = new();
    static LogEntryMapperTests()
    {
        VerifySettings.Init();
    }

    private readonly IWireMockMiddlewareOptions _options = new WireMockMiddlewareOptions();

    private readonly LogEntryMapper _sut;

    public LogEntryMapperTests()
    {
        _sut = new LogEntryMapper(_options);
    }

    [Fact]
    public Task LogEntryMapper_Map_LogEntry_Check_BodyTypeBytes()
    {
        // Assign
        var logEntry = new LogEntry
        {
            RequestMessage = new RequestMessage(
                new UrlDetails("http://localhost"),
                "post",
                "::1",
                new BodyData
                {
                    DetectedBodyType = BodyType.Bytes,
                    BodyAsBytes = new byte[] { 0 }
                }
            ),
            ResponseMessage = new ResponseMessage
            {
                BodyData = new BodyData
                {
                    DetectedBodyType = BodyType.Bytes,
                    BodyAsBytes = new byte[] { 0 }
                }
            }
        };

        // Act
        var result = _sut.Map(logEntry);

        // Verify
        return Verify(result, VerifySettings);
    }

    [Fact]
    public Task LogEntryMapper_Map_LogEntry_Check_ResponseBodyTypeFile()
    {
        // Assign
        var logEntry = new LogEntry
        {
            RequestMessage = new RequestMessage(new UrlDetails("http://localhost"), "get", "::1"),
            ResponseMessage = new ResponseMessage
            {
                BodyData = new BodyData
                {
                    DetectedBodyType = BodyType.File,
                    BodyAsFile = "test"
                }
            }
        };

        // Act
        var result = _sut.Map(logEntry);

        // Verify
        return Verify(result, VerifySettings);
    }

    [Fact]
    public Task LogEntryMapper_Map_LogEntry_WithFault()
    {
        // Assign
        var logEntry = new LogEntry
        {
            RequestMessage = new RequestMessage(new UrlDetails("http://localhost"), "get", "::1"),
            ResponseMessage = new ResponseMessage
            {
                BodyData = new BodyData
                {
                    DetectedBodyType = BodyType.File,
                    BodyAsFile = "test"
                },
                FaultType = FaultType.EMPTY_RESPONSE,
                FaultPercentage = 0.5
            }
        };

        // Act
        var result = _sut.Map(logEntry);

        // Verify
        return Verify(result, VerifySettings);
    }

    [Fact]
    public Task LogEntryMapper_Map_LogEntry_WhenFuncIsUsed_And_DoNotSaveDynamicResponseInLogEntry_Is_True_Should_NotSave_StringResponse()
    {
        // Assign
        var options = new WireMockMiddlewareOptions
        {
            DoNotSaveDynamicResponseInLogEntry = true
        };
        var isFuncUsed = "Func<IRequestMessage, string>";
        var logEntry = new LogEntry
        {
            RequestMessage = new RequestMessage(
                new UrlDetails("http://localhost"),
                "post",
                "::1",
                new BodyData
                {
                    DetectedBodyType = BodyType.Bytes,
                    BodyAsBytes = new byte[] { 0 }
                }
            ),
            ResponseMessage = new ResponseMessage
            {
                BodyData = new BodyData
                {
                    DetectedBodyType = BodyType.String,
                    BodyAsString = "test",
                    IsFuncUsed = isFuncUsed
                }
            }
        };

        // Act
        var result = new LogEntryMapper(options).Map(logEntry);

        // Verify
        return Verify(result, VerifySettings);
    }
}