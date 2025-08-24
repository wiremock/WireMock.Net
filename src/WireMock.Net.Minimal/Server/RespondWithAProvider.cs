// Copyright © WireMock.Net and mock4net by Alexandre Victoor

// This source file is based on mock4net by Alexandre Victoor which is licensed under the Apache 2.0 License.
// For more details see 'mock4net/LICENSE.txt' and 'mock4net/readme.md' in this project root.
using System;
using System.Collections.Generic;
using System.Net;
using Stef.Validation;
using WireMock.Matchers.Request;
using WireMock.Models;
using WireMock.ResponseBuilders;
using WireMock.ResponseProviders;
using WireMock.Settings;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.Server;

/// <summary>
/// The RespondWithAProvider.
/// </summary>
internal class RespondWithAProvider : IRespondWithAProvider
{
    private readonly RegistrationCallback _registrationCallback;
    private readonly IRequestMatcher _requestMatcher;
    private readonly WireMockServerSettings _settings;
    private readonly IDateTimeUtils _dateTimeUtils;
    private readonly bool _saveToFile;

    private int _priority;
    private string? _title;
    private string? _description;
    private string? _path;
    private string? _executionConditionState;
    private string? _nextState;
    private string? _scenario;
    private int _timesInSameState = 1;
    private bool? _useWebhookFireAndForget;
    private double? _probability;
    private GraphQLSchemaDetails? _graphQLSchemaDetails;

    public Guid Guid { get; private set; }

    public IWebhook[]? Webhooks { get; private set; }

    public ITimeSettings? TimeSettings { get; private set; }

    public object? Data { get; private set; }

