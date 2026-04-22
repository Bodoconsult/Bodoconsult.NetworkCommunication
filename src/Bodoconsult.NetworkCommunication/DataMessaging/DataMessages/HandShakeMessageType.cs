// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;

/// <summary>
/// This class holds all the handshake message types
/// </summary>
public static class HandShakeMessageType
{
    /// <summary>
    /// ACK (6, 0x6)
    /// </summary>
    public  const byte Ack = 6;
    /// <summary>
    /// CAN (24, 0x18)
    /// </summary>
    public const byte Can = 24;
    /// <summary>
    /// NACK (21, 0x15)
    /// </summary>
    public const byte Nack = 21;
}