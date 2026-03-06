// Copyright © WireMock.Net

using System.Net;

// ReSharper disable once CheckNamespace
namespace WireMock.Admin.Mappings;

/// <summary>
/// ResponseModelBuilder
/// </summary>
public partial class ResponseModelBuilder
{
    /// <summary>
    /// Set the StatusCode.
    /// </summary>
    public ResponseModelBuilder WithStatusCode(int value) => WithStatusCode(() => value);

    /// <summary>
    /// Set the StatusCode.
    /// </summary>
    public ResponseModelBuilder WithStatusCode(HttpStatusCode value) => WithStatusCode(() => value);

    /// <summary>
    /// Set the Delay.
    /// </summary>
    public ResponseModelBuilder WithDelay(TimeSpan value) => WithDelay((int) value.TotalMilliseconds);

    /// <summary>
    /// Set the MinimumRandomDelay.
    /// </summary>
    public ResponseModelBuilder WithMinimumRandomDelay(TimeSpan value) => WithMinimumRandomDelay((int)value.TotalMilliseconds);

    /// <summary>
    /// Set the MaximumRandomDelay.
    /// </summary>
    public ResponseModelBuilder WithMaximumRandomDelay(TimeSpan value) => WithMaximumRandomDelay((int)value.TotalMilliseconds);
}