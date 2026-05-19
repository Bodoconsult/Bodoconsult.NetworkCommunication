// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using System.Reflection.Metadata;
using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlocks;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataLoggers;

/// <summary>
/// Implementation of <see cref="IInboundDataLogger"/> to log the datachunk content of all inbound SFXP messages for a certain channel to file
/// </summary>
public class SfxpDataChunkInboundDataLogger : IInboundDataLogger
{
    private readonly IMemoryDataExportService _dataExportService;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="dataExportService">Current data export service</param>
    public SfxpDataChunkInboundDataLogger(IDataExportService<byte[]> dataExportService)
    {
        if (dataExportService is not IMemoryDataExportService byteArrayDataExportService)
        {
            throw new ArgumentException("dataExportService is not ByteArrayDataExportService");
        }

        DataExportService = dataExportService;
        _dataExportService = byteArrayDataExportService;
    }

    /// <summary>
    /// Channel to log
    /// </summary>
    public byte Channel { get; set; } = 0xFF;

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
        if (message is not SfxpInboundDataMessage sfxp)
        {
            return false;
        }

        return sfxp.DataBlock is SfxpInboundDatablock;
    }

    /// <summary>
    /// Log a data message
    /// </summary>
    /// <param name="message">Data message to log</param>
    public void LogTheMessage(IInboundDataMessage message)
    {
        if (message is not SfxpInboundDataMessage sfxp)
        {
            return;
        }

        if (sfxp.DataBlock is not SfxpInboundDatablock db)
        {
            return;
        }

        foreach (var chunk in db.DataChunks.Where(x => x.Channel == Channel).ToList())
        {
            if (!chunk.Data.HasValue)
            {
                continue;
            }
            //Debug.Print($"{chunk.Channel}: {chunk.Data.Value.Length}");

            _dataExportService.Add(chunk.Data.Value);
        }
    }
}