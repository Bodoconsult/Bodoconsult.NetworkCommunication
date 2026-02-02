// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System.Runtime.InteropServices;

namespace Bodoconsult.Network.Windows.Dhcp.Native;

/// <summary>
/// The DHCP_IP_RANGE structure defines a range of IP addresses.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly struct DhcpIpRange
{
    /// <summary>
    /// DHCP_IP_ADDRESS value that contains the first IP address in the range.
    /// </summary>
    public readonly DhcpIpAddress StartAddress;
    /// <summary>
    /// DHCP_IP_ADDRESS value that contains the last IP address in the range.
    /// </summary>
    public readonly DhcpIpAddress EndAddress;

    public DhcpIpRange(DhcpIpAddress startAddress, DhcpIpAddress endAddress)
    {
        StartAddress = startAddress;
        EndAddress = endAddress;
    }
}
