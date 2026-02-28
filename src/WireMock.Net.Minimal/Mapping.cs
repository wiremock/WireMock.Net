// Copyright © WireMock.Net

using Microsoft.AspNetCore.Http;
using Stef.Validation;
using WireMock.Matchers.Request;
using WireMock.Models;
using WireMock.ResponseProviders;
using WireMock.Settings;

namespace WireMock;

/// <summary>
/// The Mapping.
/// </summary>
public class Mapping : IMapping
{
    /// <inheritdoc />
    public Guid Guid { get; }

    /// <inheritdoc />
    public DateTime? UpdatedAt { get; set; }

    /// <inheritdoc />
    public string? Title { get; }

    /// <inheritdoc />
    public string? Description { get; }

    /// <inheritdoc />
    public string? Path { get; set; }

    /// <inheritdoc />
    public int Priority { get; }

    /// <inheritdoc />
    public string? Scenario { get; private set; }

    /// <inheritdoc />
    public string? ExecutionConditionState { get; }

    /// <inheritdoc />
    public string? NextState { get; }

    /// <inheritdoc />
    public int? TimesInSameState { get; }

    /// <inheritdoc />
    public IRequestMatcher RequestMatcher { get; }

    /// <inheritdoc />
    public IResponseProvider Provider { get; }

    /// <inheritdoc />
    public WireMockServerSettings Settings { get; }

    /// <inheritdoc />
    public bool IsStartState => Scenario == null || Scenario != null && NextState != null && ExecutionConditionState == null;

    /// <inheritdoc />
    public bool IsAdminInterface => Provider is DynamicResponseProvider or DynamicAsyncResponseProvider or ProxyAsyncResponseProvider;

    /// <inheritdoc />
    public bool IsProxy => Provider is ProxyAsyncResponseProvider;

    /// <inheritdoc />
    public bool LogMapping => Provider is not (DynamicResponseProvider or DynamicAsyncResponseProvider);

    /// <inheritdoc />
    public IWebhook[]? Webhooks { get; }

    /// <inheritdoc />
    public bool? UseWebhooksFireAndForget { get; }

    /// <inheritdoc />
    public ITimeSettings? TimeSettings { get; }

    /// <inheritdoc />
    public object? Data { get; }

    /// <inheritdoc />
    public double? Probability { get; private set; }

    /// <inheritdoc />
    public IdOrTexts? ProtoDefinition { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Mapping"/> class.
    /// </summary>
    /// <param name="guid">The unique identifier.</param>
    /// <param name="updatedAt">The datetime when this mapping was created.</param>
    /// <param name="title">The unique title (can be null).</param>
    /// <param name="description">The description (can be null).</param>
    /// <param name="path">The full file path from this mapping title (can be null).</param>
    /// <param name="settings">The WireMockServerSettings.</param>
    /// <param name="requestMatcher">The request matcher.</param>
    /// <param name="provider">The provider.</param>
    /// <param name="priority">The priority for this mapping.</param>
    /// <param name="scenario">The scenario. [Optional]</param>
    /// <param name="executionConditionState">State in which the current mapping can occur. [Optional]</param>
    /// <param name="nextState">The next state which will occur after the current mapping execution. [Optional]</param>
    /// <param name="stateTimes">Only when the current state is executed this number, the next state which will occur. [Optional]</param>
    /// <param name="webhooks">The Webhooks. [Optional]</param>
    /// <param name="useWebhooksFireAndForget">Use Fire and Forget for the defined webhook(s). [Optional]</param>
    /// <param name="timeSettings">The TimeSettings. [Optional]</param>
    /// <param name="data">The data object. [Optional]</param>
    public Mapping
    (
        Guid guid,
        DateTime updatedAt,
        string? title,
        string? description,
        string? path,
        WireMockServerSettings settings,
        IRequestMatcher requestMatcher,
        IResponseProvider provider,
        int priority,
        string? scenario,
        string? executionConditionState,
        string? nextState,
        int? stateTimes,
        IWebhook[]? webhooks,
        bool? useWebhooksFireAndForget,
        ITimeSettings? timeSettings,
        object? data
    )
    {
        Guid = guid;
        UpdatedAt = updatedAt;
        Title = title;
        Description = description;
        Path = path;
        Settings = settings;
        RequestMatcher = requestMatcher;
        Provider = provider;
        Priority = priority;
        Scenario = scenario;
        ExecutionConditionState = executionConditionState;
        NextState = nextState;
        TimesInSameState = stateTimes;
        Webhooks = webhooks;
        UseWebhooksFireAndForget = useWebhooksFireAndForget;
        TimeSettings = timeSettings;
        Data = data;
    }

    /// <inheritdoc />
    public Task<(IResponseMessage Message, IMapping? Mapping)> ProvideResponseAsync(HttpContext context, IRequestMessage requestMessage)
    {
        return Provider.ProvideResponseAsync(this, context, requestMessage, Settings);
    }

    /// <inheritdoc />
    public IRequestMatchResult GetRequestMatchResult(IRequestMessage requestMessage, string? nextState)
    {
        var result = new RequestMatchResult();

        RequestMatcher.GetMatchingScore(requestMessage, result);

        // Only check state if Scenario is defined
        if (Scenario != null)
        {
            var matcher = new RequestMessageScenarioAndStateMatcher(nextState, ExecutionConditionState);
            matcher.GetMatchingScore(requestMessage, result);
            //// If ExecutionConditionState is null, this means that request is the start from a scenario. So just return.
            //if (ExecutionConditionState != null)
            //{
            //    // ExecutionConditionState is not null, so get score for matching with the nextState.
            //    var matcher = new RequestMessageScenarioAndStateMatcher(nextState, ExecutionConditionState);
            //    matcher.GetMatchingScore(requestMessage, result);
            //}
        }

        return result;
    }

    /// <inheritdoc />
    public IMapping WithProbability(double probability)
    {
        Probability = Guard.NotNull(probability);
        return this;
    }

    /// <inheritdoc />
    public IMapping WithScenario(string scenario)
    {
        Scenario = Guard.NotNullOrWhiteSpace(scenario);
        return this;
    }

    /// <inheritdoc />
    public IMapping WithProtoDefinition(IdOrTexts protoDefinition)
    {
        ProtoDefinition = protoDefinition;
        return this;
    }
}