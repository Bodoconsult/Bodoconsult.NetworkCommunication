// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataBlocks;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataBlockCodecs;

/// <summary>
/// Basic datablock codec for SFXP protocol
/// </summary>
public class SfxpDataBlockCodec : IDataBlockCodec
{
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
        var data = db.Data;

        for (var i = 0; i < data.Length; i++)
        {
            var firstByte = data.Slice(i, 1).Span[0];

            // 0x0 sync byte
            if (firstByte == SfxpProtocolHelper.RegularSyncByte.SyncByte)
            {
                continue;
            }

            // 0x9 sync byte
            if (firstByte == SfxpProtocolHelper.SampleCounterSyncByteBlock.SyncByte)
            {
                i += SfxpProtocolHelper.SampleCounterSyncByteBlock.SyncByte - 1;
                continue;
            }

            if (i + SfxpProtocolHelper.DataChunkLength >= data.Length)
            {
                return;
            }

            var chunk = data.Slice(i, SfxpProtocolHelper.DataChunkLength);

            var dataChunk = new DataChunk
            {
                Data = chunk,
                // ToDo: correct channel
                Channel = 1
            };

            db.DataChunks.Add(dataChunk);
            i += SfxpProtocolHelper.DataChunkLength - 1;
        }
    }
}