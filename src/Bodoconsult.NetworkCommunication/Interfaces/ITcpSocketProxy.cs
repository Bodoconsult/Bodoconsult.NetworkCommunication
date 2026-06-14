// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for UDP implementations of <see cref="ISocketProxy"/>
/// </summary>
public interface ITcpSocketProxy : ISocketProxy
{
    /// <summary>
    /// Delegate fired if the socket was receiving data
    /// </summary>
    SocketReceivedDataDelegate2? SocketReceivedDataDelegate { get; }

    /// <summary>
    /// Start the receiver loop
    /// </summary>
    /// <param name="socketReceivedDataDelegate">Delegate for forwarding received messages</param>
    void StartReceiverLoop(SocketReceivedDataDelegate2 socketReceivedDataDelegate);
}