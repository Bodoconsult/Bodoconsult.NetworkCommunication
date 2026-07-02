// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using Bodoconsult.App.BufferPool;
using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlocks;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;

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

    /// <summary>
    /// Default ctor
    /// </summary>
    public SfxpDataBlockCodec()
    {
        _bufferPool.LoadFactoryMethod(() => new DataChunk
        {
            ReturnDataChunkDelegate = ReturnDataChunkDelegate
        });
        _bufferPool.Allocate(10000);
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
            Data = datablockBytes,
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

        //var chunk = db.DataChunks[655];

        // Check the potential sync chunks now if they are really sync chunks
        CheckPotentialSyncChunks(syncChunks, db.DataChunks);

        // Apply channels to chunks
        ApplyChannelsToChunks(db.DataChunks, syncChunks);

        //ListChunks(db.DataChunks);

        // Remove the sync chunks now
        RemoveSyncChunks(db.DataChunks);

        // Remove not identified chunks now
        RemoveNotIdentifiedChunks(db.DataChunks);
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
                //Debug.Print("All sync chunks");
                continue;
            }

            foreach (var syncChunk in sl)
            {
                if (path.Contains(syncChunk))
                {
                    //Debug.Print($"Sync {syncChunk}");
                    continue;
                }

                //Debug.Print($"No sync {syncChunk}");
                allChunks[syncChunk].DataChunkType = DataChunkType.DataChunk;
            }

            return;
        }
    }

    private static List<int> CheckPotentialSyncChunkItem(IEnumerable<int> syncChunks, int chunk, int streamingConfigLength)
    {
        var path = new List<int>();
        var offSet = streamingConfigLength - chunk;

        foreach (var c in syncChunks)
        {
            var result = (c + offSet) % streamingConfigLength;
            //Debug.Print($"{c + offSet} {result:0}");
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

    private void ApplyChannelsToChunks(List<DataChunk> chunks, List<int> syncChunks)
    {
        var currentIndex = 0;

        if (syncChunks.Count == 0)
        {
            return;
        }

        var firstSyncChunk = syncChunks.First();

        // Skip all chunks before the first sync byte
        for (var index = firstSyncChunk + 1; index < chunks.Count; index++)
        {
            var chunk = chunks[index];

            //Debug.Print($"Chunk {index}: {chunk.DataChunkType}: {StreamingConfig[currentIndex]}");

            // if data chunk
            chunk.Channel = StreamingConfig[currentIndex];

            if (currentIndex == _streamingConfigLength)
            {
                currentIndex = 0;
            }
            else
            {
                currentIndex++;
            }
        }

        // Now check if the data chunks have type 255 0xFF
        var indexMask = _streamingConfigLength;
        for (var index = firstSyncChunk; index >= 0; index--)
        {
            var chunk = chunks[index];

            chunk.Channel = StreamingConfig[indexMask];

            //Debug.Print($"OxFF: {index}: {chunk.DataChunkType}:{chunk.Channel}");

            if (indexMask == 0)
            {
                indexMask = _streamingConfigLength;
            }
            else
            {
                indexMask--;
            }
        }
    }

    private void GetTheChunksNow(SfxpInboundDatablock db)
    {
        var data = db.Data;

        //Debug.Print($"{ArrayHelper.GetStringFromArray(db.Data)}");

        var len = data.Length;

        for (var i = 0; i < len; i += SfxpProtocolHelper.DataChunkLength)
        {
            var chunk = data.Slice(i, SfxpProtocolHelper.DataChunkLength); //.ToArray();

            //Debug.Print($"{i}: " + ArrayHelper.GetStringFromArray(chunk));

            var dataChunk = _bufferPool.Dequeue();
            dataChunk.Data = chunk;

            db.DataChunks.Add(dataChunk);
        }
    }

    private static void RemoveSyncChunks(List<DataChunk> chunks)
    {
        foreach (var chunk in chunks.Where(x => x.DataChunkType == DataChunkType.RegularSyncChunk).ToList())
        {
            chunks.Remove(chunk);
        }
    }

    private static void RemoveNotIdentifiedChunks(List<DataChunk> chunks)
    {
        foreach (var chunk in chunks.Where(x => x.Channel == DataChunk.Identifier).ToList())
        {
            chunks.Remove(chunk);
        }
    }

    private static void ListChunks(List<DataChunk> chunks)
    {
        for (var i = 0; i < chunks.Count; i++)
        {
            var chunk = chunks[i];
            Debug.Print($"{i}: {chunk.DataChunkType} CH{chunk.Channel}" + ArrayHelper.GetStringFromArrayCsharpStyle(chunk.Data!.Value, false));
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

            var value = chunk.Data.Value;
            var lastByte = value.Span[7];

            // 0x0 sync byte found
            if (lastByte == SfxpProtocolHelper.RegularSyncByte.SyncByte)
            {
                // Check for 00x0 sync byte chunk
                var block = CheckForRegularSyncChunk(value);

                if (block)
                {
                    syncChunks.Add(i);
                    //Debug.Print($"SyncChunk 0x0 {i} {ArrayHelper.GetStringFromArrayCsharpStyle(value, false)}");
                    chunk.DataChunkType = DataChunkType.RegularSyncChunk;
                    continue;
                }
            }

            // No 0x9 sync byte
            if (lastByte != SfxpProtocolHelper.SampleCounterSyncByteBlock.SyncByte)
            {
                continue;
            }

            // 0x9 sync byte found: check for 0x9 sync chunk
            if (!CheckForSampleCounterChunk(value))
            {
                continue;
            }

            // 0x9 sync chunk
            syncChunks.Add(i);
            //Debug.Print($"SyncChunk 0x9 {i} {ArrayHelper.GetStringFromArrayCsharpStyle(value, false)}");
            chunk.DataChunkType = DataChunkType.SampleCounterSyncChunk;
        }

        return syncChunks;
    }

    /// <summary>
    /// Check if a chunk is a sample counter chunk
    /// </summary>
    /// <param name="chunk">Chunk to check</param>
    /// <returns>True if a chunk is a sample counter chunk</returns>
    public static bool CheckForSampleCounterChunk(Memory<byte> chunk)
    {
        //Debug.Print(ArrayHelper.GetStringFromArrayCsharpStyle(chunk, false));

        // No 0x9 sync chunk
        if (chunk.Span[7] == 0x9 &&
            chunk.Span[5] == 0x9 &&
            chunk.Span[3] == 0x9 &&
            chunk.Span[1] == 0x9)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Check if a chunk is a sample counter chunk
    /// </summary>
    /// <param name="chunk">Chunk to check</param>
    /// <returns>True if a chunk is a sample counter chunk</returns>
    public static bool CheckForRegularSyncChunk(Memory<byte> chunk)
    {
        //Debug.Print(ArrayHelper.GetStringFromArrayCsharpStyle(chunk, false));

        // No 0x9 sync chunk
        if (chunk.Span[7] == 0x0 &&
            chunk.Span[6] == 0x0 &&
            chunk.Span[5] == 0x0 &&
            chunk.Span[4] == 0x0 &&
            chunk.Span[3] == 0x0 &&
            chunk.Span[2] == 0x0 &&
            chunk.Span[1] == 0x0 &&
            chunk.Span[0] == 0x0
            )
        {
            return true;
        }

        return false;
    }

    private void ReturnDataChunkDelegate(DataChunk chunk)
    {
        chunk.Reset();
        _bufferPool.Enqueue(chunk);
    }
}