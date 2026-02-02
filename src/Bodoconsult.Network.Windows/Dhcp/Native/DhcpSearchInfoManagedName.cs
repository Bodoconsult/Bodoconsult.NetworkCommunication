// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System.Runtime.InteropServices;

namespace Bodoconsult.Network.Windows.Dhcp.Native;

/// <summary>
/// The DHCP_SEARCH_INFO structure defines the DHCP client record data used to search against for particular server operations.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly struct DhcpSearchInfoManagedName
{
    private readonly IntPtr SearchTypePtr;

    /// <summary>
    /// DHCP_SEARCH_INFO_TYPE enumeration value that specifies the data included in the subsequent member of this structure.
    /// </summary>
    public DhcpSearchInfoType SearchType => (DhcpSearchInfoType)SearchTypePtr;

    /// <summary>
    /// Unicode string that specifies the network name of the DHCP client. This field is populated if SearchType is set to DhcpClientName.
    /// </summary>
    public readonly IntPtr ClientName;

    public DhcpSearchInfoManagedName(IntPtr clientName)
    {
        SearchTypePtr = (IntPtr)DhcpSearchInfoType.DhcpClientName;
        ClientName = clientName;
    }
}
