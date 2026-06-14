// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DigitalTwins;

/// <summary>
/// Factory for a digital twin for SFXP messages
/// </summary>
public class SfxpDigitalTwinMessageFactory : IDigitalTwinMessageFactory
{
    private long _messageId;
    private int _lastChunkId;
    private int _syncByteCounter = -1;
    private int _lastSampleCounter = -1;

    /// <summary>
    /// The interval for sending sample counter instead of regular sync byte
    /// </summary>
    public double SendSampleCounterInterval { get; set; } = 5.0;

    /// <summary>
    /// The number of messages created
    /// </summary>
    public int NumberOfMessagesCreated { get; set; } = 10;

    /// <summary>
    /// Generate a set of messages as defined with <see cref="NumberOfMessagesCreated"/>
    /// </summary>
    /// <returns>List with messages to send</returns>
    public List<Memory<byte>> GenerateMessages()
    {
        var result = new List<Memory<byte>>();

        for (var i = 0; i < NumberOfMessagesCreated; i++)
        {
            //Debug.Print($"\r\n*********** File {i} ***********");
            result.Add(CreateMessage());
        }

        return result;
    }

    /// <summary>
    /// Generate the next message in endless mode
    /// </summary>
    /// <returns>Message to send</returns>
    public Memory<byte> GenerateNextMessage()
    {
        return CreateMessage();
    }

    private Memory<byte> CreateMessage()
    {
        var data = new List<byte>();

        // MessageId (as big endian)
        var intBytes = GetMessageIdAsBytes();

        data.AddRange(intBytes);

        while (true)
        {
            // Add sync byte
            if (_lastChunkId == 0)
            {
                if (AddSyncByte(data))
                {
                    break;
                }
            }

            if (data.Count == SfxpProtocolHelper.MaximumMessageLength - 1)
            {
                break;
            }

            if (CreateChunks(data))
            {
                break;
            }
        }

        SetNextMessageId();

        return data.ToArray().AsMemory();
    }

    private bool CreateChunks(List<byte> data)
    {
        var counter = 1;

        // Create chunks
        for (var j = _lastChunkId; j < SfxpProtocolHelper.NumberOfChunksBeforeSyncByteIsSent; j++)
        {
            // Add chunk if possible
            var chunk = GenerateChunk(j);

            // Resulting message is too long: leave here but remember chunk ID
            if (data.Count + chunk.Count >= SfxpProtocolHelper.MaximumMessageLength)
            {
                //Debug.Print($"{counter} chunks");
                _lastChunkId = j + 1;
                return true;
            }

            data.AddRange(chunk);
            counter++;
        }

        //Debug.Print($"{counter - 1} chunks");
        _lastChunkId = 0;
        return false;
    }

    /// <summary>
    /// Add the sync bytes
    /// </summary>
    /// <param name="data">Message</param>
    /// <returns>True if no sync byte could be added because of the length of the message else false</returns>
    private bool AddSyncByte(List<byte> data)
    {
        SyncByteDefinition sd;
        byte[] syncBytes;

        _syncByteCounter++;

        //var test = _lastSampleCounter % SendSampleCounterInterval;
        if (_syncByteCounter >= SendSampleCounterInterval && (_syncByteCounter) % SendSampleCounterInterval < 0.0001)
        {
            // Send sample counter
            //Debug.Print($"Sample sync byte: {_syncByteCounter} // {_lastSampleCounter}  // {(_syncByteCounter) % SendSampleCounterInterval}");
            sd = SfxpProtocolHelper.SampleCounterSyncByteBlock;
            syncBytes = [sd.SyncByte, 0x1, sd.SyncByte, 0x1, sd.SyncByte, 0x1, sd.SyncByte, 0x1];
            _lastSampleCounter = _syncByteCounter;
        }
        else
        {
            // repeat the sample counter
            if (_syncByteCounter > SendSampleCounterInterval && _lastSampleCounter + 1 == _syncByteCounter)
            {
                //Debug.Print($"Repeat sample sync byte:  {_syncByteCounter} // {_lastSampleCounter}");
                sd = SfxpProtocolHelper.SampleCounterSyncByteBlock;
                syncBytes = [sd.SyncByte, 0x2, sd.SyncByte, 0x2, sd.SyncByte, 0x2, sd.SyncByte, 0x2];
                _lastSampleCounter = _syncByteCounter - 1;
            }
            else
            {
                // Normal sync byte
                //Debug.Print($"Normal sync byte: {_syncByteCounter} // {_lastSampleCounter}");
                sd = SfxpProtocolHelper.RegularSyncByte;
                syncBytes = [sd.SyncByte];
            }
        }

        if (data.Count > SfxpProtocolHelper.MaximumMessageLength - sd.Length)
        {
            _syncByteCounter--;
            return true;
        }

        data.AddRange(syncBytes);
        return false;
    }

    /// <summary>
    /// Set the next message ID. If long.MaxValue is reached restart with 0
    /// </summary>
    private void SetNextMessageId()
    {
        if (_messageId == long.MaxValue)
        {
            _messageId = 0;
        }
        else
        {
            _messageId++;
        }
    }

    /// <summary>
    /// MessageId (as big endian)
    /// </summary>
    /// <returns></returns>
    private byte[] GetMessageIdAsBytes()
    {
        var intBytes = BitHelper.FromInt64ToBigEndian(_messageId);

        //var intBytes = BitConverter.GetBytes();
        //if (_islittleEndian)
        //{
        //    Array.Reverse(intBytes);
        //}
        return intBytes;
    }

    /// <summary>
    /// Generate a data chunk
    /// </summary>
    /// <param name="index">Index of the data chunk</param>
    /// <returns></returns>
    private List<byte> GenerateChunk(int index)
    {
        var result = new List<byte>();

        //Debug.Print($"Chunk {index}");

        index += 32;    // Add 32 to avoid issue with 0x9 sync byte and others
        if (index > 255)
        {
            index = index - 255 + 32;
        }

        for (var i = 0; i < SfxpProtocolHelper.DataChunkLength; i++)
        {
            result.Add((byte)index);
        }

        return result;
    }
}