    public IdOrTexts? ProtoDefinition { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RespondWithAProvider"/> class.
    /// </summary>
    /// <param name="registrationCallback">The registration callback.</param>
    /// <param name="requestMatcher">The request matcher.</param>
    /// <param name="settings">The WireMockServerSettings.</param>
    /// <param name="guidUtils">GuidUtils to make unit testing possible.</param>
    /// <param name="dateTimeUtils">DateTimeUtils to make unit testing possible.</param>
    /// <param name="saveToFile">Optional boolean to indicate if this mapping should be saved as static mapping file.</param>
    public RespondWithAProvider(
        RegistrationCallback registrationCallback,
        IRequestMatcher requestMatcher,
        WireMockServerSettings settings,
        IGuidUtils guidUtils,
        IDateTimeUtils dateTimeUtils,
        bool saveToFile = false
    )
    {
        _registrationCallback = Guard.NotNull(registrationCallback);
        _requestMatcher = Guard.NotNull(requestMatcher);
        _settings = Guard.NotNull(settings);
        _dateTimeUtils = Guard.NotNull(dateTimeUtils);
        _saveToFile = saveToFile;

        Guid = guidUtils.NewGuid();
    }

    /// <inheritdoc />
    public void RespondWith(IResponseProvider provider)
    {
        var mapping = new Mapping
        (
            Guid,
            _dateTimeUtils.UtcNow,
            _title,
            _description,
            _path,
            _settings,
            _requestMatcher,
            provider,
            _priority,
            _scenario,
            _executionConditionState,
            _nextState,
            _timesInSameState,
            Webhooks,
            _useWebhookFireAndForget,
            TimeSettings,
            Data
        );

        if (_probability != null)
        {
            mapping.WithProbability(_probability.Value);
        }

        if (ProtoDefinition != null)
        {
            mapping.WithProtoDefinition(ProtoDefinition.Value);
        }

        _registrationCallback(mapping, _saveToFile);
    }

    /// <inheritdoc />
    public void ThenRespondWith(Action<IResponseBuilder> action)
    {
        var responseBuilder = Response.Create();

        action(responseBuilder);

        RespondWith(responseBuilder);
    }

    /// <inheritdoc />
    public void ThenRespondWithOK()
    {
        var responseBuilder = Response.Create();

        RespondWith(responseBuilder);
    }

    /// <inheritdoc />
    public void ThenRespondWithStatusCode(int code)
    {
        var responseBuilder = Response.Create().WithStatusCode(code);

        RespondWith(responseBuilder);
    }

    /// <inheritdoc />
    public void ThenRespondWithStatusCode(string code)
    {
        var responseBuilder = Response.Create().WithStatusCode(code);

        RespondWith(responseBuilder);
    }

    /// <inheritdoc />
    public void ThenRespondWithStatusCode(HttpStatusCode code)
    {
        var responseBuilder = Response.Create().WithStatusCode(code);

        RespondWith(responseBuilder);
    }

    /// <inheritdoc />
    public IRespondWithAProvider WithData(object data)
    {
        Data = data;
        return this;
    }

    /// <inheritdoc />
    public IRespondWithAProvider WithGuid(string guid)
    {
        return WithGuid(Guid.Parse(guid));
    }

    /// <inheritdoc />
    public IRespondWithAProvider WithGuid(Guid guid)
    {
        Guid = guid;
        return this;
    }

    /// <inheritdoc />
    public IRespondWithAProvider DefineGuid(Guid guid)
    {
        return WithGuid(guid);
    }

    /// <inheritdoc />
    public IRespondWithAProvider DefineGuid(string guid)
    {
        return WithGuid(guid);
    }

    /// <inheritdoc />
    public IRespondWithAProvider WithTitle(string title)
    {
        _title = title;

        return this;
    }

    /// <inheritdoc />
    public IRespondWithAProvider WithDescription(string description)
    {
        _description = description;
        return this;
    }

    /// <inheritdoc />
    public IRespondWithAProvider WithPath(string path)
    {
        _path = path;
        return this;
    }

    /// <inheritdoc />
    public IRespondWithAProvider AtPriority(int priority)
    {
        _priority = priority;
        return this;
    }

    /// <inheritdoc />
    public IRespondWithAProvider InScenario(string scenario)
    {
        _scenario = Guard.NotNullOrWhiteSpace(scenario);
        return this;
    }

    /// <inheritdoc />
    public IRespondWithAProvider InScenario(int scenario)
    {
        return InScenario(scenario.ToString());
    }

    /// <inheritdoc />
    public IRespondWithAProvider WhenStateIs(string state)
    {
        if (string.IsNullOrEmpty(_scenario))
        {
            throw new NotSupportedException("Unable to set state condition when no scenario is defined.");
        }

        _executionConditionState = state;

        return this;
    }

    /// <inheritdoc />
    public IRespondWithAProvider WhenStateIs(int state)
    {
        return WhenStateIs(state.ToString());
    }

    /// <inheritdoc />
    public IRespondWithAProvider WillSetStateTo(string state, int? times = 1)
    {
        if (string.IsNullOrEmpty(_scenario))
        {
            throw new NotSupportedException("Unable to set next state when no scenario is defined.");
        }

        _nextState = state;
        _timesInSameState = times ?? 1;

        return this;
    }

    /// <inheritdoc />
    public IRespondWithAProvider WillSetStateTo(int state, int? times = 1)
    {
        return WillSetStateTo(state.ToString(), times);
    }

    /// <inheritdoc />
    public IRespondWithAProvider WithTimeSettings(ITimeSettings timeSettings)
    {
        TimeSettings = Guard.NotNull(timeSettings);

        return this;
    }

    /// <inheritdoc />
    public IRespondWithAProvider WithWebhook(params IWebhook[] webhooks)
    {
        Guard.HasNoNulls(webhooks);

        Webhooks = webhooks;
        return this;
    }

    /// <inheritdoc />
    public IRespondWithAProvider WithWebhook(
        string url,
        string method = "post",
        IDictionary<string, WireMockList<string>>? headers = null,
        string? body = null,
        bool useTransformer = true,
        TransformerType transformerType = TransformerType.Handlebars)
    {
        Guard.NotNull(url);
        Guard.NotNull(method);

        Webhooks = [InitWebhook(url, method, headers, useTransformer, transformerType)];

        if (body != null)
        {
            Webhooks[0].Request.BodyData = new BodyData
            {
                BodyAsString = body,
                DetectedBodyType = BodyType.String,
                DetectedBodyTypeFromContentType = BodyType.String
            };
        }

        return this;
    }

    /// <inheritdoc />
    public IRespondWithAProvider WithWebhook(
        string url,
        string method = "post",
        IDictionary<string, WireMockList<string>>? headers = null,
        object? body = null,
        bool useTransformer = true,
        TransformerType transformerType = TransformerType.Handlebars)
    {
        Guard.NotNull(url);
        Guard.NotNull(method);

        Webhooks = [InitWebhook(url, method, headers, useTransformer, transformerType)];

        if (body != null)
        {
            Webhooks[0].Request.BodyData = new BodyData
            {
                BodyAsJson = body,
                DetectedBodyType = BodyType.Json,
                DetectedBodyTypeFromContentType = BodyType.Json
            };
        }

        return this;
    }

    public IRespondWithAProvider WithWebhookFireAndForget(bool useWebhooksFireAndForget)
    {
        _useWebhookFireAndForget = useWebhooksFireAndForget;
        return this;
    }

    public IRespondWithAProvider WithProbability(double probability)
    {
        _probability = Guard.Condition(probability, p => p is >= 0 and <= 1.0);
        return this;
    }

    /// <inheritdoc />
    public IRespondWithAProvider WithProtoDefinition(params string[] protoDefinitionOrId)
    {
        Guard.NotNull(protoDefinitionOrId);

        ProtoDefinition = ProtoDefinitionUtils.GetIdOrTexts(_settings, protoDefinitionOrId);

        return this;
    }

    /// <inheritdoc />
    public IRespondWithAProvider WithGraphQLSchema(string graphQLSchemaOrId, IDictionary<string, Type>? customScalars = null)
    {
        Guard.NotNullOrWhiteSpace(graphQLSchemaOrId);

        if (_settings.GraphQLSchemas?.TryGetValue(graphQLSchemaOrId, out _graphQLSchemaDetails) != true)
        {
            _graphQLSchemaDetails = new GraphQLSchemaDetails
            {
                SchemaAsString = graphQLSchemaOrId,
                CustomScalars = customScalars
            };
        }

        return this;
    }

    private static IWebhook InitWebhook(
        string url,
        string method,
        IDictionary<string, WireMockList<string>>? headers,
        bool useTransformer,
        TransformerType transformerType
    )
    {
        return new Webhook
        {
            Request = new WebhookRequest
            {
                Url = url,
                Method = method,
                Headers = headers,
                UseTransformer = useTransformer,
                TransformerType = transformerType
            }
        };
    }
}