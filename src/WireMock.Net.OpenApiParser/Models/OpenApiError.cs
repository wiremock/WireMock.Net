// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace WireMock.Net.OpenApiParser.Models;

/// <summary>
/// Error related to the Open API Document.
/// </summary>
public class OpenApiError
{
    /// <summary>
    /// Initializes the <see cref="OpenApiError"/> class.
    /// </summary>
    public OpenApiError(string? pointer, string message)
    {
        Pointer = pointer;
        Message = message;
    }

    /// <summary>
    /// Initializes a copy of an <see cref="OpenApiError"/> object
    /// </summary>
    public OpenApiError(OpenApiError error)
    {
        Pointer = error.Pointer;
        Message = error.Message;
    }

    /// <summary>
    /// Message explaining the error.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Pointer to the location of the error.
    /// </summary>
    public string? Pointer { get; set; }

    /// <summary>
    /// Gets the string representation of <see cref="OpenApiError"/>.
    /// </summary>
    public override string ToString()
    {
        return Message + (!string.IsNullOrEmpty(Pointer) ? " [" + Pointer + "]" : "");
    }
}