// Copyright © WireMock.Net

using System;
using GraphQL.Types;

// ReSharper disable once CheckNamespace
namespace WireMock.GraphQL.Models;

/// <inheritdoc />
public abstract class WireMockCustomScalarGraphType<T> : ScalarGraphType
{
    /// <inheritdoc />
    public override object? ParseValue(object? value)
    {
        switch (value)
        {
            case null:
                return null;

            case T:
                return value;
        }

        if (value is string && typeof(T) != typeof(string))
        {
            throw new InvalidCastException($"Unable to convert value '{value}' of type '{typeof(string)}' to type '{typeof(T)}'.");
        }

        return (T)Convert.ChangeType(value, typeof(T));
    }
}