// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.
// Licence MIT

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for handshake data messages
/// </summary>
public interface IHandShakeDataMessage: IDataMessage
{
    /// <summary>
    /// Typpe of handshake as byte value
    /// </summary>
    public byte HandshakeMessageType { set; get; }
}