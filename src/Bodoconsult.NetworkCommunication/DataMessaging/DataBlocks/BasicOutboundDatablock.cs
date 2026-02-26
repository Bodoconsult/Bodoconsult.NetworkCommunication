// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataBlocks;

/// <summary>
/// Basic outbound data block
/// </summary>
public class BasicOutboundDatablock : ITypedOutboundDataBlock
{
    /// <summary>
    /// Data contains the bytes of the DataBlock except the byte representing datablock type
    /// </summary>
    public Memory<byte> Data { get; set; }

    /// <summary>
    /// Type code for the type of the outbound datablock
    /// </summary>
    public char DataBlockType { get; set; } = 'x';
}