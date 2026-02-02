// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System.Runtime.InteropServices;

namespace Bodoconsult.Network.Windows.Dhcp.Native;

/// <summary>
/// The DHCP_BOOTP_IP_RANGE structure defines a suite of IPs for lease to BOOTP-specific clients.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly struct DhcpBootpIpRange
{
    /// <summary>
    /// DHCP_IP_ADDRESS value that specifies the start of the IP range used for BOOTP service.
    /// </summary>
    public readonly DhcpIpAddress StartAddress;
    /// <summary>
    /// DHCP_IP_ADDRESS value that specifies the end of the IP range used for BOOTP service.
    /// </summary>
    public readonly DhcpIpAddress EndAddress;
    /// <summary>
    /// Specifies the number of BOOTP clients with addresses served from this range.
    /// </summary>
    public readonly int BootpAllocated;
    /// <summary>
    /// Specifies the maximum number of BOOTP clients this range is allowed to serve.
    /// </summary>
    public readonly int MaxBootpAllowed;

    public DhcpBootpIpRange(DhcpIpAddress startAddress, DhcpIpAddress endAddress, int bootpAllocated, int maxBootpAllowed)
    {
        StartAddress = startAddress;
        EndAddress = endAddress;
        BootpAllocated = bootpAllocated;
        MaxBootpAllowed = maxBootpAllowed;
    }
}
