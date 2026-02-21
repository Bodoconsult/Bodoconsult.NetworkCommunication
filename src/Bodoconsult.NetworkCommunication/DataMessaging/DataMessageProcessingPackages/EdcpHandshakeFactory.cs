// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessageProcessingPackages;

/// <summary>
/// Factory for creating handshakes for EDCP protocol to sent to the client
/// </summary>
public class EdcpHandshakeFactory : IDataMessageHandshakeFactory
{
    /// <summary>
    /// Get an ACK handshake message
    /// </summary>
    /// <param name="message">Current message received</param>
    /// <returns>ACK handshake message to send</returns>
    public IInboundDataMessage GetAckResponse(IInboundDataMessage message)
    {
        if (message is not EdcpInboundDataMessage em)
        {
            var can = new EdcpInboundHandshakeMessage(MessageTypeEnum.Sent)
            {
                HandshakeMessageType = HandShakeMessageType.Can,
            };
            return can;
        }

        var ack = new EdcpInboundHandshakeMessage(MessageTypeEnum.Sent)
        {
            HandshakeMessageType = HandShakeMessageType.Ack,
            BlockCode = em.BlockCode
        };

        return ack;
    }

    /// <summary>
    /// Get a NAK handshake message
    /// </summary>
    /// <param name="message">Current message received</param>
    /// <returns>NAK handshake message to send</returns>
    public IInboundDataMessage GetNakResponse(IInboundDataMessage message)
    {
        if (message is not EdcpInboundDataMessage em)
        {
            var can = new EdcpInboundHandshakeMessage(MessageTypeEnum.Sent)
            {
                HandshakeMessageType = HandShakeMessageType.Can,
            };
            return can;
        }

        var nak = new EdcpInboundHandshakeMessage(MessageTypeEnum.Sent)
        {
            HandshakeMessageType = HandShakeMessageType.Nack,
            BlockCode = em.BlockCode
        };
        return nak;
    }

    /// <summary>
    /// Get a CAN handshake message
    /// </summary>
    /// <param name="message">Current message received</param>
    /// <returns>CAN handshake message to send</returns>
    public IInboundDataMessage GetCanResponse(IInboundDataMessage message)
    {
        if (message is not EdcpInboundDataMessage em)
        {
            var can1 = new EdcpInboundHandshakeMessage(MessageTypeEnum.Sent)
            {
                HandshakeMessageType = HandShakeMessageType.Can,
            };
            return can1;
        }

        var can = new EdcpInboundHandshakeMessage(MessageTypeEnum.Sent)
        {
            HandshakeMessageType = HandShakeMessageType.Can,
            BlockCode = em.BlockCode
        };
        return can;
    }
}