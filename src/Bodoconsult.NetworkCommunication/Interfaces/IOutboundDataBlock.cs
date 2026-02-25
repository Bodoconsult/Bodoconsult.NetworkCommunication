// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for an outbound datablock in a <see cref="IInboundDataMessage"/>
/// </summary>
public interface IOutboundDataBlock
{
    /// <summary>
    /// Data contains the bytes of the Data except the byte representing datablock type
    /// </summary>
    Memory<byte> Data { set; get; }
}