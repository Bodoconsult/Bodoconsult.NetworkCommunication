// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.BufferPool;
using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlocks;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataBlockCodecs;

/// <summary>
/// Basic datablock codec for SFXP protocol. 
/// </summary>
public class SfxpDataBlockCodec : IDataBlockCodec
{
    private readonly BufferPool<DataChunk> _bufferPool = new();
    private readonly Lock _streamingConfigLock = new();
    private byte[] _streamingConfig = [];
    private int _streamingConfigLength;

    private const ulong CompareValueSampleCounter = 0b_00000000_00001001_00000000_00001001_00000000_00001001_00000000_00001001;
    private const ulong ResultValueSampleCounter = 0b_00000000_00001001_00000000_00001001_00000000_00001001_0000000_000001001;

    /// <summary>
    /// Default ctor
    /// </summary>
    public SfxpDataBlockCodec()
    {
        _bufferPool.LoadFactoryMethod(() => new DataChunk
        {
            ReturnDataChunkDelegate = ReturnDataChunkDelegate
        });
        _bufferPool.Allocate(2100);
    }

    /// <summary>
    /// Byte mask for the mapping of the chunk to channel relationship. Mask may have a maximum length of 254 bytes
    /// </summary>
    public byte[] StreamingConfig
    {
        get
        {
            lock (_streamingConfigLock)
            {
                return _streamingConfig;
            }
        }
        private set
        {
            lock (_streamingConfigLock)
            {
                _streamingConfig = value;
                _streamingConfigLength = value.Length - 1;
            }
        }
    }

    /// <summary>
    /// Load the byte mask for the mapping of the chunk to channel relationship. Mask may have a maximum length of 254 bytes
    /// </summary>
    /// <param name="config">Mask to load. Mask may have a maximum length of 254 bytes</param>
    public void LoadStreamingConfig(byte[] config)
    {
        var result = new List<byte>();

        foreach (var b in config)
        {
            if (b == 0xF || result.Count == 255)
            {
                break;
            }

            result.Add(b);
        }

        StreamingConfig = result.ToArray();
    }

    /// <summary>
    /// Method encodes an instance of Datablock in bytes array.
    /// Method is called when a message is sent to the device
    /// </summary>
    /// <param name="data">The array as list to add the datablock to</param>
    /// <param name="datablock">Current datablock object</param>
    /// <returns>a byte array with datablock infos</returns>
    public void EncodeDataBlock(List<byte> data, ITypedOutboundDataBlock datablock)
    {
        if (datablock is not BasicOutboundDatablock db)
        {
            throw new ArgumentException("Wrong type of datablock");
        }

        // You should add some datablock validation here

        // Add data block type
        data.Add(Convert.ToByte(datablock.DataBlockType));

        // Now add the data or place any logic here to create byte array from your specific datablock
        foreach (var b in db.Data.Span)
        {
            data.Add(b);
        }
    }

    /// <summary>
    /// Method decodes an incoming bytes array to an instance of Datablock object
    /// Method is used while receiving bytes from device
    /// </summary>
    /// <param name="datablockBytes">Datablock bytes received</param>
    /// <returns>Datablock object</returns>
    public ITypedInboundDataBlock DecodeDataBlock(Memory<byte> datablockBytes)
    {
        // Now create your datablock as request by specs here
        if (datablockBytes.Length < 2)
        {
            return new SfxpInboundDatablock
            {
                Data = Array.Empty<byte>(),
                DataBlockType = 's'
            };
        }

        var db = new SfxpInboundDatablock
        {
            Data = datablockBytes[1..],
            DataBlockType = 's'
        };

        ParseDataChunks(db);

        return db;
    }

    private void ParseDataChunks(SfxpInboundDatablock db)
    {
        if (_streamingConfigLength == 0)
        {
            return;
        }

        // Get the data chunks now
        GetTheChunksNow(db);

        // Find potential sync chunks
        var syncChunks = FindPotentialSyncChunks(db.DataChunks);

        // Check the potential sync chunks now if they are really sync chunks
        CheckPotentialSyncChunks(syncChunks, db.DataChunks);

        // Apply channels to chunks
        ApplyChannelsToChunks(db.DataChunks);

        // Remove the sync chunks now
        RemoveSyncChunks(db.DataChunks, syncChunks);
    }


