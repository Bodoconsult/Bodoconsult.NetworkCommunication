// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.BufferPool;
using Bodoconsult.NetworkCommunication.DataMessaging.DataBlocks;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;
using System.Diagnostics;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataBlockCodecs;

/// <summary>
/// Basic datablock codec for SFXP protocol. 
/// </summary>
public class SfxpDataBlockCodec : IDataBlockCodec
{
    private readonly BufferPool<DataChunk> _bufferPool = new();

    /// <summary>
    /// Default ctor
    /// </summary>
    public SfxpDataBlockCodec()
    {
        _bufferPool.LoadFactoryMethod(() => new DataChunk()
        {
            ReturnDataChunkDelegate = ReturnDataChunkDelegate
        });
        _bufferPool.Allocate(200);
    }

    /// <summary>
    /// Byte mask for the mapping of the chunk to channel relationship. Mask may have a maximum length of 254 bytes
    /// </summary>
    public byte[] Mask { get; private set; } = [];

    /// <summary>
    /// Load the byte mask for the mapping of the chunk to channel relationship. Mask may have a maximum length of 254 bytes
    /// </summary>
    /// <param name="mask">Mask to load. Mask may have a maximum length of 254 bytes</param>
    public void LoadMask(byte[] mask)
    {
        var result = new List<byte>();

        foreach (var b in mask)
        {
            if (b == 0xF || result.Count == 254)
            {
                break;
            }

            result.Add(b);
        }

        Mask = result.ToArray();
    }

    /// <summary>
    /// Method encode an instance of Datablock in bytes array.
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
        if (Mask.Length == 0)
        {
            return;
        }

        var currentIndex = (byte)0xff;

        var data = db.Data;

        for (var i = 0; i < data.Length; i++)
        {
            var firstByte = data.Slice(i, 1).Span[0];

            // 0x0 sync byte
            if (firstByte == SfxpProtocolHelper.RegularSyncByte.SyncByte)
            {
                currentIndex = 0;
                continue;
            }

            // 0x9 sync byte
            if (firstByte == SfxpProtocolHelper.SampleCounterSyncByteBlock.SyncByte)
            {
                i += SfxpProtocolHelper.SampleCounterSyncByteBlock.SyncByte - 1;
                currentIndex = 0;
                continue;
            }

            if (i + SfxpProtocolHelper.DataChunkLength >= data.Length)
            {
                break;
            }

            var chunk = data.Slice(i, SfxpProtocolHelper.DataChunkLength);

            var dataChunk = _bufferPool.Dequeue();
            dataChunk.Data = chunk;
            dataChunk.Channel = currentIndex == 0xFF ? (byte)0xFF : Mask[currentIndex];

            if (currentIndex != 255)
            {
                currentIndex++;
                if (currentIndex == Mask.Length)
                {
                    currentIndex = 0;
                }
            }

            db.DataChunks.Add(dataChunk);
            i += SfxpProtocolHelper.DataChunkLength - 1;
        }

        // Now check if the chunks have type 255 0xFF
        var indexMask = Mask.Length -1;
        for (var index = db.DataChunks.Count - 1; index >= 0; index--)
        {
            var chunk = db.DataChunks[index];

            if (chunk.Channel != 0xFF)
            {
                continue;
            }

            chunk.Channel = Mask[indexMask];

            Debug.Print($"{index}: {chunk.Channel}");

            indexMask--;
            if (indexMask < 0)
            {
                indexMask = Mask.Length - 1;
            }
        }
    }

    private void ReturnDataChunkDelegate(DataChunk chunk)
    {
        chunk.Reset();
        _bufferPool.Enqueue(chunk);
    }
}