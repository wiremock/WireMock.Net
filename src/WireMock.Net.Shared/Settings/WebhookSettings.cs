// Copyright © WireMock.Net

using System.Collections.Generic;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.Settings;

/// <summary>
/// WebhookSettings
/// </summary>
public class WebhookSettings : HttpClientSettings
{
    /// <summary>
    /// Executes an action after the transformation of the request body.
    /// </summary>
    /// <param name="mapping">The mapping used for the request.</param>
    /// <param name="requestUrl">The request Url.</param>
    /// <param name="bodyData">The body data of the request. [Optional]</param>
    /// <param name="headers">The headers of the request. [Optional]</param>
    public virtual void PostTransform(IMapping mapping, string requestUrl, IBodyData? bodyData = null, IDictionary<string, WireMockList<string>>? headers = null)
    {
    }
}