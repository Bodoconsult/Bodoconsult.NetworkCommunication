// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for inbound datablocks having a char properties containing an info about the type of datablock
/// </summary>
public interface ITypedInboundDataBlock: IInboundDataBlock
{
    /// <summary>
    /// Type code for the type of the inbound datablock
    /// </summary>
    char DataBlockType { get; }
}