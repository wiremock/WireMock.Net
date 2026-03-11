// Copyright © WireMock.Net

using System.Diagnostics;
using WireMock.Logging;

namespace WireMock.Owin;

internal interface IWireMockMiddlewareLogger
{
    void LogRequestAndResponse(bool logRequest, RequestMessage request, IResponseMessage? response, MappingMatcherResult? match, MappingMatcherResult? partialMatch, Activity? activity);

    void LogLogEntry(LogEntry entry, bool addRequest);
}