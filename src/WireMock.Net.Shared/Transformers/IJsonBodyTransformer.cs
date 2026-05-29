// Copyright © WireMock.Net

using JetBrains.Annotations;
using WireMock.Types;
using WireMock.Util;

namespace WireMock.Transformers;

/// <summary>
/// Defines the contract for transforming JSON-like body data using a transformer context.
/// </summary>
[PublicAPI]
public interface IJsonBodyTransformer
{
    /// <summary>
    /// Transforms the JSON body using the provided transformer context and model.
    /// </summary>
    /// <param name="transformerContext">The transformer context used to render and evaluate template values.</param>
    /// <param name="options">The JSON node replacement behavior to apply during transformation.</param>
    /// <param name="model">The model used when rendering or evaluating template values.</param>
    /// <param name="original">The original body data to transform.</param>
    /// <returns>The transformed JSON body data.</returns>
    BodyData TransformBodyAsJson(
        ITransformerContext transformerContext,
        ReplaceNodeOptions options,
        object model,
        IBodyData original);
}
