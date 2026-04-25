// Copyright © WireMock.Net

using JetBrains.Annotations;
using WireMock.Handlers;

namespace WireMock.Transformers;

/// <summary>
/// Defines the transformer context used to render and evaluate templates during response transformation.
/// </summary>
[PublicAPI]
public interface ITransformerContext
{
    /// <summary>
    /// Gets the file system handler used by the current transformer context.
    /// </summary>
    IFileSystemHandler FileSystemHandler { get; }

    /// <summary>
    /// Renders the specified template text using the supplied model.
    /// </summary>
    /// <param name="text">The template text to render.</param>
    /// <param name="model">The model used during rendering.</param>
    /// <returns>The rendered text.</returns>
    string ParseAndRender(string text, object model);

    /// <summary>
    /// Evaluates the specified template text using the supplied model.
    /// </summary>
    /// <param name="text">The template text to evaluate.</param>
    /// <param name="model">The model used during evaluation.</param>
    /// <returns>The evaluated value.</returns>
    object? ParseAndEvaluate(string text, object model);
}
