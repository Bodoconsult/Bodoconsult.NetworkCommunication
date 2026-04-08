// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.HandshakeDataMessageValidators;

/// <summary>
/// Implementation of <see cref="IHandshakeDataMessageValidator"/> for SFXP protocol
/// </summary>
public class SfxpHandshakeDataMessageValidator : IHandshakeDataMessageValidator
{
    /// <summary>
    /// Is a received message a handshake for a sent message
    /// </summary>
    /// <param name="sentMessage">Sent message</param>
    /// <param name="handshakeMessage">Received handshake message</param>
    /// <returns>True if the message was the handshake for the sent message</returns>
    public DataMessageValidatorResult IsHandshakeForSentMessage(IOutboundDataMessage sentMessage,
        IInboundHandShakeMessage? handshakeMessage)
    {
        return new DataMessageValidatorResult(true, string.Empty);
    }

    /// <summary>
    /// Handle the received handshake and sets the ProcessExecutionResult for the responsible send process <see cref="ISendPacketProcess"/>
    /// </summary>
    /// <param name="context">Current send message process</param>
    /// <param name="handshake">Received handshake</param>
    public void HandleHandshake(ISendPacketProcess context, IInboundHandShakeMessage? handshake)
    {
        throw new NotSupportedException("SFXP does not use handshakes");
    }
}