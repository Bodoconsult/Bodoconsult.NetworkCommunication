// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;

/// <summary>
/// Represents an EDCP protocol handshake message
/// </summary>
public sealed class EdcpInboundHandshakeMessage : BaseInboundHandShakeMessage
{
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
            6 => "EdcpInboundHandshake ACK",
            21 => "EdcpInboundHandshake NAK",
            24 => "EdcpInboundHandshake CAN",
            _ => "EdcpInboundHandshake Unknown"
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
            6 => "EdcpInboundHandshake ACK",
            21 => "EdcpInboundHandshake NAK",
            24 => "EdcpInboundHandshake CAN",
            _ => "EdcpInboundHandshake Unknown"
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
            6 => "EdcpInboundHandshake ACK",
            21 => "EdcpInboundHandshake NAK",
            24 => "EdcpInboundHandshake CAN",
            _ => "EdcpInboundHandshake Unknown"
        }} BlockCode {BlockCode}";
    }
}