// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.
// Licence MIT

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for an inbound datablock in a <see cref="IInboundDataMessage"/>
/// </summary>
public interface IInboundDataBlock
{
    /// <summary>
    /// Data contains the bytes of the DataBlock except the byte representing datablock type
    /// </summary>
    Memory<byte> Data { set; get; }
}