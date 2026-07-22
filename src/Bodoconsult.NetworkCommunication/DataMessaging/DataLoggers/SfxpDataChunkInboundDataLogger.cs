// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
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
    private long _loggedBytes;

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
        Debug.Print($"SfxpDataChunkInboundDataLogger: {_loggedBytes}B");
        DataExportService.Stop();
    }

    /// <summary>
    /// Check if the message is to log. A message can be logged by zero or one logger maximum.
    /// </summary>
    /// <param name="message">Data message to check for logging</param>
    /// <returns>A list with array items to log or empty list</returns>
    public List<Memory<byte>> CheckIfMessageIsToLog(IInboundDataMessage message)
    {
        var list = new List<Memory<byte>>(1);

        if (message is not SfxpInboundDataMessage { DataBlock: SfxpInboundDatablock db })
        {
            Debug.Print("No SFXP message");
            return list;
        }

        var chunks = db.DataChunks.Where(x => x.Channel == Channel && x.Data.HasValue).Select(x => x.Data!.Value).ToList();

        if (chunks.Count == 0)
        {
            return list;
        }

        var result = new byte[chunks.Count * 8];

        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if (_loggedBytes >= long.MaxValue - result.Length)
        {
            _loggedBytes = 0;
        }

        _loggedBytes += result.Length;

        Span<byte> resultSpan = result;

        foreach (var chunk in chunks)
        {
            chunk.Span.CopyTo(resultSpan);
            resultSpan = resultSpan[chunk.Length..]; // Move span forward
        }

        list.Add(result.AsMemory());
        return list;
    }

    /// <summary>
    /// Log messages
    /// </summary>
    /// <param name="messages">Messages to log</param>
    public void LogTheMessages(List<Memory<byte>> messages)
    {
        _dataExportService.AddRange(messages);
    }
}