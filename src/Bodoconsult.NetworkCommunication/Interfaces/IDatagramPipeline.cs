// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.Delegates;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for stream based <see cref="IPipeline"/> implementations
/// </summary>
public interface IDatagramPipeline : IPipeline
{
    /// <summary>
    /// Inbound queue for received UDP result
    /// </summary>
    public ProducerConsumerQueue<byte[]> InboundQueue { get; }

    /// <summary>
    /// Delegate fired if the socket was receiving data
    /// </summary>
    SocketReceivedDataDelegate? SocketReceivedDataDelegate { get; }

    /// <summary>
    /// Start the receiver loop
    /// </summary>
    /// <param name="socketReceivedDataDelegate">Delegate for forwarding received messages</param>
    void StartReceiverLoop(SocketReceivedDataDelegate socketReceivedDataDelegate);

    /// <summary>
    /// Stop the receiver loop
    /// </summary>
    void StopReceiverLoop();

    ///// <summary>
    ///// Run the receiver loop
    ///// </summary>
    ///// <param name="waitForLoopStarted"><see cref="AutoResetEvent"/> to wait until the loop has been started</param>
    ///// <returns></returns>
    //Task ReceiverLoop(AutoResetEvent waitForLoopStarted);
    
    
    /// <summary>
    /// Add the received data to the queue
    /// </summary>
    void AddMemory(Memory<byte> data);

    /// <summary>
    /// Add a received UDP result to the inbound queue
    /// </summary>
    /// <param name="result">Received UDP result</param>
    void AddResult(byte[] result);
}