    private void CheckPotentialSyncChunks(List<int> syncChunks, List<DataChunk> allChunks)
    {
        var sl = syncChunks.ToList();

        var len = _streamingConfigLength + 1;

        foreach (var chunk in sl)
        {
            var path = CheckPotentialSyncChunkItem(syncChunks.Where(x => x > chunk), chunk, len);

            if (path.Count == 0)
            {
                continue;
            }

            foreach (var syncChunk in sl)
            {
                if (path.Contains(syncChunk))
                {
                    continue;
                }

                allChunks[syncChunk].DataChunkType = DataChunkType.DataChunk;
            }

            return;
        }

    }

    private static List<int> CheckPotentialSyncChunkItem(IEnumerable<int> syncChunks, int chunk, int streamingConfigLength)
    {
        var path = new List<int>();

        foreach (var c in syncChunks)
        {
            var result = c % streamingConfigLength;
            //Debug.Print($"{c} {result}");
            if (result == 0)
            {
                path.Add(c);
            }
        }

        if (path.Count < 7)
        {
            path.Clear();
        }
        else
        {
            path.Add(chunk);
        }
        
        return path;
    }

    private void ApplyChannelsToChunks(List<DataChunk> chunks)
    {
        var currentIndex = 0;

        foreach (var chunk in chunks)
        {
            // If sync chunk reset current index
            if (chunk.DataChunkType != DataChunkType.DataChunk)
            {
                currentIndex = 0;
                continue;
            }

            // if data chunk
            chunk.Channel = StreamingConfig[currentIndex];

            if (currentIndex == _streamingConfigLength)
            {
                currentIndex = 0;
            }
            currentIndex++;
        }

        // Now check if the data chunks have type 255 0xFF
        var indexMask = _streamingConfigLength;
        for (var index = chunks.Count - 1; index >= 0; index--)
        {
            var chunk = chunks[index];

            if (chunk.Channel != 0xFF || chunk.DataChunkType == DataChunkType.DataChunk)
            {
                continue;
            }

            chunk.Channel = StreamingConfig[indexMask];

            //Debug.Print($"{index}: {chunk.Channel}");

            indexMask--;
            if (indexMask < 0)
            {
                indexMask = _streamingConfigLength;
            }
        }
    }

    private void GetTheChunksNow(SfxpInboundDatablock db)
    {
        var data = db.Data;

        Debug.Print($"{ArrayHelper.GetStringFromArray(db.Data)}");

        var len = data.Length;

        for (var i = 0; i < len; i += SfxpProtocolHelper.DataChunkLength)
        {
            var chunk = data.Slice(i, SfxpProtocolHelper.DataChunkLength);

            //Debug.Print($"{i}: "+ArrayHelper.GetStringFromArray(chunk));

            var dataChunk = _bufferPool.Dequeue();
            dataChunk.Data = chunk;

            db.DataChunks.Add(dataChunk);
        }
    }

    private static void RemoveSyncChunks(List<DataChunk> chunks, List<int> syncChunks)
    {
        for (var i = 0; i < syncChunks.Count; i++)
        {
            var chunk = chunks[i];

            chunks.Remove(chunk);
        }
    }

    private static List<int> FindPotentialSyncChunks(List<DataChunk> chunks)
    {
        var syncChunks = new List<int>();

        for (var i = 0; i < chunks.Count; i++)
        {
            var chunk = chunks[i];

            if (chunk.Data == null)
            {
                continue;
            }

            var firstByte = chunk.Data.Value.Slice(0, 1).Span[0];

            // 0x0 sync byte found
            ulong block;
            if (firstByte == SfxpProtocolHelper.RegularSyncByte.SyncByte)
            {

                // Check for 00x0 sync byte chunk
                block = BitConverter.ToUInt64(chunk.Data.Value.ToArray());

                if (block == 0)
                {
                    syncChunks.Add(i);
                    //Debug.Print($"SyncChunk 0x0 {i}");
                    chunk.DataChunkType = DataChunkType.RegularSyncChunk;
                    continue;
                }
            }

            // No 0x9 sync byte
            if (firstByte != SfxpProtocolHelper.SampleCounterSyncByteBlock.SyncByte)
            {

                continue;
            }

            // 0x9 sync byte found: check for 0x9 sync chunk
            block = BitConverter.ToUInt64(chunk.Data.Value.ToArray());

            // No 0x9 sync chunk
            if ((block & CompareValueSampleCounter) != ResultValueSampleCounter)
            {
                continue;
            }

            // 0x9 sync chunk
            syncChunks.Add(i);
            //Debug.Print($"SynChunk 0x9 {i}");
            chunk.DataChunkType = DataChunkType.SampleCounterSyncChunk;
        }

        return syncChunks;
    }

    private void ReturnDataChunkDelegate(DataChunk chunk)
    {
        chunk.Reset();
        _bufferPool.Enqueue(chunk);
    }
}