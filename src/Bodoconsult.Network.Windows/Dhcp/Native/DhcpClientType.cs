// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


namespace Bodoconsult.Network.Windows.Dhcp.Native;

/// <summary>
/// Possible types of the DHCPv4 client.
/// </summary>
[Flags]
internal enum DhcpClientType : byte
{
    /// <summary>
    /// A DHCPv4 client other than ones defined in this table.
    /// </summary>
    ClientTypeUnspecified = 0x00,
    /// <summary>
    /// The DHCPv4 client supports the DHCP protocol.
    /// </summary>
    ClientTypeDhcp = 0x01,
    /// <summary>
    /// The DHCPv4 client supports the BOOTP protocol.
    /// </summary>
    ClientTypeBootp = 0x02,
    /// <summary>
    /// The DHCPv4 client understands both the DHCPv4 and the BOOTP protocols.
    /// </summary>
    ClientTypeBoth = 0x03,
    /// <summary>
    /// There is an IPv4 reservation created for the DHCPv4 client.
    /// </summary>
    ClientTypeReservationFlag = 0x04,
    /// <summary>
    /// Backward compatibility for manual addressing.
    /// </summary>
    ClientTypeNone = 0x64,
}
