// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.
// Licence MIT

using Bodoconsult.NetworkCommunication.Communication.Sending;
using Bodoconsult.NetworkCommunication.Delegates;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface to handle data message acknowledgments
/// </summary>
public interface IWaitStateManager: IDisposable
{

    /// <summary>
    /// Current data messaging config
    /// </summary>
    IDataMessagingConfig DataMessagingConfig { get; }

    /// <summary>
    /// The timestamp of the last message received
    /// </summary>
    long LastMessageTimeStamp { get; set; }

    /// <summary>
    /// Method used to bind to a delegate for receiving handshakes
    /// </summary>
    /// <param name="msg">Data message</param>
    void OnHandshakeReceived(IHandShakeDataMessage msg);

    /// <summary>
    /// Register a wait for ACK state to be handled
    /// </summary>
    /// <param name="state">State to be registered for ACK handling</param>
    void RegisterWaitState(SendPacketProcess state);

    /// <summary>
    /// Delegate called if <see cref="OnHandshakeReceived"/> is starting to process a received handshake message
    /// </summary>
    RaiseDataMessageHandshakeReceivedDelegate RaiseHandshakeReceivedDelegate { get; set; }

    /// <summary>
    /// Current number of wait states registered
    /// </summary>
    int Count { get;  }
}