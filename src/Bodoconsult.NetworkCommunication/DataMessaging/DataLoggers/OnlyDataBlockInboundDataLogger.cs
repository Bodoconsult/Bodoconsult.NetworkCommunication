// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataLoggers;

/// <summary>
/// Implementation of <see cref="IInboundDataLogger"/> to log the datablock content of all inbound data messages to file
/// </summary>
public class OnlyDataBlockInboundDataLogger : IInboundDataLogger
{
    private readonly IMemoryDataExportService _dataExportService;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="dataExportService">Current data export service</param>
    public OnlyDataBlockInboundDataLogger(IDataExportService<byte[]> dataExportService)
    {
        if (dataExportService is not IMemoryDataExportService byteArrayDataExportService)
        {
            throw new ArgumentException("dataExportService is not ByteArrayDataExportService");
        }

        DataExportService = dataExportService;
        _dataExportService = byteArrayDataExportService;
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
    public List<Memory<byte>> CheckIfMessageIsToLog(IInboundDataMessage message)
    {
        if (message.DataBlock != null)
        {
            return [message.DataBlock.Data];
        }
        return [];
    }

    /// <summary>
    /// Log messages
    /// </summary>
    /// <param name="messages">Messages to log</param>
    public void LogTheMessages(List<Memory<byte>> messages)
    {
        foreach (var message in messages)
        {
            //Debug.Print($"{chunk.Channel}: {chunk.Data.Value.Length}");

            _dataExportService.Add(message);
        }
    }
}