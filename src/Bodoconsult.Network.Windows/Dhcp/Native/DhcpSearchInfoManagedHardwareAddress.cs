// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using System.Runtime.InteropServices;

namespace Bodoconsult.Network.Windows.Dhcp.Native;

/// <summary>
/// The DHCP_SEARCH_INFO structure defines the DHCP client record data used to search against for particular server operations.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly struct DhcpSearchInfoManagedHardwareAddress
{
    private readonly IntPtr SearchTypePtr;

    /// <summary>
    /// DHCP_SEARCH_INFO_TYPE enumeration value that specifies the data included in the subsequent member of this structure.
    /// </summary>
    public DhcpSearchInfoType SearchType => (DhcpSearchInfoType)SearchTypePtr;

    /// <summary>
    /// DHCP_CLIENT_UID structure that contains a hardware MAC address. This field is populated if SearchType is set to DhcpClientHardwareAddress.
    /// </summary>
    public readonly DhcpClientUid ClientHardwareAddress;

    public DhcpSearchInfoManagedHardwareAddress(DhcpClientUid clientHardwareAddress)
    {
        SearchTypePtr = (IntPtr)DhcpSearchInfoType.DhcpClientHardwareAddress;
        ClientHardwareAddress = clientHardwareAddress;
    }
}
