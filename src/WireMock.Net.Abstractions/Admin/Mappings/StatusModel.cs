// Copyright © WireMock.Net

namespace WireMock.Admin.Mappings;

/// <summary>
/// Status
/// </summary>
[FluentBuilder.AutoGenerateBuilder]
public class StatusModel
{
    /// <summary>
    /// The optional guid.
    /// </summary>
    public Guid? Guid { get; set; }

    /// <summary>
    /// The status.
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// The error message.
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Returns a string that represents the current status model, including its unique identifier, status, and error information.
    /// </summary>
    /// <returns>A string containing the values of the Guid, Status, and Error properties formatted for display.</returns>
    public override string ToString()
    {
        return $"StatusModel [Guid={Guid}, Status={Status}, Error={Error}]";
    }
}