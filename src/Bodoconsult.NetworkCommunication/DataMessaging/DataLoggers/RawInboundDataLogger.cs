// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.App.Abstractions.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataLoggers;

/// <summary>
/// Implementation of <see cref="IInboundDataLogger"/> to log all inbound data messages to file
/// </summary>
public class RawInboundDataLogger: IInboundDataLogger
{
    private readonly IMemoryDataExportService _dataExportService;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="dataExportService">Current data export service</param>
    public RawInboundDataLogger(IDataExportService<byte[]> dataExportService)
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
        if (message.RawMessageData.Length == 0)
        {
            return [];
        }
        return [message.RawMessageData];
    }


    /// <summary>
    /// Log messages
    /// </summary>
    /// <param name="messages">Messages to log</param>
    public void LogTheMessages(List<Memory<byte>> messages)
    {
        foreach (var message in messages)
        {
            _dataExportService.Add(message);
        }
    }
}