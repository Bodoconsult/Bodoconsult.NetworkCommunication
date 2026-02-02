// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System.Runtime.InteropServices;

namespace Bodoconsult.Network.Windows.Dhcp.Native;

/// <summary>
/// The DHCP_SUBNET_INFO structure defines information describing a subnet.
/// </summary>
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal readonly struct DhcpSubnetInfoManaged
{
    /// <summary>
    /// DHCP_IP_ADDRESS value that specifies the subnet ID.
    /// </summary>
    public readonly DhcpIpAddress SubnetAddress;
    /// <summary>
    /// DHCP_IP_MASK value that specifies the subnet IP mask.
    /// </summary>
    public readonly DhcpIpMask SubnetMask;
    /// <summary>
    /// Unicode string that specifies the network name of the subnet.
    /// </summary>
    public readonly string SubnetName;
    /// <summary>
    /// Unicode string that contains an optional comment particular to this subnet.
    /// </summary>
    public readonly string SubnetComment;
    /// <summary>
    /// DHCP_HOST_INFO structure that contains information about the DHCP server servicing this subnet.
    /// </summary>
    public readonly DhcpHostInfoManaged PrimaryHost;
    /// <summary>
    /// DHCP_SUBNET_STATE enumeration value indicating the current state of the subnet (enabled/disabled).
    /// </summary>
    public readonly DhcpSubnetState SubnetState;

    public DhcpSubnetInfoManaged(DhcpIpAddress subnetAddress, DhcpIpMask subnetMask, string subnetName, string subnetComment, DhcpHostInfoManaged primaryHost, DhcpSubnetState subnetState)
    {
        SubnetAddress = subnetAddress;
        SubnetMask = subnetMask;
        SubnetName = subnetName;
        SubnetComment = subnetComment;
        PrimaryHost = primaryHost;
        SubnetState = subnetState;
    }
}
