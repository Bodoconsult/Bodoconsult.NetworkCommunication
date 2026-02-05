// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataBlocks;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataBlockCodecs;

/// <summary>
/// Datablock codec example for SDCP and EDCP protocol
/// </summary>
public class SdcpDummyDataBlockCodec : IDataBlockCodec
{
    /// <summary>
    /// Method encode an instance of Datablock in bytes array.
    /// Method is called when a message is sent to the device
    /// </summary>
    /// <param name="data">The array as list to add the datablock to</param>
    /// <param name="datablock">Current datablock object</param>
    /// <returns>a byte array with datablock infos</returns>
    public void EncodeDataBlock(List<byte> data, IDataBlock datablock)
    {
        if (datablock is not SdcpDummyDatablock db)
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
    public IDataBlock DecodeDataBlock(Memory<byte> datablockBytes)
    {

        // You should add some datablock validation here

        // 

        // Now create your datablock as request by specs here
        if (datablockBytes.Length < 2)
        {
            return new SdcpDummyDatablock
            {
                Data = Array.Empty<byte>(),
                DataBlockType = 'x'
            };
        }
        return new SdcpDummyDatablock
        {
            Data = datablockBytes[1..],
            DataBlockType = 'x'
        };
    }
}