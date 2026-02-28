// Copyright © WireMock.Net

using System.Net;
using WireMock.Models;
using WireMock.ResponseBuilders;
using WireMock.ResponseProviders;
using WireMock.Settings;
using WireMock.Types;

namespace WireMock.Server;

/// <summary>
/// IRespondWithAProvider
/// </summary>
public interface IRespondWithAProvider
{
    /// <summary>
    /// Gets the unique identifier for this mapping.
    /// </summary>
    Guid Guid { get; }

    /// <summary>
    /// Define a unique identifier for this mapping.
    /// </summary>
    /// <param name="guid">The unique identifier.</param>
    /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
    IRespondWithAProvider WithGuid(Guid guid);

    /// <summary>
    /// Define a unique identifier for this mapping.
    /// </summary>
    /// <param name="guid">The unique identifier.</param>
    /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
    IRespondWithAProvider WithGuid(string guid);

    /// <summary>
    /// Define a unique identifier for this mapping.
    /// </summary>
    /// <param name="guid">The unique identifier.</param>
    /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
    IRespondWithAProvider DefineGuid(Guid guid);

    /// <summary>
    /// Define a unique identifier for this mapping.
    /// </summary>
    /// <param name="guid">The unique identifier.</param>
    /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
    IRespondWithAProvider DefineGuid(string guid);

    /// <summary>
    /// Define the TimeSettings for this mapping.
    /// </summary>
    /// <param name="timeSettings">The TimeSettings.</param>
    /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
    IRespondWithAProvider WithTimeSettings(ITimeSettings timeSettings);

    /// <summary>
    /// Define a unique title for this mapping.
    /// </summary>
    /// <param name="title">The unique title.</param>
    /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
    IRespondWithAProvider WithTitle(string title);

    /// <summary>
    /// Define a description for this mapping.
    /// </summary>
    /// <param name="description">The description.</param>
    /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
    IRespondWithAProvider WithDescription(string description);

    /// <summary>
    /// Define the full filepath for this mapping.
    /// </summary>
    /// <param name="path">The full filepath.</param>
    /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
    IRespondWithAProvider WithPath(string path);

    /// <summary>
    /// Define the priority for this mapping.
    /// </summary>
    /// <param name="priority">The priority. (A lower value means a higher priority.)</param>
    /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
    IRespondWithAProvider AtPriority(int priority);

    /// <summary>
    /// RespondWith
    /// </summary>
    /// <param name="provider">The provider.</param>
    void RespondWith(IResponseProvider provider);

    /// <summary>
    /// RespondWith
    /// </summary>
    /// <param name="action">The action to use the fluent <see cref="IResponseBuilder"/>.</param>
    void ThenRespondWith(Action<IResponseBuilder> action);

    /// <summary>
    /// RespondWith a status code 200 (OK);
    /// </summary>
    void ThenRespondWithOK();

    /// <summary>
    /// RespondWith a status code.
    /// By default all status codes are allowed, to change this behaviour, see <inheritdoc cref="WireMockServerSettings.AllowOnlyDefinedHttpStatusCodeInResponse"/>.
    /// </summary>
    /// <param name="code">The code.</param>
    void ThenRespondWithStatusCode(int code);

    /// <summary>
    /// RespondWith a status code.
    /// By default all status codes are allowed, to change this behaviour, see <inheritdoc cref="WireMockServerSettings.AllowOnlyDefinedHttpStatusCodeInResponse"/>.
    /// </summary>
    /// <param name="code">The code.</param>
    void ThenRespondWithStatusCode(string code);

    /// <summary>
    /// RespondWith a status code.
    /// By default all status codes are allowed, to change this behaviour, see <inheritdoc cref="WireMockServerSettings.AllowOnlyDefinedHttpStatusCodeInResponse"/>.
    /// </summary>
    /// <param name="code">The code.</param>
    void ThenRespondWithStatusCode(HttpStatusCode code);

    /// <summary>
    /// Sets the scenario.
    /// </summary>
    /// <param name="scenario">The scenario.</param>
    /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
    IRespondWithAProvider InScenario(string scenario);

    /// <summary>
    /// Sets the scenario with an integer value.
    /// </summary>
    /// <param name="scenario">The scenario.</param>
    /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
    IRespondWithAProvider InScenario(int scenario);

    /// <summary>
    /// Execute this respond only in case the current state is equal to specified one.
    /// </summary>
    /// <param name="state">Any object which identifies the current state</param>
    /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
    IRespondWithAProvider WhenStateIs(string state);

