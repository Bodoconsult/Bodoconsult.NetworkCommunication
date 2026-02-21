// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Tests.TestData;

public class DummyHandshakeValidator : IHandshakeDataMessageValidator
{
    /// <summary>
    /// Is a received message a handshake for a sent message
    /// </summary>
    /// <param name="sentMessage">Sent message</param>
    /// <param name="handshakeMessage">Received handshake message</param>
    /// <returns>True if the message was the handshake for the sent message</returns>
    public DataMessageValidatorResult IsHandshakeForSentMessage(IOutboundDataMessage sentMessage,
        IInboundDataMessage handshakeMessage)
    {
        return new DataMessageValidatorResult(true, null);
    }


    /// <summary>
    /// Handle the received handshake and sets the ProcessExecutionResult for the responsible send process <see cref="ISendPacketProcess"/>
    /// </summary>
    /// <param name="context">Current context</param>
    /// <param name="handshake">Received handshake</param>
    public void HandleHandshake(ISendPacketProcess context, IInboundDataMessage handshake)
    {

        if (handshake is not IInboundHandShakeDataMessage hs)
        {
            context.ProcessExecutionResult = OrderExecutionResultState.Unsuccessful;
            return;
        }

        context.ProcessExecutionResult = OrderExecutionResultState.Successful;
        context.CurrentSendAttempsCount = 0;
        context.DataMessagingConfig.MonitorLogger?.LogDebug($"Message {context.Message.MessageId}: handshake received [{hs.HandshakeMessageType:X2}]");
    }
}