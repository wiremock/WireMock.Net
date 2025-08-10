// Copyright © WireMock.Net

using System;
using System.Threading.Tasks;
using System.Linq;
using System.Net;
using Stef.Validation;
using WireMock.Logging;
using WireMock.Matchers;
using WireMock.Http;
using WireMock.Owin.Mappers;
using WireMock.Serialization;
using WireMock.ResponseBuilders;
using WireMock.Settings;
using System.Collections.Generic;
using WireMock.Constants;
using WireMock.Exceptions;
using WireMock.Util;
#if !USE_ASPNETCORE
using IContext = Microsoft.Owin.IOwinContext;
using OwinMiddleware = Microsoft.Owin.OwinMiddleware;
using Next = Microsoft.Owin.OwinMiddleware;
#else
using OwinMiddleware = System.Object;
using IContext = Microsoft.AspNetCore.Http.HttpContext;
using Next = Microsoft.AspNetCore.Http.RequestDelegate;
#endif

namespace WireMock.Owin
{
    internal class WireMockMiddleware : OwinMiddleware
    {
        private readonly object _lock = new();
        private static readonly Task CompletedTask = Task.FromResult(false);

        private readonly IWireMockMiddlewareOptions _options;
        private readonly IOwinRequestMapper _requestMapper;
        private readonly IOwinResponseMapper _responseMapper;
        private readonly IMappingMatcher _mappingMatcher;
        private readonly LogEntryMapper _logEntryMapper;
        private readonly IGuidUtils _guidUtils;

#if !USE_ASPNETCORE
        public WireMockMiddleware(
            Next next,
            IWireMockMiddlewareOptions options,
            IOwinRequestMapper requestMapper,
            IOwinResponseMapper responseMapper,
            IMappingMatcher mappingMatcher,
            IGuidUtils guidUtils
        ) : base(next)
        {
            _options = Guard.NotNull(options);
            _requestMapper = Guard.NotNull(requestMapper);
            _responseMapper = Guard.NotNull(responseMapper);
            _mappingMatcher = Guard.NotNull(mappingMatcher);
            _logEntryMapper = new LogEntryMapper(options);
            _guidUtils = Guard.NotNull(guidUtils);
        }
#else
        public WireMockMiddleware(
            Next next,
            IWireMockMiddlewareOptions options,
            IOwinRequestMapper requestMapper,
            IOwinResponseMapper responseMapper,
            IMappingMatcher mappingMatcher,
            IGuidUtils guidUtils
        )
        {
            _options = Guard.NotNull(options);
            _requestMapper = Guard.NotNull(requestMapper);
            _responseMapper = Guard.NotNull(responseMapper);
            _mappingMatcher = Guard.NotNull(mappingMatcher);
            _logEntryMapper = new LogEntryMapper(options);
            _guidUtils = Guard.NotNull(guidUtils);
        }
#endif

#if !USE_ASPNETCORE
        public override Task Invoke(IContext ctx)
#else
        public Task Invoke(IContext ctx)
#endif
        {
            if (_options.HandleRequestsSynchronously.GetValueOrDefault(false))
            {
                lock (_lock)
                {
                    return InvokeInternalAsync(ctx);
                }
            }

            return InvokeInternalAsync(ctx);
        }

