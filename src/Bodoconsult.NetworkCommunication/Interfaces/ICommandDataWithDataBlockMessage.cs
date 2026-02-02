// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for data messages used for commands delivering a <see cref="IDataBlock"/>
/// </summary>
public interface ICommandDataWithDataBlockMessage : ICommandDataMessage
{

    /// <summary>
    /// Length of the datablock  or 0 if no data block is contained
    /// </summary>
    public int DataBlockLength { get; set; }

    /// <summary>
    /// The datablock instance bound to this message or null
    /// </summary>
    public IDataBlock DataBlock { get; set; }
}