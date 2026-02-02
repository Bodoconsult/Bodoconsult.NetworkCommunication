// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;

/// <summary>
/// This class holds all the handshake message types
/// </summary>
public static class HandShakeMessageType
{
    public  const byte Ack = 6;
    public const byte Can = 24;
    public const byte Nack = 21;
}