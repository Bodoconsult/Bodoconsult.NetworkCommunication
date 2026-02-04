// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.NetworkCommunication.EnumAndStates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;

/// <summary>
/// Represents an EDCP protocol handshake message
/// </summary>
public sealed class EdcpHandshakeMessage : BaseHandShakeDataMessage, IHandShakeDataMessage
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="messageType">Current message type</param>
    public EdcpHandshakeMessage(MessageTypeEnum messageType)
    {
        MessageType = messageType;
    }

    /// <summary>
    /// Type of handshake as byte value
    /// </summary>
    public byte HandshakeMessageType { set; get; }

    /// <summary>
    /// Block code of the data message this handshake is acknowledging
    /// </summary>
    public byte BlockCode { get; set; }

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        return $"{HandshakeMessageType switch
        {
            6 => "EdcpHandshake ACK",
            21 => "EdcpHandshake NAK",
            24 => "EdcpHandshake CAN",
            _ => "EdcpHandshake Unknown"
        }} BlockCode {BlockCode}";
    }

    /// <summary>
    /// Create an info string for logging
    /// </summary>
    /// <returns>Info string</returns>
    public override string ToInfoString()
    {
        return $"{HandshakeMessageType switch
        {
            6 => "EdcpHandshake ACK",
            21 => "EdcpHandshake NAK",
            24 => "EdcpHandshake CAN",
            _ => "EdcpHandshake Unknown"
        }} BlockCode {BlockCode}";
    }

    /// <summary>
    /// Create a short info string for logging
    /// </summary>
    /// <returns>Info string</returns>
    public override string ToShortInfoString()
    {
        return $"{HandshakeMessageType switch
        {
            6 => "EdcpHandshake ACK",
            21 => "EdcpHandshake NAK",
            24 => "EdcpHandshake CAN",
            _ => "EdcpHandshake Unknown"
        }} BlockCode {BlockCode}";
    }
}