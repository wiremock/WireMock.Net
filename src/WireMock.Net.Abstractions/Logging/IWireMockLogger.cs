// Copyright © WireMock.Net

using JetBrains.Annotations;
using WireMock.Admin.Requests;

namespace WireMock.Logging;

/// <summary>
/// IWireMockLogger interface
/// </summary>
[PublicAPI]
public interface IWireMockLogger
{
    /// <summary>
    /// Writes the message at the Debug level using the specified parameters. 
    /// </summary>
    /// <param name="formatString">The format string.</param>
    /// <param name="args">The arguments.</param>
    [PublicAPI]
    [StringFormatMethod("formatString")]
    void Debug(string formatString, params object[] args);

    /// <summary>
    /// Writes the message at the Info level using the specified parameters. 
    /// </summary>
    /// <param name="formatString">The format string.</param>
    /// <param name="args">The arguments.</param>
    [PublicAPI]
    [StringFormatMethod("formatString")]
    void Info(string formatString, params object[] args);

    /// <summary>
    /// Writes the message at the Warning level using the specified parameters. 
    /// </summary>
    /// <param name="formatString">The format string.</param>
    /// <param name="args">The arguments.</param>
    [PublicAPI]
    [StringFormatMethod("formatString")]
    void Warn(string formatString, params object[] args);

    /// <summary>
    /// Writes the message at the Error level using the specified parameters. 
    /// </summary>
    /// <param name="formatString">The format string.</param>
    /// <param name="args">The arguments.</param>
    [PublicAPI]
    [StringFormatMethod("formatString")]
    void Error(string formatString, params object[] args);

    /// <summary>
    /// Writes the message at the Error level using the specified exception. 
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    [PublicAPI]
    void Error(string message, Exception exception);

    /// <summary>
    /// Writes the LogEntryModel (LogRequestModel, LogResponseModel and more).
    /// </summary>
    /// <param name="logEntryModel">The Request Log Model.</param>
    /// <param name="isAdminRequest">Defines if this request is an admin request.</param>
    [PublicAPI]
    void DebugRequestResponse(LogEntryModel logEntryModel, bool isAdminRequest);
}