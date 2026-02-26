// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for outbound handshake messages
/// </summary>
public interface IOutboundHandShakeMessage : IOutboundMessage
{
    /// <summary>
    /// Type of handshake as byte value
    /// </summary>
    public byte HandshakeMessageType { set; get; }
}