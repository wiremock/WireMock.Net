// Copyright © WireMock.Net

using WireMock.Types;
using WireMock.Util;

namespace WireMock.Models;

/// <summary>
/// IWebhookRequest
/// </summary>
public interface IWebhookRequest
{
    /// <summary>
    /// The Webhook Url.
    /// </summary>
    string Url { get; set; }

    /// <summary>
    /// The method to use.
    /// </summary>
    string Method { get; set; }

    /// <summary>
    /// The Headers to send.
    /// </summary>
    IDictionary<string, WireMockList<string>>? Headers { get; }

    /// <summary>
    /// The body to send.
    /// </summary>
    IBodyData? BodyData { get; set; }

    /// <summary>
    /// Use Transformer.
    /// </summary>
    bool? UseTransformer { get; set; }

    /// <summary>
    /// The transformer type.
    /// </summary>
    TransformerType TransformerType { get; set; }

    /// <summary>
    /// The ReplaceNodeOptions to use when transforming a JSON node.
    /// </summary>
    ReplaceNodeOptions TransformerReplaceNodeOptions { get; set; }

    /// <summary>
    /// Gets or sets the delay in milliseconds.
    /// </summary>
    int? Delay { get; set; }

    /// <summary>
    /// Gets or sets the minimum random delay in milliseconds.
    /// </summary>
    int? MinimumRandomDelay { get; set; }

    /// <summary>
    /// Gets or sets the maximum random delay in milliseconds.
    /// </summary>
    int? MaximumRandomDelay { get; set; }
}