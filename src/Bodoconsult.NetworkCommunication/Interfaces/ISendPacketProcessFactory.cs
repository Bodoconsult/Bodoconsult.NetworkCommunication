// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Factory interface for <see cref="ISendPacketProcess"/> implementations
/// </summary>
public interface ISendPacketProcessFactory
{
    /// <summary>
    /// Create a instance of <see cref="ISendPacketProcess "/> to send a message to the tower
    /// </summary>
    /// <param name="duplexIo">Current Duplex-IO instance</param>
    /// <param name="message">Current message to send</param>
    /// <param name="smdtower">Current tower instance</param>
    /// <returns>A send packet process instance to hande the message to send</returns>
    ISendPacketProcess CreateInstance(IDuplexIo duplexIo,
        IDataMessage message,
        IDataMessagingConfig smdtower);

    /// <summary>
    /// Reset the <see cref="ISendPacketProcess"/> instance and give it back to pool
    /// </summary>
    /// <param name="sendPacketProcess">Instance to return</param>
    void EnqueueInstance(ISendPacketProcess sendPacketProcess);

}