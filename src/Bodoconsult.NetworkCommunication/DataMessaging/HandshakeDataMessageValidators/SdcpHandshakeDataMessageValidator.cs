// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.HandshakeDataMessageValidators;

/// <summary>
/// Implementation of <see cref="IHandshakeDataMessageValidator"/> for SDCP protocol
/// </summary>
public class SdcpHandshakeDataMessageValidator : IHandshakeDataMessageValidator

{
    /// <summary>
    /// Is a received message a handshake for a sent message
    /// </summary>
    /// <param name="sentMessage">Sent message</param>
    /// <param name="handshakeMessage">Received handshake message</param>
    /// <returns>True if the message was the handshake for the sent message</returns>
    public DataMessageValidatorResult IsHandshakeForSentMessage(IDataMessage sentMessage, IDataMessage handshakeMessage)
    {

        if (sentMessage is not SdcpDataMessage)
        {
            return new DataMessageValidatorResult(false, "No SDCP data message sent");
        }

        if (handshakeMessage is not HandshakeMessage)
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
    public void HandleHandshake(ISendPacketProcess context, IDataMessage handshake)
    {
        if (handshake == null)
        {
            context.ProcessExecutionResult = OrderExecutionResultState.Error;
            return;
        }

        if (handshake is not HandshakeMessage hs)
        {
            //todo result wrong message?
            context.ProcessExecutionResult = OrderExecutionResultState.NoResponseFromDevice;
            context.DataMessagingConfig.MonitorLogger?.LogWarning($"Message {context.Message.MessageId}: No handshake received. Current Sent Attempt Count > MaxRepeatCount. No ResponseFromTower! ");
            return;
        }

        switch (hs.HandshakeMessageType)
        {
            case HandShakeMessageType.Ack:
                context.ProcessExecutionResult = OrderExecutionResultState.Successful;
                context.CurrentSendAttempsCount = 0;
                context.DataMessagingConfig.MonitorLogger?.LogDebug($"Message {context.Message.MessageId}: ACK received");
                break;

            case HandShakeMessageType.Nack:
                context.ProcessExecutionResult = OrderExecutionResultState.Nack;
                context.DataMessagingConfig.MonitorLogger?.LogWarning($"Message {context.Message.MessageId}: NAK received");
                break;

            case HandShakeMessageType.Can:
                context.ProcessExecutionResult = OrderExecutionResultState.Can;
                //IMPORTANT clear
                context.CurrentSendAttempsCount = 0;
                context.DataMessagingConfig.MonitorLogger?.LogWarning($"Message {context.Message.MessageId}: CAN received");
                break;
            default:
                context.ProcessExecutionResult = OrderExecutionResultState.Error;
                break;
        }
    }
}