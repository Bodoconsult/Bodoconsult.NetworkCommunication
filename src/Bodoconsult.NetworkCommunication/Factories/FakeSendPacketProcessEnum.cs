// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Factories;

/// <summary>
/// Types of fake send packet process results
/// </summary>
public enum FakeSendPacketProcessEnum
{
    Successful,
    EncodingError,
    SocketError
}