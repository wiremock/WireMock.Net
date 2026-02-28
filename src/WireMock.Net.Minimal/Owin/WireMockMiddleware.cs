// Copyright © WireMock.Net

using System.Diagnostics;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Http;
using WireMock.Constants;
using WireMock.Exceptions;
using WireMock.Http;
using WireMock.Matchers;
using WireMock.Owin.ActivityTracing;
using WireMock.Owin.Mappers;
using WireMock.ResponseBuilders;
using WireMock.Serialization;
using WireMock.Settings;
using WireMock.Util;

namespace WireMock.Owin;

internal class WireMockMiddleware(
    RequestDelegate next,
    IWireMockMiddlewareOptions options,
    IOwinRequestMapper requestMapper,
    IOwinResponseMapper responseMapper,
    IMappingMatcher mappingMatcher,
    IWireMockMiddlewareLogger logger,
    IGuidUtils guidUtils,
    IDateTimeUtils dateTimeUtils
)
{
    private readonly object _lock = new();

    public Task Invoke(HttpContext ctx)
    {
        if (options.HandleRequestsSynchronously.GetValueOrDefault(false))
        {
            lock (_lock)
            {
                return InvokeInternalAsync(ctx);
            }
        }

        return InvokeInternalAsync(ctx);
    }

    private async Task InvokeInternalAsync(HttpContext ctx)
    {
        // Store options in HttpContext for providers to access (e.g., WebSocketResponseProvider)
        ctx.Items[nameof(IWireMockMiddlewareOptions)] = options;
        ctx.Items[nameof(IWireMockMiddlewareLogger)] = logger;
        ctx.Items[nameof(IGuidUtils)] = guidUtils;
        ctx.Items[nameof(IDateTimeUtils)] = dateTimeUtils;

        var request = await requestMapper.MapAsync(ctx, options).ConfigureAwait(false);

        var logRequest = false;
        IResponseMessage? response = null;
        (MappingMatcherResult? Match, MappingMatcherResult? Partial) result = (null, null);

        var tracingEnabled = options.ActivityTracingOptions is not null;
        var excludeAdmin = options.ActivityTracingOptions?.ExcludeAdminRequests ?? true;
        Activity? activity = null;

        // Check if we should trace this request (optionally exclude admin requests)
        var shouldTrace = tracingEnabled && !(excludeAdmin && request.Path.StartsWith("/__admin/"));

        if (shouldTrace)
        {
            activity = WireMockActivitySource.StartRequestActivity(request.Method, request.Path);
            WireMockActivitySource.EnrichWithRequest(activity, request, options.ActivityTracingOptions);
        }

        try
        {
            foreach (var mapping in options.Mappings.Values)
            {
                if (mapping.Scenario is null)
                {
                    continue;
                }

                // Set scenario start
                if (!options.Scenarios.ContainsKey(mapping.Scenario) && mapping.IsStartState)
                {
                    options.Scenarios.TryAdd(mapping.Scenario, new ScenarioState
                    {
                        Name = mapping.Scenario
                    });
                }
            }

            result = mappingMatcher.FindBestMatch(request);

            var targetMapping = result.Match?.Mapping;
            if (targetMapping == null)
            {
                logRequest = true;
                options.Logger.Warn("HttpStatusCode set to 404 : No matching mapping found");
                response = ResponseMessageBuilder.Create(HttpStatusCode.NotFound, WireMockConstants.NoMatchingFound);
                return;
            }

            logRequest = targetMapping.LogMapping;

            if (targetMapping.IsAdminInterface && options.AuthenticationMatcher != null && request.Headers != null)
            {
                var authorizationHeaderPresent = request.Headers.TryGetValue(HttpKnownHeaderNames.Authorization, out var authorization);
                if (!authorizationHeaderPresent)
                {
                    options.Logger.Error("HttpStatusCode set to 401, authorization header is missing.");
                    response = ResponseMessageBuilder.Create(HttpStatusCode.Unauthorized, null);
                    return;
                }

                var authorizationHeaderMatchResult = options.AuthenticationMatcher.IsMatch(authorization!.ToString());
                if (!MatchScores.IsPerfect(authorizationHeaderMatchResult.Score))
                {
                    options.Logger.Error("HttpStatusCode set to 401, authentication failed.", authorizationHeaderMatchResult.Exception ?? throw new WireMockException("Authentication failed"));
                    response = ResponseMessageBuilder.Create(HttpStatusCode.Unauthorized, null);
                    return;
                }
            }

            if (!targetMapping.IsAdminInterface && options.RequestProcessingDelay > TimeSpan.Zero)
            {
                await Task.Delay(options.RequestProcessingDelay.Value).ConfigureAwait(false);
            }

            var (theResponse, theOptionalNewMapping) = await targetMapping.ProvideResponseAsync(ctx, request).ConfigureAwait(false);
            response = theResponse;

            if (targetMapping.Provider is Response responseBuilder && !targetMapping.IsAdminInterface && theOptionalNewMapping != null)
            {
                if (responseBuilder?.ProxyAndRecordSettings?.SaveMapping == true || targetMapping.Settings.ProxyAndRecordSettings?.SaveMapping == true)
                {
                    options.Mappings.TryAdd(theOptionalNewMapping.Guid, theOptionalNewMapping);
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
            options.Logger.Error($"Providing a Response for Mapping '{result.Match?.Mapping.Guid}' failed. HttpStatusCode set to 500. Exception: {ex}");
            WireMockActivitySource.RecordException(activity, ex);

            response = ResponseMessageBuilder.Create(500, ex.Message);
        }
        finally
        {
            logger.LogRequestAndResponse(logRequest, request, response, result.Match, result.Partial, activity);

            try
            {
                await responseMapper.MapAsync(response, ctx.Response).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                options.Logger.Error("HttpStatusCode set to 404 : No matching mapping found", ex);

                var notFoundResponse = ResponseMessageBuilder.Create(HttpStatusCode.NotFound, WireMockConstants.NoMatchingFound);
                await responseMapper.MapAsync(notFoundResponse, ctx.Response).ConfigureAwait(false);
            }
        }
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
                        options.Logger.Warn($"Sending message to Webhook [{webHookIndex}] from Mapping '{mapping.Guid}' failed. HttpStatusCode: {result.StatusCode} Content: {content}");
                    }
                }
                catch (Exception ex)
                {
                    options.Logger.Error($"Sending message to Webhook [{webHookIndex}] from Mapping '{mapping.Guid}' failed. Exception: {ex}");
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
        var scenario = options.Scenarios[mapping.Scenario!];

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
}