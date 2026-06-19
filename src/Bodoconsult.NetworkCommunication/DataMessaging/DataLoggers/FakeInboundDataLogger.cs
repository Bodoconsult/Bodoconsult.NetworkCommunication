// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.DataExportServices;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataLoggers;

/// <summary>
/// Fake implementation of <see cref="IInboundDataLogger"/> to log all inbound data messages to nirwana
/// </summary>
public class FakeInboundDataLogger : IInboundDataLogger
{
    /// <summary>
    /// Data were logged
    /// </summary>
    public bool WasLogged { get; set; }

    /// <summary>
    /// Current data export service
    /// </summary>
    public IDataExportService<byte[]> DataExportService { get; } = new FakeDataExportService();

    /// <summary>
    /// Start the data export
    /// </summary>
    public void Start()
    {
        DataExportService.Start();
    }

    /// <summary>
    /// Flush the cache to disk
    /// </summary>
    public void FlushCache()
    {
        DataExportService.FlushCache();
    }

    /// <summary>
    /// Save all data and then stop the data export
    /// </summary>
    public void Stop()
    {
        DataExportService.Stop();
    }

    /// <summary>
    /// Check if the message is to log. A message can be logged by zero or one logger maximum.
    /// </summary>
    /// <param name="message">Data message to check for logging</param>
    /// <returns>A list with array items to log or empty list</returns>
    public List<Memory<byte>> CheckIfMessageIsToLog(IInboundDataMessage message)
    {
        // Do not filter anything
        return [message.RawMessageData];
    }

    /// <summary>
    /// Log messages
    /// </summary>
    /// <param name="messages">Messages to log</param>
    public void LogTheMessages(List<Memory<byte>> messages)
    {
        WasLogged = true;
    }
}