// Copyright © WireMock.Net

using System.Diagnostics;

namespace WireMock.Owin;

internal interface IWireMockMiddlewareLogger
{
    void Log(bool logRequest, RequestMessage request, IResponseMessage? response, MappingMatcherResult? match, MappingMatcherResult? partialMatch, Activity? activity);
}