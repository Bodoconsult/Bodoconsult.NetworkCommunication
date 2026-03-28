// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.HandshakeDataMessageValidators;

/// <summary>
/// Implementation of <see cref="IHandshakeDataMessageValidator"/> for BTCP protocol
/// </summary>
public class BtcpHandshakeDataMessageValidator : IHandshakeDataMessageValidator
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

        if (sentMessage is not BtcpRequestOutboundDataMessage)
        {
            return new DataMessageValidatorResult(false, "No BTCP data message sent");
        }

        if (handshakeMessage is not InboundHandshakeMessage)
        {
            return new DataMessageValidatorResult(false, "Received message is NOT a valid handshake message");
        }

        return new DataMessageValidatorResult(true, string.Empty);
    }


    /// <summary>
    /// Handle the received handshake and sets the ProcessExecutionResult for the responsible send process <see cref="ISendPacketProcess"/>
    /// </summary>
    /// <param name="context">Current send message process</param>
    /// <param name="handshake">Received handshake</param>
    public void HandleHandshake(ISendPacketProcess context, IInboundHandShakeMessage? handshake)
    {
        var logger = context.DataMessagingConfig?.MonitorLogger;

        if (handshake == null)
        {
            context.ProcessExecutionResult = OrderExecutionResultState.Error;
            return;
        }

        if (handshake is not InboundHandshakeMessage hs)
        {
            //todo result wrong message?
            context.ProcessExecutionResult = OrderExecutionResultState.NoResponseFromDevice;
            logger?.LogWarning($"Message {context.Message?.MessageId}: No handshake received. Current Sent Attempt Count > MaxRepeatCount. No ResponseFromdevice! ");
            return;
        }

        switch (hs.HandshakeMessageType)
        {
            case HandShakeMessageType.Ack:
                context.ProcessExecutionResult = OrderExecutionResultState.Successful;
                context.CurrentSendAttempsCount = 0;
                logger?.LogDebug($"Message {context.Message?.MessageId}: ACK received");
                break;

            case HandShakeMessageType.Nack:
                context.ProcessExecutionResult = OrderExecutionResultState.Nack;
                logger?.LogWarning($"Message {context.Message?.MessageId}: NAK received");
                break;

            case HandShakeMessageType.Can:
                context.ProcessExecutionResult = OrderExecutionResultState.Can;
                //IMPORTANT clear
                context.CurrentSendAttempsCount = 0;
                logger?.LogWarning($"Message {context.Message?.MessageId}: CAN received");
                break;
            default:
                context.ProcessExecutionResult = OrderExecutionResultState.Error;
                break;
        }
    }
}