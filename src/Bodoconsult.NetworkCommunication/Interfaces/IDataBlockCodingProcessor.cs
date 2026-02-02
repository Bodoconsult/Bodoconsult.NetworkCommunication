// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

public interface IDataBlockCodingProcessor
{

    /// <summary>
    /// Load a <see cref="IDataBlockCodec"/> with the key char to identify the datablock. Key char is always the first char in a datablock byte array
    /// </summary>
    /// <param name="key">Key char to identify the datablock. Key char is always the first char in a datablock byte array</param>
    /// <param name="dataBlockCodec">Data block codec to load for the key char</param>
    void LoadDataBlockCodecs(char key, IDataBlockCodec dataBlockCodec);


    /// <summary>
    /// Create a byte array to send to device from datablock
    /// </summary>
    /// <param name="data">device message data to send</param>
    /// <param name="dataBlock">Current datablock instance</param>
    void FromDataBlockToBytes(List<byte> data, IDataBlock dataBlock);

    /// <summary>
    /// Get the correct codec for a given datablock
    /// </summary>
    /// <param name="datablockType">Type of the datablock</param>
    /// <returns>Codec or null if no fitting codec was found</returns>
    IDataBlockCodec GetDatablockCodecCanBeNull(char datablockType);

    /// <summary>
    /// Decode a given datablock as byte array to the related internal datablock object
    /// </summary>
    /// <param name="dataBlockBytes">Received datablock as byte array</param>
    /// <returns>Datablock object or null if no fitting codec was found</returns>
    IDataBlock FromBytesToDataBlock(Memory<byte> dataBlockBytes);
}