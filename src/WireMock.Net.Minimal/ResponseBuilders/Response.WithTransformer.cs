// Copyright © WireMock.Net

using System;
using WireMock.Types;

namespace WireMock.ResponseBuilders;

public partial class Response
{
    /// <inheritdoc cref="ITransformResponseBuilder.WithTransformer(bool)"/>
    public IResponseBuilder WithTransformer(bool transformContentFromBodyAsFile)
    {
        return WithTransformer(TransformerType.Handlebars, transformContentFromBodyAsFile);
    }

    /// <inheritdoc cref="ITransformResponseBuilder.WithTransformer(ReplaceNodeOptions)"/>
    public IResponseBuilder WithTransformer(ReplaceNodeOptions options)
    {
        return WithTransformer(TransformerType.Handlebars, false, options);
    }

    /// <inheritdoc />
    public IResponseBuilder WithTransformer(TransformerType transformerType = TransformerType.Handlebars, bool transformContentFromBodyAsFile = false, ReplaceNodeOptions options = ReplaceNodeOptions.EvaluateAndTryToConvert)
    {
        if (_bodyFromFileSet)
        {
            throw new InvalidOperationException("WithTransformer should be used before WithBodyFromFile.");
        }

        UseTransformer = true;
        TransformerType = transformerType;
        UseTransformerForBodyAsFile = transformContentFromBodyAsFile;
        TransformerReplaceNodeOptions = options;
        return this;
    }
}