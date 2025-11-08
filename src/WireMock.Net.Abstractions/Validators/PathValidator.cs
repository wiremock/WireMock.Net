// Copyright © WireMock.Net

using System;

namespace WireMock.Validators;

public static class PathValidator
{
    /// <summary>
    /// A valid path must start with a '/' and cannot be null, empty or whitespace.
    /// </summary>
    public static void ValidateAndThrow(string? path, string? paramName = null)
    {
        if (string.IsNullOrWhiteSpace(path) || path?.StartsWith("/") == false)
        {
            throw new ArgumentException("Path must start with a '/' and cannot be null, empty or whitespace.", paramName ?? nameof(path));
        }
    }
}