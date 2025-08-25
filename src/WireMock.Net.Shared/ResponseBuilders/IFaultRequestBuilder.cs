// Copyright © WireMock.Net

using JetBrains.Annotations;

namespace WireMock.ResponseBuilders;

/// <summary>
/// The FaultRequestBuilder interface.
/// </summary>
public interface IFaultResponseBuilder : ITransformResponseBuilder
{
    /// <summary>
    /// WithBody : Create a fault response.
    /// </summary>
    /// <param name="faultType">The FaultType.</param>
    /// <param name="percentage">The percentage when this fault should occur. When null, it's always a fault.</param>
    /// <returns>A <see cref="IResponseBuilder"/>.</returns>
    IResponseBuilder WithFault(FaultType faultType, double? percentage = null);
}