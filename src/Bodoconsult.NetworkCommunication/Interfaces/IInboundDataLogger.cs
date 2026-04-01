// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.App.Abstractions.DataExportServices;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for logger implementation for data messages on a low level
/// </summary>
public interface IInboundDataLogger
{
    /// <summary>
    /// Current data export service
    /// </summary>
    IDataExportService<byte[]> DataExportService { get; }

    /// <summary>
    /// Start the data export
    /// </summary>
    void Start();

    /// <summary>
    /// Save all data and then stop the data export
    /// </summary>
    void Stop();

    /// <summary>
    /// Check if the message is to log. A message can be logged by zero or one logger maximum.
    /// </summary>
    /// <param name="message">Data message to check for logging</param>
    /// <returns>True if the message is a candiate for logging with the current logger else false</returns>
    bool CheckIfMessageIsToLog(IInboundDataMessage message);

    /// <summary>
    /// Log a data message
    /// </summary>
    /// <param name="message">Data message to log</param>
    void LogTheMessage(IInboundDataMessage message);
}