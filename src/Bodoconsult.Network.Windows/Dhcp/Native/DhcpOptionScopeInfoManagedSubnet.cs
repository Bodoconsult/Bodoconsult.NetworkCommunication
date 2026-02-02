// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using System.Runtime.InteropServices;

namespace Bodoconsult.Network.Windows.Dhcp.Native;

/// <summary>
/// The DHCP_OPTION_SCOPE_INFO structure defines information about the options provided for a certain DHCP scope.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly struct DhcpOptionScopeInfoManagedSubnet
{
    private readonly IntPtr scopeType;

    /// <summary>
    /// <see cref="DhcpOptionScopeType"/> enumeration value that defines the scope type of the associated DHCP options, and indicates which of the following fields in the union will be populated.
    /// </summary>
    public DhcpOptionScopeType ScopeType => (DhcpOptionScopeType)scopeType;

    /// <summary>
    /// DHCP_IP_ADDRESS value that contains the subnet ID as a DWORD.
    /// </summary>
    public readonly DhcpIpAddress SubnetScopeInfo;

    public DhcpOptionScopeInfoManagedSubnet(DhcpIpAddress subnetScopeInfo)
    {
        scopeType = (IntPtr)DhcpOptionScopeType.DhcpSubnetOptions;
        SubnetScopeInfo = subnetScopeInfo;
    }
}
