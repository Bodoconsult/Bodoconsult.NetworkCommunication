// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.Protocols;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Base interface for UDP socket implementations
/// </summary>
public interface IUdpSocketProxy : ISocketProxy
{
    /// <summary>
    /// Delegate fired if the socket was receiving data
    /// </summary>
    SocketReceivedDataDelegate? SocketReceivedDataDelegate { get; }

    /// <summary>
    /// Datagrampipeline
    /// </summary>
    IDatagramPipeline Pipeline { get; }

    /// <summary>
    /// Start the receiver loop
    /// </summary>
    /// <param name="socketReceivedDataDelegate">Delegate for forwarding received messages</param>
    void StartReceiverLoop(SocketReceivedDataDelegate socketReceivedDataDelegate);

    /// <summary>
    /// Run the receiver loop
    /// </summary>
    /// <param name="waitForLoopStarted"></param>
    /// <returns></returns>
    public Task ReceiverLoop(AutoResetEvent waitForLoopStarted);
}