// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using System.Runtime.InteropServices;

namespace Bodoconsult.Network.Windows.Dhcp.Native;

/// <summary>
/// The DHCP_SEARCH_INFO structure defines the DHCP client record data used to search against for particular server operations.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly struct DhcpSearchInfoManagedIpAddress
{
    private readonly IntPtr SearchTypePtr;

    /// <summary>
    /// DHCP_SEARCH_INFO_TYPE enumeration value that specifies the data included in the subsequent member of this structure.
    /// </summary>
    public DhcpSearchInfoType SearchType => (DhcpSearchInfoType)SearchTypePtr;

    /// <summary>
    /// DHCP_IP_ADDRESS value that specifies a client IP address. This field is populated if SearchType is set to DhcpClientIpAddress.
    /// </summary>
    public readonly DhcpIpAddress ClientIpAddress;

    public DhcpSearchInfoManagedIpAddress(DhcpIpAddress clientIpAddress)
    {
        SearchTypePtr = (IntPtr)DhcpSearchInfoType.DhcpClientIpAddress;
        ClientIpAddress = clientIpAddress;
    }
}
