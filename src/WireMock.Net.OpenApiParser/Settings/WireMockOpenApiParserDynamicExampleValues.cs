// Copyright © WireMock.Net

using System;
using Microsoft.OpenApi;
using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;

namespace WireMock.Net.OpenApiParser.Settings;

/// <summary>
/// A class defining the random example values to use for the different types.
/// </summary>
public class WireMockOpenApiParserDynamicExampleValues : IWireMockOpenApiParserExampleValues
{
    /// <inheritdoc />
    public virtual bool Boolean => RandomizerFactory.GetRandomizer(new FieldOptionsBoolean()).Generate() ?? true;

    /// <inheritdoc />
    public virtual int Integer => RandomizerFactory.GetRandomizer(new FieldOptionsInteger()).Generate() ?? 42;

    /// <inheritdoc />
    public virtual float Float => RandomizerFactory.GetRandomizer(new FieldOptionsFloat()).Generate() ?? 4.2f;

    /// <inheritdoc />
    public virtual decimal Decimal => SafeConvertFloatToDecimal(RandomizerFactory.GetRandomizer(new FieldOptionsFloat()).Generate() ?? 4.2f);

    /// <inheritdoc />
    public virtual Func<DateTime> Date => () => RandomizerFactory.GetRandomizer(new FieldOptionsDateTime()).Generate() ?? System.DateTime.UtcNow.Date;

    /// <inheritdoc />
    public virtual Func<DateTime> DateTime => () => RandomizerFactory.GetRandomizer(new FieldOptionsDateTime()).Generate() ?? System.DateTime.UtcNow;

    /// <inheritdoc />
    public virtual byte[] Bytes => RandomizerFactory.GetRandomizer(new FieldOptionsBytes()).Generate();

    /// <inheritdoc />
    public virtual object Object => "example-object";

    /// <inheritdoc />
    public virtual string String => RandomizerFactory.GetRandomizer(new FieldOptionsTextRegex { Pattern = @"^[0-9]{2}[A-Z]{5}[0-9]{2}" }).Generate() ?? "example-string";

    /// <inheritdoc />
    public virtual IOpenApiSchema? Schema { get; set; }

    /// <summary>
    /// Safely converts a float to a decimal, ensuring the value stays within the bounds of a decimal.
    /// </summary>
    /// <param name="value">The float value to convert.</param>
    /// <returns>A decimal value within the valid range of a decimal.</returns>
    private static decimal SafeConvertFloatToDecimal(float value)
    {
        return value switch
        {
            < (float)decimal.MinValue => decimal.MinValue,
            > (float)decimal.MaxValue => decimal.MaxValue,
            _ => (decimal)value
        };
    }
}