// Copyright © WireMock.Net

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using Stef.Validation;
using WireMock.Models;
using WireMock.Settings;
using WireMock.Transformers;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.Http;

internal class WebhookSender(WireMockServerSettings settings)
{
    private const string ClientIp = "::1";
    private static readonly ThreadLocal<Random> Random = new(() => new Random(DateTime.UtcNow.Millisecond));

    private readonly WireMockServerSettings _settings = Guard.NotNull(settings);

    public async Task<HttpResponseMessage> SendAsync(
        HttpClient client,
        IMapping mapping,
        IWebhookRequest webhookRequest,
        IRequestMessage originalRequestMessage,
        IResponseMessage originalResponseMessage
    )
    {
        Guard.NotNull(client);
        Guard.NotNull(mapping);
        Guard.NotNull(webhookRequest);
        Guard.NotNull(originalRequestMessage);
        Guard.NotNull(originalResponseMessage);

        IBodyData? bodyData;
        IDictionary<string, WireMockList<string>>? headers;
        string requestUrl;
        if (webhookRequest.UseTransformer == true)
        {
            var transformer = TransformerFactory.Create(webhookRequest.TransformerType, _settings);
            bodyData = transformer.TransformBody(mapping, originalRequestMessage, originalResponseMessage, webhookRequest.BodyData, webhookRequest.TransformerReplaceNodeOptions);
            headers = transformer.TransformHeaders(mapping, originalRequestMessage, originalResponseMessage, webhookRequest.Headers);
            requestUrl = transformer.TransformString(mapping, originalRequestMessage, originalResponseMessage, webhookRequest.Url);

            mapping.Settings.WebhookSettings?.PostTransform(mapping, requestUrl, bodyData, headers);
        }
        else
        {
            bodyData = webhookRequest.BodyData;
            headers = webhookRequest.Headers;
            requestUrl = webhookRequest.Url;
        }

        // Create RequestMessage
        var requestMessage = new RequestMessage(
            new UrlDetails(requestUrl),
            webhookRequest.Method,
            ClientIp,
            bodyData,
            headers?.ToDictionary(x => x.Key, x => x.Value.ToArray())
        )
        {
            DateTime = DateTime.UtcNow
        };

        // Create HttpRequestMessage
        var httpRequestMessage = HttpRequestMessageHelper.Create(requestMessage, requestUrl);

        // Delay (if required)
        if (TryGetDelay(webhookRequest, out var delay))
        {
            await Task.Delay(delay.Value).ConfigureAwait(false);
        }

        // Call the URL
        return await client.SendAsync(httpRequestMessage).ConfigureAwait(false);
    }

    private static bool TryGetDelay(IWebhookRequest webhookRequest, [NotNullWhen(true)] out int? delay)
    {
        delay = webhookRequest.Delay;
        var minimumDelay = webhookRequest.MinimumRandomDelay;
        var maximumDelay = webhookRequest.MaximumRandomDelay;

        if (minimumDelay is not null && maximumDelay is not null && maximumDelay >= minimumDelay)
        {
            delay = Random.Value!.Next(minimumDelay.Value, maximumDelay.Value);
            return true;
        }

        return delay is not null;
    }
}