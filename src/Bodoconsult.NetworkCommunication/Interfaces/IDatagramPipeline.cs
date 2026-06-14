// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Net.Sockets;
using Bodoconsult.NetworkCommunication.Delegates;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for stream based <see cref="IPipeline"/> implementations
/// </summary>
public interface IDatagramPipeline : IPipeline
{
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
    /// <param name="udpResult">Received result</param>
    void AddMemory(UdpReceiveResult udpResult);
}