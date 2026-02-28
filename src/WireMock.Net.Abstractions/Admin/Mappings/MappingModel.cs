// Copyright © WireMock.Net

using WireMock.Models;

namespace WireMock.Admin.Mappings;

/// <summary>
/// MappingModel
/// </summary>
[FluentBuilder.AutoGenerateBuilder]
public class MappingModel
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    public Guid? Guid { get; set; }

    /// <summary>
    /// The datetime when this mapping was created or updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the TimeSettings when which this mapping should be used.
    /// </summary>
    public TimeSettingsModel? TimeSettings { get; set; }

    /// <summary>
    /// The unique title.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// The description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The priority. (A low value means higher priority.)
    /// </summary>
    public int? Priority { get; set; }

    /// <summary>
    /// The Scenario.
    /// </summary>
    public string? Scenario { get; set; }

    /// <summary>
    /// Execution state condition for the current mapping.
    /// </summary>
    public string? WhenStateIs { get; set; }

    /// <summary>
    /// The next state which will be signaled after the current mapping execution.
    /// In case the value is null state will not be changed.
    /// </summary>
    public string? SetStateTo { get; set; }
    
    /// <summary>
    /// The number of times this match should be matched before the state will be changed to the specified one.
    /// </summary>
    public int? TimesInSameState { get; set; }

    /// <summary>
    /// The request model.
    /// </summary>
    public required RequestModel Request { get; set; }

    /// <summary>
    /// The response model.
    /// </summary>
    public required ResponseModel Response { get; set; }

    /// <summary>
    /// Saves this mapping as a static mapping file.
    /// </summary>
    public bool? SaveToFile { get; set; }

    /// <summary>
    /// The Webhook.
    /// </summary>
    public WebhookModel? Webhook { get; set; }

    /// <summary>
    /// The Webhooks.
    /// </summary>
    public WebhookModel[]? Webhooks { get; set; }

    /// <summary>
    /// Fire and forget for webhooks.
    /// </summary>
    public bool? UseWebhooksFireAndForget { get; set; }

    /// <summary>
    /// Data Object which can be used when WithTransformer is used.
    /// e.g. lookup a path in this object using
    /// <example>
    /// lookup data "1"
    /// </example>
    /// </summary>
    public object? Data { get; set; }

    /// <summary> 
    /// The probability when this request should be matched. Value is between 0 and 1. [Optional]
    /// </summary>
    public double? Probability { get; set; }

    /// <summary>
    /// The Grpc ProtoDefinition which is used for this mapping (request and response). [Optional]
    /// </summary>
    public string? ProtoDefinition { get; set; }

    /// <summary>
    /// The Grpc ProtoDefinitions which are used for this mapping (request and response). [Optional]
    /// </summary>
    public string[]? ProtoDefinitions { get; set; }
}