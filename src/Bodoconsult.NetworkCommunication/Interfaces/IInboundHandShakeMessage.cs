// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.
// Licence MIT

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for inbound handshake data messages
/// </summary>
public interface IInboundHandShakeMessage: IInboundMessage
{
    /// <summary>
    /// Type of handshake as byte value
    /// </summary>
    public byte HandshakeMessageType { set; get; }
}