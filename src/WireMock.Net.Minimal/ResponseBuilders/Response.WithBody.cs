// Copyright © WireMock.Net

using System;
using System.Text;
using System.Threading.Tasks;
using JsonConverter.Abstractions;
using Stef.Validation;
using WireMock.Models;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.ResponseBuilders;

public partial class Response
{
    private bool _bodyFromFileSet;

    /// <inheritdoc />
    public IResponseBuilder WithBody(Func<IRequestMessage, string> bodyFactory, string? destination = BodyDestinationFormat.SameAsSource, Encoding? encoding = null)
    {
        Guard.NotNull(bodyFactory);

        return WithCallbackInternal(true, req => new ResponseMessage
        {
            BodyData = new BodyData
            {
                DetectedBodyType = BodyType.String,
                BodyAsString = bodyFactory(req),
                Encoding = encoding ?? Encoding.UTF8,
                IsFuncUsed = "Func<IRequestMessage, string>"
            }
        });
    }

    /// <inheritdoc />
    public IResponseBuilder WithBody(Func<IRequestMessage, Task<string>> bodyFactory, string? destination = BodyDestinationFormat.SameAsSource, Encoding? encoding = null)
    {
        Guard.NotNull(bodyFactory);

        return WithCallbackInternal(true, async req => new ResponseMessage
        {
            BodyData = new BodyData
            {
                DetectedBodyType = BodyType.String,
                BodyAsString = await bodyFactory(req).ConfigureAwait(false),
                Encoding = encoding ?? Encoding.UTF8,
                IsFuncUsed = "Func<IRequestMessage, Task<string>>"
            }
        });
    }

    /// <inheritdoc />
    public IResponseBuilder WithSseBody(Func<IRequestMessage, IBlockingQueue<string?>, Task> bodyFactory, TimeSpan? timeout = null)
    {
        Guard.NotNull(bodyFactory);

        var queue = new BlockingQueue<string?>(timeout);

        return WithCallbackInternal(true, req => new ResponseMessage
        {
            BodyData = new BodyData
            {
                DetectedBodyType = BodyType.SseString,
                SseStringQueue = queue,
                BodyAsSseStringTask = bodyFactory(req, queue),
                Encoding = Encoding.UTF8,
                IsFuncUsed = "Func<IRequestMessage, BlockingQueue<string?>, Task>"
            }
        });
    }

    /// <inheritdoc />
    public IResponseBuilder WithBody(byte[] body, string? destination = BodyDestinationFormat.SameAsSource, Encoding? encoding = null)
    {
        Guard.NotNull(body);

        ResponseMessage.BodyDestination = destination;
        ResponseMessage.BodyData = new BodyData();

        switch (destination)
        {
            case BodyDestinationFormat.String:
                var enc = encoding ?? Encoding.UTF8;
                ResponseMessage.BodyData.DetectedBodyType = BodyType.String;
                ResponseMessage.BodyData.BodyAsString = enc.GetString(body);
                ResponseMessage.BodyData.Encoding = enc;
                break;

            default:
                ResponseMessage.BodyData.DetectedBodyType = BodyType.Bytes;
                ResponseMessage.BodyData.BodyAsBytes = body;
                break;
        }

        return this;
    }

    /// <inheritdoc />
    public IResponseBuilder WithBodyFromFile(string filename, bool cache = true)
    {
        Guard.NotNull(filename);

        _bodyFromFileSet = true;

        ResponseMessage.BodyData = new BodyData
        {
            BodyAsFileIsCached = cache,
            BodyAsFile = filename
        };

        if (cache && !UseTransformer)
        {
            ResponseMessage.BodyData.DetectedBodyType = BodyType.Bytes;
        }
        else
        {
            ResponseMessage.BodyData.DetectedBodyType = BodyType.File;
        }

        return this;
    }

    /// <inheritdoc />
    public IResponseBuilder WithBody(string body, string? destination = BodyDestinationFormat.SameAsSource, Encoding? encoding = null)
    {
        Guard.NotNull(body);

        encoding ??= Encoding.UTF8;

        ResponseMessage.BodyDestination = destination;
        ResponseMessage.BodyData = new BodyData
        {
            Encoding = encoding
        };

        switch (destination)
        {
            case BodyDestinationFormat.Bytes:
                ResponseMessage.BodyData.DetectedBodyType = BodyType.Bytes;
                ResponseMessage.BodyData.BodyAsBytes = encoding.GetBytes(body);
                break;

            case BodyDestinationFormat.Json:
                ResponseMessage.BodyData.DetectedBodyType = BodyType.Json;
                ResponseMessage.BodyData.BodyAsJson = JsonUtils.DeserializeObject(body);
                break;

            default:
                ResponseMessage.BodyData.DetectedBodyType = BodyType.String;
                ResponseMessage.BodyData.BodyAsString = body;
                break;
        }

        return this;
    }

    /// <inheritdoc />
    public IResponseBuilder WithBodyAsJson(object body, Encoding? encoding = null, bool? indented = null)
    {
        Guard.NotNull(body);

        ResponseMessage.BodyDestination = null;
        ResponseMessage.BodyData = new BodyData
        {
            Encoding = encoding,
            DetectedBodyType = BodyType.Json,
            BodyAsJson = body,
            BodyAsJsonIndented = indented
        };

        return this;
    }

    /// <inheritdoc />
    public IResponseBuilder WithBodyAsJson(object body, bool indented)
    {
        return WithBodyAsJson(body, null, indented);
    }

    /// <inheritdoc />
    public IResponseBuilder WithBodyAsJson(Func<IRequestMessage, object> bodyFactory, Encoding? encoding = null)
    {
        Guard.NotNull(bodyFactory);

        return WithCallbackInternal(true, req => new ResponseMessage
        {
            BodyData = new BodyData
            {
                Encoding = encoding ?? Encoding.UTF8,
                DetectedBodyType = BodyType.Json,
                BodyAsJson = bodyFactory(req),
                IsFuncUsed = "Func<IRequestMessage, object>"
            }
        });
    }

    /// <inheritdoc />
    public IResponseBuilder WithBodyAsJson(Func<IRequestMessage, Task<object>> bodyFactory, Encoding? encoding = null)
    {
        Guard.NotNull(bodyFactory);

        return WithCallbackInternal(true, async req => new ResponseMessage
        {
            BodyData = new BodyData
            {
                Encoding = encoding ?? Encoding.UTF8,
                DetectedBodyType = BodyType.Json,
                BodyAsJson = await bodyFactory(req).ConfigureAwait(false),
                IsFuncUsed = "Func<IRequestMessage, Task<object>>"
            }
        });
    }

    /// <inheritdoc />
    public IResponseBuilder WithBody(object body, IJsonConverter jsonConverter, JsonConverterOptions? options = null)
    {
        return WithBody(body, null, jsonConverter, options);
    }

    /// <inheritdoc />
    public IResponseBuilder WithBody(object body, Encoding? encoding, IJsonConverter jsonConverter, JsonConverterOptions? options = null)
    {
        Guard.NotNull(body);
        Guard.NotNull(jsonConverter);

        ResponseMessage.BodyDestination = null;
        ResponseMessage.BodyData = new BodyData
        {
            Encoding = encoding,
            DetectedBodyType = BodyType.String,
            BodyAsString = jsonConverter.Serialize(body, options)
        };

        return this;
    }
}