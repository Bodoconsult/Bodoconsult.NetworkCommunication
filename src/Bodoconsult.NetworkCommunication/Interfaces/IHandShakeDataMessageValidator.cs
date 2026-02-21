// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.
// Licence MIT

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for validating customer specific handshake message. Used in 
/// </summary>
public interface IHandshakeDataMessageValidator
{
    /// <summary>
    /// Is a received message a handshake for a sent message
    /// </summary>
    /// <param name="sentMessage">Sent message</param>
    /// <param name="handshakeMessage">Received handshake message</param>
    /// <returns>True if the message was the handshake for the sent message</returns>
    DataMessageValidatorResult IsHandshakeForSentMessage(IOutboundDataMessage sentMessage,
        IInboundDataMessage handshakeMessage);

    /// <summary>
    /// Handle the received handshake and sets the ProcessExecutionResult for the responsible send process <see cref="ISendPacketProcess"/>
    /// </summary>
    /// <param name="context">Current send message process</param>
    /// <param name="handshake">Received handshake</param>
    void HandleHandshake(ISendPacketProcess context, IInboundDataMessage handshake);

}