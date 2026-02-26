// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessageProcessingPackages;

/// <summary>
/// Factory for creating handshakes for SDCP protocol to sent to the client
/// </summary>
public class SdcpHandshakeFactory : IDataMessageHandshakeFactory
{
    /// <summary>
    /// Get an ACK handshake message
    /// </summary>
    /// <param name="message">Current message received</param>
    /// <returns>ACK handshake message to send</returns>
    public IOutboundHandShakeMessage GetAckResponse(IInboundDataMessage message)
    {
        var ack = new OutboundHandshakeMessage
        {
            HandshakeMessageType = HandShakeMessageType.Ack
        };

        return ack;
    }

    /// <summary>
    /// Get a NAK handshake message
    /// </summary>
    /// <param name="message">Current message received</param>
    /// <returns>NAK handshake message to send</returns>
    public IOutboundHandShakeMessage GetNakResponse(IInboundDataMessage message)
    {
        var nak = new OutboundHandshakeMessage
        {
            HandshakeMessageType = HandShakeMessageType.Nack
        };
        return nak;
    }

    /// <summary>
    /// Get a CAN handshake message
    /// </summary>
    /// <param name="message">Current message received</param>
    /// <returns>CAN handshake message to send</returns>
    public IOutboundHandShakeMessage GetCanResponse(IInboundDataMessage message)
    {
        var can = new OutboundHandshakeMessage
        {
            HandshakeMessageType = HandShakeMessageType.Can
        };
        return can;
    }
}