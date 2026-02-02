// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;

/// <summary>
/// Represents a handshake message
/// </summary>
public sealed class HandshakeMessage : BaseHandShakeDataMessage, IHandShakeDataMessage
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="messageType">Current message type</param>
    public HandshakeMessage(MessageTypeEnum messageType)
    {
        MessageType = messageType;
    }

    /// <summary>
    /// Typpe of handshake as byte value
    /// </summary>
    public byte HandshakeMessageType { set; get; }

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        return HandshakeMessageType switch
        {
            6 => "Handshake ACK",
            21 => "Handshake NAK",
            24 => "Handshake CAN",
            _ => "Handshake Unknown"
        };
    }

    /// <summary>
    /// Create an info string for logging
    /// </summary>
    /// <returns>Info string</returns>
    public override string ToInfoString()
    {
        return HandshakeMessageType switch
        {
            6 => "Handshake ACK",
            21 => "Handshake NAK",
            24 => "Handshake CAN",
            _ => "Handshake Unknown"
        };
    }

    /// <summary>
    /// Create a short info string for logging
    /// </summary>
    /// <returns>Info string</returns>
    public override string ToShortInfoString()
    {
        return HandshakeMessageType switch
        {
            6 => "Handshake ACK",
            21 => "Handshake NAK",
            24 => "Handshake CAN",
            _ => "Handshake Unknown"
        };
    }
}