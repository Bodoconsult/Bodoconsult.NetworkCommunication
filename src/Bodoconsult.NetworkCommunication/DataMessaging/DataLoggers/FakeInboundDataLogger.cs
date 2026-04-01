// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.App.Abstractions.DataExportServices;
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
    /// <returns>True if the message is a candiate for logging with the current logger else false</returns>
    public bool CheckIfMessageIsToLog(IInboundDataMessage message)
    {
        // Do not filter anything
        return true;
    }

    /// <summary>
    /// Log a data message
    /// </summary>
    /// <param name="message">Data message to log</param>
    public void LogTheMessage(IInboundDataMessage message)
    {
        WasLogged = true;
    }
}