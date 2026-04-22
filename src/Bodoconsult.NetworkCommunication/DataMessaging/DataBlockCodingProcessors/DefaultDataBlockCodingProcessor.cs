// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataBlockCodingProcessors;

/// <summary>
/// Default implementation for handling of the usage of the correct datablock codec for processing a datablock 
/// </summary>
public class DefaultDataBlockCodingProcessor : IDataBlockCodingProcessor
{
    /// <summary>
    /// Datablock codes registered
    /// </summary>
    protected readonly Dictionary<char, IDataBlockCodec> DatablockCodecs = new();

    /// <summary>
    /// Load a <see cref="IDataBlockCodec"/> with the key char to identify the datablock. Key char is always the first char in a datablock byte array
    /// </summary>
    /// <param name="key">Key char to identify the datablock. Key char is always the first char in a datablock byte array</param>
    /// <param name="dataBlockCodec">Data block codec to load for the key char</param>
    public void LoadDataBlockCodecs(char key, IDataBlockCodec dataBlockCodec)
    {
        DatablockCodecs.Add(key, dataBlockCodec);
    }

    /// <summary>
    /// Create a byte array to send to device from datablock
    /// </summary>
    /// <param name="data">device message data to send</param>
    /// <param name="dataBlock">Current datablock instance</param>
    public void FromDataBlockToBytes(List<byte> data, ITypedOutboundDataBlock dataBlock)
    {
        //throw new NotImplementedException();

        var result = new DataBlockCodecResult();

        var dataBlockCodec = GetDatablockCodecCanBeNull(dataBlock.DataBlockType);
        if (dataBlockCodec == null)
        {
            result.ErrorMessage = $"No codec available for {dataBlock.DataBlockType}";
            return;
        }
        dataBlockCodec.EncodeDataBlock(data, dataBlock);
    }


    /// <summary>
    /// Get the correct codec for a given datablock
    /// </summary>
    /// <param name="datablockType">Type of the datablock</param>
    /// <returns>Codec or null if no fitting codec was found</returns>
    public IDataBlockCodec? GetDatablockCodecCanBeNull(char datablockType)
    {
        DatablockCodecs.TryGetValue(datablockType, out var dataBlockCodec);
        return dataBlockCodec;
    }

    /// <summary>
    /// Decode a given datablock as byte array to the related internal datablock object
    /// </summary>
    /// <param name="dataBlockBytes">Received datablock as byte array</param>
    /// <returns>Datablock object or null if no fitting codec was found</returns>
    public ITypedInboundDataBlock? FromBytesToDataBlock(Memory<byte> dataBlockBytes)
    {
        if (dataBlockBytes.Length == 0)
        {
            return null;
        }

        var dataBlockType = Convert.ToChar(dataBlockBytes[..1].Span[0]);
        var dataBlockCodec = GetDatablockCodecCanBeNull(dataBlockType);
        if (dataBlockCodec == null)
        {
            return null;
        }

        var dataBlock = dataBlockCodec.DecodeDataBlock(dataBlockBytes);
        return dataBlock;
    }
}