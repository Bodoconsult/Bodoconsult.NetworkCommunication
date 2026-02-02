// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using System.Runtime.InteropServices;

namespace Bodoconsult.Network.Windows.Dhcp.Native;

/// <summary>
/// The DHCP_OPTION_SCOPE_INFO structure defines information about the options provided for a certain DHCP scope.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly struct DhcpOptionScopeInfoManagedReserved
{
    private readonly IntPtr scopeType;

    /// <summary>
    /// <see cref="DhcpOptionScopeType"/> enumeration value that defines the scope type of the associated DHCP options, and indicates which of the following fields in the union will be populated.
    /// </summary>
    public DhcpOptionScopeType ScopeType => (DhcpOptionScopeType)scopeType;

    /// <summary>
    /// DHCP_IP_ADDRESS value that contains an IP address used to identify the reservation.
    /// </summary>
    public readonly DhcpIpAddress ReservedIpAddress;

    /// <summary>
    /// DHCP_IP_ADDRESS value that specifies the subnet ID of the subnet containing the reservation.
    /// </summary>
    public readonly DhcpIpAddress ReservedIpSubnetAddress;

    public DhcpOptionScopeInfoManagedReserved(DhcpIpAddress reservedIpSubnetAddress, DhcpIpAddress reservedIpAddress)
    {
        scopeType = (IntPtr)DhcpOptionScopeType.DhcpReservedOptions;
        ReservedIpAddress = reservedIpAddress;
        ReservedIpSubnetAddress = reservedIpSubnetAddress;
    }
}
