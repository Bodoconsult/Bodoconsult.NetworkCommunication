// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Factories;

/// <summary>
/// Types of fake send packet process results
/// </summary>
public enum FakeSendPacketProcessEnum
{
    /// <summary>
    /// Successful
    /// </summary>
    Successful,
    /// <summary>
    /// Encoding error
    /// </summary>
    EncodingError,
    /// <summary>
    /// Socket error
    /// </summary>
    SocketError
}