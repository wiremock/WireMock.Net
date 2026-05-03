// Copyright © WireMock.Net

using Stef.Validation;
using WireMock.Settings;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.Transformers;

internal class Transformer : ITransformer
{
    private readonly IJsonBodyTransformer _jsonBodyTransformer;
    private readonly ITransformerContextFactory _factory;

    public Transformer(WireMockServerSettings settings, ITransformerContextFactory factory)
    {
        _factory = Guard.NotNull(factory);
        _jsonBodyTransformer = Guard.NotNull(settings).DefaultJsonBodyTransformer;
    }

    public IBodyData? TransformBody(
        IMapping mapping,
        IRequestMessage originalRequestMessage,
        IResponseMessage originalResponseMessage,
        IBodyData? bodyData,
        ReplaceNodeOptions options)
    {
        var (transformerContext, model) = Create(mapping, originalRequestMessage, originalResponseMessage);

        IBodyData? newBodyData = null;
        if (bodyData?.DetectedBodyType != null)
        {
            newBodyData = TransformBodyData(transformerContext, options, model, bodyData, false);
        }

        return newBodyData;
    }

    public IDictionary<string, WireMockList<string>> TransformHeaders(
        IMapping mapping,
        IRequestMessage originalRequestMessage,
        IResponseMessage originalResponseMessage,
        IDictionary<string, WireMockList<string>>? headers
    )
    {
        var (transformerContext, model) = Create(mapping, originalRequestMessage, originalResponseMessage);

        return TransformHeaders(transformerContext, model, headers);
    }

    public string TransformString(
        IMapping mapping,
        IRequestMessage originalRequestMessage,
        IResponseMessage originalResponseMessage,
        string? value
    )
    {
        if (value is null)
        {
            return string.Empty;
        }

        var (transformerContext, model) = Create(mapping, originalRequestMessage, originalResponseMessage);
        return transformerContext.ParseAndRender(value, model);
    }

    public string Transform(string template, object? model)
    {
        return model is null ? string.Empty : _factory.Create().ParseAndRender(template, model);
    }

    public ResponseMessage Transform(IMapping mapping, IRequestMessage requestMessage, IResponseMessage original, bool useTransformerForBodyAsFile, ReplaceNodeOptions options)
    {
        var responseMessage = new ResponseMessage();

        var (transformerContext, model) = Create(mapping, requestMessage, null);

        if (original.BodyData?.DetectedBodyType != null)
        {
            responseMessage.BodyData = TransformBodyData(transformerContext, options, model, original.BodyData, useTransformerForBodyAsFile);

            if (original.BodyData.DetectedBodyType is BodyType.String or BodyType.FormUrlEncoded)
            {
                responseMessage.BodyOriginal = original.BodyData.BodyAsString;
            }
        }

        responseMessage.FaultType = original.FaultType;
        responseMessage.FaultPercentage = original.FaultPercentage;

        responseMessage.Headers = TransformHeaders(transformerContext, model, original.Headers);
        responseMessage.TrailingHeaders = TransformHeaders(transformerContext, model, original.TrailingHeaders);

        responseMessage.StatusCode = original.StatusCode switch
        {
            int statusCodeAsInteger => statusCodeAsInteger,
            string statusCodeAsString => transformerContext.ParseAndRender(statusCodeAsString, model),
            _ => responseMessage.StatusCode
        };

        return responseMessage;
    }

    private (ITransformerContext TransformerContext, TransformModel Model) Create(IMapping mapping, IRequestMessage request, IResponseMessage? response)
    {
        return (_factory.Create(), new TransformModel
        {
            mapping = mapping,
            request = request,
            response = response,
            data = mapping.Data ?? new { }
        });
    }

    private BodyData? TransformBodyData(ITransformerContext transformerContext, ReplaceNodeOptions options, TransformModel model, IBodyData original, bool useTransformerForBodyAsFile)
    {
        switch (original.DetectedBodyType)
        {
            case BodyType.Json:
            case BodyType.ProtoBuf:
                return _jsonBodyTransformer.TransformBodyAsJson(
                    transformerContext,
                    options,
                    model,
                    original);

            case BodyType.File:
                return TransformBodyAsFile(transformerContext, model, original, useTransformerForBodyAsFile);

            case BodyType.String or BodyType.FormUrlEncoded:
                return TransformBodyAsString(transformerContext, model, original);

            default:
                return null;
        }
    }

    private static IDictionary<string, WireMockList<string>> TransformHeaders(ITransformerContext transformerContext, TransformModel model, IDictionary<string, WireMockList<string>>? original)
    {
        if (original == null)
        {
            return new Dictionary<string, WireMockList<string>>();
        }

        var newHeaders = new Dictionary<string, WireMockList<string>>();
        foreach (var header in original)
        {
            var headerKey = transformerContext.ParseAndRender(header.Key, model);
            var templateHeaderValues = header.Value.Select(text => transformerContext.ParseAndRender(text, model)).ToArray();

            newHeaders.Add(headerKey, new WireMockList<string>(templateHeaderValues));
        }

        return newHeaders;
    }

    private static BodyData TransformBodyAsString(ITransformerContext transformerContext, object model, IBodyData original)
    {
        return new BodyData
        {
            Encoding = original.Encoding,
            DetectedBodyType = original.DetectedBodyType,
            DetectedBodyTypeFromContentType = original.DetectedBodyTypeFromContentType,
            BodyAsString = transformerContext.ParseAndRender(original.BodyAsString!, model)
        };
    }

    private static BodyData TransformBodyAsFile(ITransformerContext transformerContext, object model, IBodyData original, bool useTransformerForBodyAsFile)
    {
        var transformedBodyAsFilename = transformerContext.ParseAndRender(original.BodyAsFile!, model);

        if (!useTransformerForBodyAsFile)
        {
            return new BodyData
            {
                DetectedBodyType = original.DetectedBodyType,
                DetectedBodyTypeFromContentType = original.DetectedBodyTypeFromContentType,
                BodyAsFile = transformedBodyAsFilename
            };
        }

        var text = transformerContext.FileSystemHandler.ReadResponseBodyAsString(transformedBodyAsFilename);
        return new BodyData
        {
            DetectedBodyType = BodyType.String,
            DetectedBodyTypeFromContentType = original.DetectedBodyTypeFromContentType,
            BodyAsString = transformerContext.ParseAndRender(text, model),
            BodyAsFile = transformedBodyAsFilename
        };
    }
}