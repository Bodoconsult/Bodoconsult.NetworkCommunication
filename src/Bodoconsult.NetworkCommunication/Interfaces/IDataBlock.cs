// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for datablock
/// </summary>
public interface IDataBlock: IDataMessageDataBlock
{

    /// <summary>
    /// Code for the datablock type
    /// </summary>
    char DataBlockType { get; set; }
}