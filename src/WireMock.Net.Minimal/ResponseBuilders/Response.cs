// Copyright © WireMock.Net and mock4net by Alexandre Victoor

// This source file is based on mock4net by Alexandre Victoor which is licensed under the Apache 2.0 License.
// For more details see 'mock4net/LICENSE.txt' and 'mock4net/readme.md' in this project root.
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Stef.Validation;
using WireMock.Proxy;
using WireMock.RequestBuilders;
using WireMock.Settings;
using WireMock.Transformers;
using WireMock.Transformers.Handlebars;
using WireMock.Transformers.Scriban;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.ResponseBuilders;

/// <summary>
/// The Response.
/// </summary>
public partial class Response : IResponseBuilder
{
    private static readonly ThreadLocal<Random> Random = new(() => new Random(DateTime.UtcNow.Millisecond));

    private TimeSpan? _delay;

    /// <inheritdoc />
    public IMapping Mapping { get; set; } = null!;

    /// <summary>
    /// The minimum random delay in milliseconds.
    /// </summary>
    public int? MinimumDelayMilliseconds { get; private set; }

    /// <summary>
    /// The maximum random delay in milliseconds.
    /// </summary>
    public int? MaximumDelayMilliseconds { get; private set; }

    /// <summary>
    /// The delay
    /// </summary>
    public TimeSpan? Delay
    {
        get
        {
            if (MinimumDelayMilliseconds != null && MaximumDelayMilliseconds != null)
            {
                return TimeSpan.FromMilliseconds(Random.Value!.Next(MinimumDelayMilliseconds.Value, MaximumDelayMilliseconds.Value));
            }

            return _delay;
        }

        private set => _delay = value;
    }

    /// <summary>
    /// Gets a value indicating whether [use transformer].
    /// </summary>
    public bool UseTransformer { get; private set; }

    /// <summary>
    /// Gets the type of the transformer.
    /// </summary>
    public TransformerType TransformerType { get; private set; }

    /// <summary>
    /// Gets a value indicating whether to use the Handlebars transformer for the content from the referenced BodyAsFile.
    /// </summary>
    public bool UseTransformerForBodyAsFile { get; private set; }

    /// <summary>
    /// Gets the ReplaceNodeOptions to use when transforming a JSON node.
    /// </summary>
    public ReplaceNodeOptions TransformerReplaceNodeOptions { get; private set; }

    /// <inheritdoc />
    public IResponseMessage ResponseMessage { get; }

    /// <summary>
    /// Creates this instance.
    /// </summary>
    /// <param name="responseMessage">ResponseMessage</param>
    /// <returns>A <see cref="IResponseBuilder"/>.</returns>
    [PublicAPI]
    public static IResponseBuilder Create(ResponseMessage? responseMessage = null)
    {
        var message = responseMessage ?? new ResponseMessage();
        return new Response(message);
    }

