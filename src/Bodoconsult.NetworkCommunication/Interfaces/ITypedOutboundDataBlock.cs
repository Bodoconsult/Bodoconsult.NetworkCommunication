// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for outbound datablocks having a char properties containing an info about the type of datablock
/// </summary>
public interface ITypedOutboundDataBlock : IOutboundDataBlock
{
    /// <summary>
    /// Type code for the type of the outbound datablock
    /// </summary>
    char DataBlockType { get; }
}