    /// <summary>
    /// Execute this respond only in case the current state is equal to specified one.
    /// </summary>
    /// <param name="state">Any object which identifies the current state</param>
    /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
    IRespondWithAProvider WhenStateIs(int state);

    /// <summary>
    /// Once this mapping is executed the state will be changed to specified one.
    /// </summary>
    /// <param name="state">Any object which identifies the new state</param>
    /// <param name="times">The number of times this match should be matched before the state will be changed to the specified one. Default value is 1.</param>
    /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
    IRespondWithAProvider WillSetStateTo(string state, int? times = 1);

    /// <summary>
    /// Once this mapping is executed the state will be changed to specified one.
    /// </summary>
    /// <param name="state">Any object which identifies the new state</param>
    /// <param name="times">The number of times this match should be matched before the state will be changed to the specified one. Default value is 1.</param>
    /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
    IRespondWithAProvider WillSetStateTo(int state, int? times = 1);

    /// <summary>
    /// Add (multiple) Webhook(s) to call after the response has been generated.
    /// </summary>
    /// <param name="webhooks">The Webhooks</param>
    /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
    IRespondWithAProvider WithWebhook(params IWebhook[] webhooks);

    /// <summary>
    /// Support FireAndForget for any configured Webhooks
    /// </summary>
    /// <param name="useWebhooksFireAndForget"></param>
    /// <returns></returns>
    IRespondWithAProvider WithWebhookFireAndForget(bool useWebhooksFireAndForget);

    /// <summary>
    /// Add a Webhook to call after the response has been generated.
    /// </summary>
    /// <param name="url">The Webhook Url</param>
    /// <param name="method">The method to use. [optional]</param>
    /// <param name="headers">The Headers to send. [optional]</param>
    /// <param name="body">The body (as string) to send. [optional]</param>
    /// <param name="useTransformer">Use Transformer. [optional]</param>
    /// <param name="transformerType">The transformer type. [optional]</param>
    /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
    IRespondWithAProvider WithWebhook(
        string url,
        string method = "post",
        IDictionary<string, WireMockList<string>>? headers = null,
        string? body = null,
        bool useTransformer = true,
        TransformerType transformerType = TransformerType.Handlebars
    );

    /// <summary>
    /// Add a Webhook to call after the response has been generated.
    /// </summary>
    /// <param name="url">The Webhook Url</param>
    /// <param name="method">The method to use. [optional]</param>
    /// <param name="headers">The Headers to send. [optional]</param>
    /// <param name="body">The body (as json) to send. [optional]</param>
    /// <param name="useTransformer">Use Transformer. [optional]</param>
    /// <param name="transformerType">The transformer type. [optional]</param>
    /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
    IRespondWithAProvider WithWebhook(
        string url,
        string method = "post",
        IDictionary<string, WireMockList<string>>? headers = null,
        object? body = null,
        bool useTransformer = true,
        TransformerType transformerType = TransformerType.Handlebars
    );

    /// <summary>
    /// Data Object which can be used when WithTransformer is used.
    /// e.g. lookup a path in this object using
    /// <param name="data">The data dictionary object.</param>
    /// <example>
    /// lookup data "1"
    /// </example>
    /// </summary>
    /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
    IRespondWithAProvider WithData(object data);

    /// <summary>
    /// Define the probability when this request should be matched. Value is between 0 and 1.
    /// </summary>
    /// <param name="probability">The probability when this request should be matched. Value is between 0 and 1.</param>
    /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
    IRespondWithAProvider WithProbability(double probability);

    /// <summary>
    /// Define a Grpc ProtoDefinition which is used for the request and the response.
    /// This can be a ProtoDefinition as a string, or an id when the ProtoDefinitions are defined at the WireMockServer.
    /// </summary>
    /// <param name="protoDefinitionOrId">The proto definition as text or as id.</param>
    /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
    IRespondWithAProvider WithProtoDefinition(params string[] protoDefinitionOrId);

    /// <summary>
    /// Define a GraphQL Schema which is used for the request and the response.
    /// This can be a GraphQL Schema as a string, or an id when the GraphQL Schema are defined at the WireMockServer.
    /// </summary>
    /// <param name="graphQLSchemaOrId">The GraphQL Schema as text or as id.</param>
    /// <param name="customScalars">A dictionary defining the custom scalars used in this schema. [optional]</param>
    /// <returns>The <see cref="IRespondWithAProvider"/>.</returns>
    IRespondWithAProvider WithGraphQLSchema(string graphQLSchemaOrId, IDictionary<string, Type>? customScalars = null);
}