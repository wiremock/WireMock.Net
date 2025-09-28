// Copyright © WireMock.Net

using System.Diagnostics.CodeAnalysis;
using WireMock.Types;

namespace WireMock.Settings;

/// <summary>
/// Defines an old path param and a new path param to be replaced when proxying.
/// </summary>
public class ProxyUrlReplaceSettings
{
    /// <summary>
    /// The old path value to be replaced by the new path value.
    /// </summary>
    public string? OldValue { get; set; }

    /// <summary>
    /// The new path value to replace the old value with.
    /// </summary>
    public string? NewValue { get; set; }

    /// <summary>
    /// Defines if the case should be ignored when replacing.
    /// </summary>
    public bool IgnoreCase { get; set; }

    /// <summary>
    /// The new path value to replace the old value with.
    /// </summary>
    public string? TransformTemplate { get; set; }

    /// <summary>
    /// Use Transformer.
    /// </summary>
    [MemberNotNullWhen(true, nameof(TransformTemplate))]
    [MemberNotNullWhen(false, nameof(OldValue))]
    [MemberNotNullWhen(false, nameof(NewValue))]
    public bool UseTransformer => !string.IsNullOrEmpty(TransformTemplate);

    /// <summary>
    /// The transformer type, in case <see cref="UseTransformer"/> is set to <c>true</c>.
    /// </summary>
    public TransformerType TransformerType { get; set; } = TransformerType.Handlebars;
}