// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;

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
    /// Flush the cache to disk
    /// </summary>
    void FlushCache();

    /// <summary>
    /// Save all data and then stop the data export
    /// </summary>
    void Stop();

    /// <summary>
    /// Check if the message is to log. A message can be logged by zero or one logger maximum.
    /// </summary>
    /// <param name="message">Data message to check for logging</param>
    /// <returns>A list with array items to log or empty list</returns>
    List<Memory<byte>> CheckIfMessageIsToLog(IInboundDataMessage message);

    /// <summary>
    /// Log messages
    /// </summary>
    /// <param name="messages">Messages to log</param>
    void LogTheMessages(List<Memory<byte>> messages);
}