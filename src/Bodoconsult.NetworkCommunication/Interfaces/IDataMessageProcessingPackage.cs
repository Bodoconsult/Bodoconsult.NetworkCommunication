

// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.
// Licence MIT

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for processing of byte based messages i.e. via TCP/IP. This is the top level class for technology and firmware specific protocol implementations
/// </summary>
public interface IDataMessageProcessingPackage
{

    /// <summary>
    /// Current data messaging config
    /// </summary>
    public IDataMessagingConfig DataMessagingConfig { get; }

    /// <summary>
    /// Current data message splitter
    /// </summary>
    IDataMessageSplitter DataMessageSplitter { get; }

    /// <summary>
    /// Current data message coding processor
    /// </summary>
    IDataMessageCodingProcessor DataMessageCodingProcessor { get; }

    /// <summary>
    /// Current data message processor for internal forwarding of the received messages
    /// </summary>
    IDataMessageProcessor DataMessageProcessor { get; }

    /// <summary>
    /// Current wait state manager
    /// </summary>
    IWaitStateManager WaitStateManager { get; }

    /// <summary>
    /// Current validator impl for handshake messages
    /// </summary>
    IHandshakeDataMessageValidator HandshakeDataMessageValidator  { get; }

    /// <summary>
    /// Current validator impl for data messages
    /// </summary>
    IDataMessageValidator DataMessageValidator { get; }

    /// <summary>
    /// Factory for creation of handshakes to be sent for received messages
    /// </summary>
    IDataMessageHandshakeFactory DataMessageHandshakeFactory { get; }

}