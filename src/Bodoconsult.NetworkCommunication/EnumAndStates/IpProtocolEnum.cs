// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.
// Licence MIT

namespace Bodoconsult.NetworkCommunication.EnumAndStates;

/// <summary>
/// Enum for IP based protocols supported by this library
/// </summary>
public enum IpProtocolEnum
{
    /// <summary>
    /// Bidirectional and connection based TCP/IP protocol
    /// </summary>
    Tcp,

    /// <summary>
    /// Connectionless UDP protocol
    /// </summary>
    Udp
}