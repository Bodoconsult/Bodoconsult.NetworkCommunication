// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataBlocks;

/// <summary>
/// Dummy inbound data block
/// </summary>
public class DummyInboundDatablock: ITypedInboundDataBlock
{
    /// <summary>
    /// Data contains the bytes of the Data except the byte representing datablock type
    /// </summary>
    public Memory<byte> Data { get; set; }

    /// <summary>
    /// Type code for the type of the inbound datablock
    /// </summary>
    public char DataBlockType { get; set; }= 'x';
}