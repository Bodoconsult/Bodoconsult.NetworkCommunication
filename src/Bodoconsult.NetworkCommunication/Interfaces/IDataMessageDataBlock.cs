

// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.
// Licence MIT

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for a datablock in a <see cref="IInboundDataMessage"/>
/// </summary>
public interface IDataMessageDataBlock
{

    /// <summary>
    /// Data contains the bytes of the Data except the byte representing datablock type
    /// </summary>
    Memory<byte> Data { set; get; }

}