    /// <summary>
    /// Creates this instance with the specified function.
    /// </summary>
    /// <param name="func">The callback function.</param>
    /// <returns>A <see cref="IResponseBuilder"/>.</returns>
    [PublicAPI]
    public static IResponseBuilder Create(Func<ResponseMessage> func)
    {
        Guard.NotNull(func);

        return new Response(func());
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Response"/> class.
    /// </summary>
    /// <param name="responseMessage">The response.</param>
    private Response(ResponseMessage responseMessage)
    {
        ResponseMessage = responseMessage;
    }

    /// <inheritdoc cref="IStatusCodeResponseBuilder.WithStatusCode(int)"/>
    [PublicAPI]
    public IResponseBuilder WithStatusCode(int code)
    {
        ResponseMessage.StatusCode = code;
        return this;
    }

    /// <inheritdoc cref="IStatusCodeResponseBuilder.WithStatusCode(string)"/>
    [PublicAPI]
    public IResponseBuilder WithStatusCode(string code)
    {
        ResponseMessage.StatusCode = code;
        return this;
    }

    /// <inheritdoc cref="IStatusCodeResponseBuilder.WithStatusCode(HttpStatusCode)"/>
    [PublicAPI]
    public IResponseBuilder WithStatusCode(HttpStatusCode code)
    {
        return WithStatusCode((int)code);
    }

    /// <summary>
    /// The with Success status code (200).
    /// </summary>
    /// <returns>A <see cref="IResponseBuilder"/>.</returns>
    [PublicAPI]
    public IResponseBuilder WithSuccess()
    {
        return WithStatusCode((int)HttpStatusCode.OK);
    }

    /// <summary>
    /// The with NotFound status code (404).
    /// </summary>
    /// <returns>The <see cref="IResponseBuilder"/>.</returns>
    [PublicAPI]
    public IResponseBuilder WithNotFound()
    {
        return WithStatusCode((int)HttpStatusCode.NotFound);
    }

    /// <inheritdoc />
    public IResponseBuilder WithDelay(TimeSpan delay)
    {
        Guard.Condition(delay, d => d == Timeout.InfiniteTimeSpan || d > TimeSpan.Zero);

        Delay = delay;
        return this;
    }

    /// <inheritdoc />
    public IResponseBuilder WithDelay(int milliseconds)
    {
        return WithDelay(TimeSpan.FromMilliseconds(milliseconds));
    }

    /// <inheritdoc />
    public IResponseBuilder WithRandomDelay(int minimumMilliseconds = 0, int maximumMilliseconds = 60_000)
    {
        Guard.Condition(minimumMilliseconds, min => min >= 0);
        Guard.Condition(maximumMilliseconds, max => max > minimumMilliseconds);

        MinimumDelayMilliseconds = minimumMilliseconds;
        MaximumDelayMilliseconds = maximumMilliseconds;

        return this;
    }

    /// <inheritdoc />
    public async Task<(IResponseMessage Message, IMapping? Mapping)> ProvideResponseAsync(IMapping mapping, IRequestMessage requestMessage, WireMockServerSettings settings)
    {
        Guard.NotNull(requestMessage);
        Guard.NotNull(settings);

        if (Delay != null)
        {
            await Task.Delay(Delay.Value).ConfigureAwait(false);
        }

        if (ProxyAndRecordSettings != null && _httpClientForProxy != null)
        {
            string RemoveFirstOccurrence(string source, string find)
            {
                int place = source.IndexOf(find, StringComparison.OrdinalIgnoreCase);
                return place >= 0 ? source.Remove(place, find.Length) : source;
            }

            var requestUri = new Uri(requestMessage.Url);

            // Build the proxy url and skip duplicates
            string extra = RemoveFirstOccurrence(requestUri.LocalPath.TrimEnd('/'), new Uri(ProxyAndRecordSettings.Url).LocalPath.TrimEnd('/'));
            requestMessage.ProxyUrl = ProxyAndRecordSettings.Url + extra + requestUri.Query;

            var proxyHelper = new ProxyHelper(settings);

            return await proxyHelper.SendAsync(
                mapping,
                ProxyAndRecordSettings,
                _httpClientForProxy,
                requestMessage,
                requestMessage.ProxyUrl
            ).ConfigureAwait(false);
        }

        IResponseMessage responseMessage;
        if (!WithCallbackUsed)
        {
            responseMessage = ResponseMessage;
        }
        else
        {
            if (Callback != null)
            {
                responseMessage = Callback(requestMessage);
            }
            else
            {
                responseMessage = await CallbackAsync!(requestMessage).ConfigureAwait(false);
            }

            // Copy StatusCode from ResponseMessage (if defined)
            if (ResponseMessage.StatusCode != null)
            {
                responseMessage.StatusCode = ResponseMessage.StatusCode;
            }

            // Copy Headers from ResponseMessage (if defined)
            if (ResponseMessage.Headers?.Count > 0)
            {
                responseMessage.Headers = ResponseMessage.Headers;
            }

            // Copy TrailingHeaders from ResponseMessage (if defined)
            if (ResponseMessage.TrailingHeaders?.Count > 0)
            {
                responseMessage.TrailingHeaders = ResponseMessage.TrailingHeaders;
            }
        }

        if (UseTransformer)
        {
            // If the body matcher is a RequestMessageProtoBufMatcher or BodyMatcher with a ProtoBufMatcher then try to decode the byte-array to a BodyAsJson.
            if (mapping.RequestMatcher is Request request && requestMessage is RequestMessage requestMessageImplementation)
            {
                if (request.TryGetProtoBufMatcher(out var protoBufMatcher))
                {
                    var decoded = await protoBufMatcher.DecodeAsync(requestMessage.BodyData?.BodyAsBytes).ConfigureAwait(false);
                    if (decoded != null)
                    {
                        requestMessageImplementation.BodyAsJson = JsonUtils.ConvertValueToJToken(decoded);
                    }
                }
            }

            ITransformer responseMessageTransformer;
            switch (TransformerType)
            {
                case TransformerType.Handlebars:
                    var factoryHandlebars = new HandlebarsContextFactory(settings);
                    responseMessageTransformer = new Transformer(settings, factoryHandlebars);
                    break;

                case TransformerType.Scriban:
                case TransformerType.ScribanDotLiquid:
                    var factoryDotLiquid = new ScribanContextFactory(settings.FileSystemHandler, TransformerType);
                    responseMessageTransformer = new Transformer(settings, factoryDotLiquid);
                    break;

                default:
                    throw new NotSupportedException($"TransformerType '{TransformerType}' is not supported.");
            }

            return (responseMessageTransformer.Transform(mapping, requestMessage, responseMessage, UseTransformerForBodyAsFile, TransformerReplaceNodeOptions), null);
        }

        if (!UseTransformer && ResponseMessage.BodyData?.BodyAsFileIsCached == true && responseMessage.BodyData?.BodyAsFile is not null)
        {
            ResponseMessage.BodyData.BodyAsBytes = settings.FileSystemHandler.ReadResponseBodyAsFile(responseMessage.BodyData.BodyAsFile);
        }

        return (responseMessage, null);
    }
}