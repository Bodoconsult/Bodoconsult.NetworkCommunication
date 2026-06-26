// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataBlocks;

/// <summary>
/// Sfxp inbound data block
/// </summary>
public class SfxpInboundDatablock : ITypedInboundDataBlock
{
    /// <summary>
    /// Data contains the bytes of the DataBlock except the byte representing datablock type
    /// </summary>
    public Memory<byte> Data { get; set; }

    /// <summary>
    /// Type code for the type of the inbound datablock
    /// </summary>
    public char DataBlockType { get; set; } = 's';

    /// <summary>
    /// List of data chunks
    /// </summary>
    public List<DataChunk> DataChunks { get; } = new(2048);
}