// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataLoggers;

/// <summary>
/// Implementation of <see cref="IInboundDataLogger"/> to log the datablock content of all inbound data messages to file
/// </summary>
public class OnlyDataBlockInboundDataLogger : IInboundDataLogger
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="dataExportService">Current data export service</param>
    public OnlyDataBlockInboundDataLogger(IDataExportService<byte[]> dataExportService)
    {
        DataExportService = dataExportService;
    }

    /// <summary>
    /// Current data export service
    /// </summary>
    public IDataExportService<byte[]> DataExportService { get; }

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
        // Do not filter anything but messages without datablock
        return message.DataBlock != null;
    }

    /// <summary>
    /// Log a data message
    /// </summary>
    /// <param name="message">Data message to log</param>
    public void LogTheMessage(IInboundDataMessage message)
    {
        if (message.DataBlock == null)
        {
            return;
        }

        DataExportService.Add(message.DataBlock.Data.ToArray());
    }
}