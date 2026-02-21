// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for internal forwarding of the received messages of byte based messaging i.e. via TCP/IP
/// Implementations should invoke deviceCommSettings.RaiseDataMessageReceivedDelegate for data messages and deviceCommSettings.DataMessageProcessingPackage.WaitStateManager?.OnHandshakeReceived for handshakes
/// </summary>
public interface IDataMessageProcessor
{
    /// <summary>
    /// Process the message
    /// </summary>
    /// <param name="message">Message to process</param>
    public void ProcessMessage(IInboundDataMessage message);
}