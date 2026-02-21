// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for outbound handshake data messages
/// </summary>
public interface IOutboundHandShakeDataMessage : IOutboundDataMessage
{
    /// <summary>
    /// Typpe of handshake as byte value
    /// </summary>
    public byte HandshakeMessageType { set; get; }
}