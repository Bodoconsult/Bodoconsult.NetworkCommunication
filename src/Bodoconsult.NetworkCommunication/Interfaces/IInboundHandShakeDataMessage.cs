// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.
// Licence MIT

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for inbound handshake data messages
/// </summary>
public interface IInboundHandShakeDataMessage: IInboundDataMessage
{
    /// <summary>
    /// Typpe of handshake as byte value
    /// </summary>
    public byte HandshakeMessageType { set; get; }
}