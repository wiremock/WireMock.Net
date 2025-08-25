// Copyright © WireMock.Net

using HandlebarsDotNet.Helpers.Enums;
using JetBrains.Annotations;
using WireMock.Types;

namespace WireMock.Settings;

/// <summary>
/// HandlebarsSettings
/// </summary>
[PublicAPI]
public class HandlebarsSettings
{
    internal static readonly Category[] DefaultAllowedHandlebarsHelpers =
    [
        Category.Boolean,
        Category.Constants,
        Category.DateTime,
        Category.Enumerable,
        Category.Humanizer,
        Category.JsonPath,
        Category.Math,
        Category.Object,
        Category.Random,
        Category.Regex,
        Category.String,
        Category.Url,
        Category.Xeger,
        Category.XPath,
        Category.Xslt
    ];

    /// <summary>
    /// Defines the allowed custom HandlebarsHelpers which can be used. Possible values are:
    /// - <see cref="CustomHandlebarsHelpers.None"/> (Default)
    /// - <see cref="CustomHandlebarsHelpers.File"/>
    /// - <see cref="CustomHandlebarsHelpers.All"/>
    /// </summary>
    [PublicAPI]
    public CustomHandlebarsHelpers AllowedCustomHandlebarsHelpers { get; set; } = CustomHandlebarsHelpers.None;

    /// <summary>
    /// Defines the allowed HandlebarHelpers which can be used.
    ///
    /// By default, all categories except <see cref="Category.DynamicLinq"/> and <see cref="Category.Environment"/> are registered.
    /// </summary>
    [PublicAPI]
    public Category[] AllowedHandlebarsHelpers { get; set; } = DefaultAllowedHandlebarsHelpers;
}