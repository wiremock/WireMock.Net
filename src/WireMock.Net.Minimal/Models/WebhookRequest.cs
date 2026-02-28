// Copyright © WireMock.Net

using WireMock.Types;
using WireMock.Util;

namespace WireMock.Models;

/// <summary>
/// WebhookRequest
/// </summary>
public class WebhookRequest : IWebhookRequest
{
    /// <inheritdoc />
    public string Url { get; set; } = null!;

    /// <inheritdoc />
    public string Method { get; set; } = null!;

    /// <inheritdoc />
    public IDictionary<string, WireMockList<string>>? Headers { get; set; }

    /// <inheritdoc />
    public IBodyData? BodyData { get; set; }

    /// <inheritdoc />
    public bool? UseTransformer { get; set; }

    /// <inheritdoc />
    public TransformerType TransformerType { get; set; }

    /// <inheritdoc />
    public ReplaceNodeOptions TransformerReplaceNodeOptions { get; set; }

    /// <inheritdoc />
    public int? Delay { get; set; }

    /// <inheritdoc />
    public int? MinimumRandomDelay { get; set; }

    /// <inheritdoc />
    public int? MaximumRandomDelay { get; set; }
}