        private async Task InvokeInternalAsync(IContext ctx)
        {
            var request = await _requestMapper.MapAsync(ctx.Request, _options).ConfigureAwait(false);

            var logRequest = false;
            IResponseMessage? response = null;
            (MappingMatcherResult? Match, MappingMatcherResult? Partial) result = (null, null);

            try
            {
                foreach (var mapping in _options.Mappings.Values)
                {
                    if (mapping.Scenario is null)
                    {
                        continue;
                    }

                    // Set scenario start
                    if (!_options.Scenarios.ContainsKey(mapping.Scenario) && mapping.IsStartState)
                    {
                        _options.Scenarios.TryAdd(mapping.Scenario, new ScenarioState
                        {
                            Name = mapping.Scenario
                        });
                    }
                }

                result = _mappingMatcher.FindBestMatch(request);

                var targetMapping = result.Match?.Mapping;
                if (targetMapping == null)
                {
                    logRequest = true;
                    _options.Logger.Warn("HttpStatusCode set to 404 : No matching mapping found");
                    response = ResponseMessageBuilder.Create(HttpStatusCode.NotFound, WireMockConstants.NoMatchingFound);
                    return;
                }

                logRequest = targetMapping.LogMapping;

                if (targetMapping.IsAdminInterface && _options.AuthenticationMatcher != null && request.Headers != null)
                {
                    var authorizationHeaderPresent = request.Headers.TryGetValue(HttpKnownHeaderNames.Authorization, out var authorization);
                    if (!authorizationHeaderPresent)
                    {
                        _options.Logger.Error("HttpStatusCode set to 401, authorization header is missing.");
                        response = ResponseMessageBuilder.Create(HttpStatusCode.Unauthorized, null);
                        return;
                    }

                    var authorizationHeaderMatchResult = _options.AuthenticationMatcher.IsMatch(authorization!.ToString());
                    if (!MatchScores.IsPerfect(authorizationHeaderMatchResult.Score))
                    {
                        _options.Logger.Error("HttpStatusCode set to 401, authentication failed.", authorizationHeaderMatchResult.Exception ?? throw new WireMockException("Authentication failed"));
                        response = ResponseMessageBuilder.Create(HttpStatusCode.Unauthorized, null);
                        return;
                    }
                }

                if (!targetMapping.IsAdminInterface && _options.RequestProcessingDelay > TimeSpan.Zero)
                {
                    await Task.Delay(_options.RequestProcessingDelay.Value).ConfigureAwait(false);
                }

                var (theResponse, theOptionalNewMapping) = await targetMapping.ProvideResponseAsync(request).ConfigureAwait(false);
                response = theResponse;

                var responseBuilder = targetMapping.Provider as Response;

                if (!targetMapping.IsAdminInterface && theOptionalNewMapping != null)
                {
                    if (responseBuilder?.ProxyAndRecordSettings?.SaveMapping == true || targetMapping.Settings.ProxyAndRecordSettings?.SaveMapping == true)
                    {
                        _options.Mappings.TryAdd(theOptionalNewMapping.Guid, theOptionalNewMapping);
                    }

                    if (responseBuilder?.ProxyAndRecordSettings?.SaveMappingToFile == true || targetMapping.Settings.ProxyAndRecordSettings?.SaveMappingToFile == true)
                    {
                        var matcherMapper = new MatcherMapper(targetMapping.Settings);
                        var mappingConverter = new MappingConverter(matcherMapper);
                        var mappingToFileSaver = new MappingToFileSaver(targetMapping.Settings, mappingConverter);

                        mappingToFileSaver.SaveMappingToFile(theOptionalNewMapping);
                    }
                }

                if (targetMapping.Scenario != null)
                {
                    UpdateScenarioState(targetMapping);
                }

                if (!targetMapping.IsAdminInterface && targetMapping.Webhooks?.Length > 0)
                {
                    await SendToWebhooksAsync(targetMapping, request, response).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                _options.Logger.Error($"Providing a Response for Mapping '{result.Match?.Mapping.Guid}' failed. HttpStatusCode set to 500. Exception: {ex}");
                response = ResponseMessageBuilder.Create(500, ex.Message);
            }
            finally
            {
                var log = new LogEntry
                {
                    Guid = _guidUtils.NewGuid(),
                    RequestMessage = request,
                    ResponseMessage = response,

                    MappingGuid = result.Match?.Mapping?.Guid,
                    MappingTitle = result.Match?.Mapping?.Title,
                    RequestMatchResult = result.Match?.RequestMatchResult,

                    PartialMappingGuid = result.Partial?.Mapping?.Guid,
                    PartialMappingTitle = result.Partial?.Mapping?.Title,
                    PartialMatchResult = result.Partial?.RequestMatchResult
                };

                LogRequest(log, logRequest);

                try
                {
                    if (_options.SaveUnmatchedRequests == true && result.Match?.RequestMatchResult is not { IsPerfectMatch: true })
                    {
                        var filename = $"{log.Guid}.LogEntry.json";
                        _options.FileSystemHandler?.WriteUnmatchedRequest(filename, JsonUtils.Serialize(log));
                    }
                }
                catch
                {
                    // Empty catch
                }

                try
                {
                    await _responseMapper.MapAsync(response, ctx.Response).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _options.Logger.Error("HttpStatusCode set to 404 : No matching mapping found", ex);

                    var notFoundResponse = ResponseMessageBuilder.Create(HttpStatusCode.NotFound, WireMockConstants.NoMatchingFound);
                    await _responseMapper.MapAsync(notFoundResponse, ctx.Response).ConfigureAwait(false);
                }
            }

            await CompletedTask.ConfigureAwait(false);
        }

        private async Task SendToWebhooksAsync(IMapping mapping, IRequestMessage request, IResponseMessage response)
        {
            var tasks = new List<Func<Task>>();
            for (int index = 0; index < mapping.Webhooks?.Length; index++)
            {
                var httpClientForWebhook = HttpClientBuilder.Build(mapping.Settings.WebhookSettings ?? new WebhookSettings());
                var webhookSender = new WebhookSender(mapping.Settings);
                var webhookRequest = mapping.Webhooks[index].Request;
                var webHookIndex = index;

                tasks.Add(async () =>
                {
                    try
                    {
                        var result = await webhookSender.SendAsync(httpClientForWebhook, mapping, webhookRequest, request, response).ConfigureAwait(false);
                        if (!result.IsSuccessStatusCode)
                        {
                            var content = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
                            _options.Logger.Warn($"Sending message to Webhook [{webHookIndex}] from Mapping '{mapping.Guid}' failed. HttpStatusCode: {result.StatusCode} Content: {content}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _options.Logger.Error($"Sending message to Webhook [{webHookIndex}] from Mapping '{mapping.Guid}' failed. Exception: {ex}");
                    }
                });
            }

            if (mapping.UseWebhooksFireAndForget == true)
            {
                try
                {
                    // Do not wait
                    await Task.Run(() =>
                    {
                        Task.WhenAll(tasks.Select(async task => await task.Invoke())).ConfigureAwait(false);
                    });
                }
                catch
                {
                    // Ignore
                }
            }
            else
            {
                await Task.WhenAll(tasks.Select(async task => await task.Invoke())).ConfigureAwait(false);
            }
        }

        private void UpdateScenarioState(IMapping mapping)
        {
            var scenario = _options.Scenarios[mapping.Scenario!];

            // Increase the number of times this state has been executed
            scenario.Counter++;

            // Only if the number of times this state is executed equals the required StateTimes, proceed to next state and reset the counter to 0
            if (scenario.Counter == (mapping.TimesInSameState ?? 1))
            {
                scenario.NextState = mapping.NextState;
                scenario.Counter = 0;
            }

            // Else just update Started and Finished
            scenario.Started = true;
            scenario.Finished = mapping.NextState == null;
        }

        private void LogRequest(LogEntry entry, bool addRequest)
        {
            _options.Logger.DebugRequestResponse(_logEntryMapper.Map(entry), entry.RequestMessage.Path.StartsWith("/__admin/"));

            // If addRequest is set to true and MaxRequestLogCount is null or does have a value greater than 0, try to add a new request log.
            if (addRequest && _options.MaxRequestLogCount is null or > 0)
            {
                TryAddLogEntry(entry);
            }

            // In case MaxRequestLogCount has a value greater than 0, try to delete existing request logs based on the count.
            if (_options.MaxRequestLogCount is > 0)
            {
                var logEntries = _options.LogEntries.ToList();
                foreach (var logEntry in logEntries.OrderBy(le => le.RequestMessage.DateTime).Take(logEntries.Count - _options.MaxRequestLogCount.Value))
                {
                    TryRemoveLogEntry(logEntry);
                }
            }

            // In case RequestLogExpirationDuration has a value greater than 0, try to delete existing request logs based on the date.
            if (_options.RequestLogExpirationDuration is > 0)
            {
                var checkTime = DateTime.UtcNow.AddHours(-_options.RequestLogExpirationDuration.Value);
                foreach (var logEntry in _options.LogEntries.ToList().Where(le => le.RequestMessage.DateTime < checkTime))
                {
                    TryRemoveLogEntry(logEntry);
                }
            }
        }

        private void TryAddLogEntry(LogEntry logEntry)
        {
            try
            {
                _options.LogEntries.Add(logEntry);
            }
            catch
            {
                // Ignore exception (can happen during stress testing)
            }
        }

        private void TryRemoveLogEntry(LogEntry logEntry)
        {
            try
            {
                _options.LogEntries.Remove(logEntry);
            }
            catch
            {
                // Ignore exception (can happen during stress testing)
            }
        }
    }
}
