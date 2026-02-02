// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.BufferPool;
using Bodoconsult.NetworkCommunication.Communication.Sending;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Factories;

/// <summary>
/// Create an instance of <see cref="SendPacketProcess"/> to implement <see cref="ISendPacketProcess"/>
/// </summary>
public class SendPacketProcessFactory : ISendPacketProcessFactory
{
    /// <summary>
    /// Current buffer pool instance
    /// </summary>
    private readonly BufferPool<ISendPacketProcess> _bufferPool;

    public SendPacketProcessFactory()
    {
        _bufferPool = new BufferPool<ISendPacketProcess>(() =>
        {
            try
            {
                return new SendPacketProcess();
            }
            catch
            {
                // Do nothing
                return null;
            }
        });

        _bufferPool.Allocate(20);
    }

    /// <summary>
    /// Create an instance of <see cref="ISendPacketProcess "/> to send a message to the tower
    /// </summary>
    /// <param name="duplexIo">Current Duplex-IO instance</param>
    /// <param name="message">Current message to send</param>
    /// <param name="smdtower">Current tower instance</param>
    /// <returns>A send packet process instance to hande the message to send</returns>
    public ISendPacketProcess CreateInstance(IDuplexIo duplexIo, IDataMessage message, IDataMessagingConfig smdtower)
    {
        var result = _bufferPool.Dequeue();
        result.LoadDependencies(duplexIo, message, smdtower);
        result.RegisterWaitState();
        return result;
    }

    /// <summary>
    /// Reset the <see cref="SendPacketProcess"/> instance and give it back to pool
    /// </summary>
    /// <param name="sendPacketProcess">Instance to return</param>
    public void EnqueueInstance(ISendPacketProcess sendPacketProcess)
    {
        if (sendPacketProcess == null)
        {
            return;
        }

        try
        {
            sendPacketProcess.Reset();
            _bufferPool.Enqueue(sendPacketProcess);
        }
        catch //(Exception e)
        {
            // Do nothing
        }
    }
}