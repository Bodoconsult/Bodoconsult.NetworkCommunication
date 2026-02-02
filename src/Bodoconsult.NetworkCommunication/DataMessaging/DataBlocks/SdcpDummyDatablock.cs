// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataBlocks;

/// <summary>
/// Dummy data block
/// </summary>
public class SdcpDummyDatablock: IDataBlock
{
    /// <summary>
    /// Data contains the bytes of the Data except the byte representing datablock type
    /// </summary>
    public Memory<byte> Data { get; set; }

    /// <summary>
    /// Code for the datablock type
    /// </summary>
    public char DataBlockType { get; set; }= 'x';
}