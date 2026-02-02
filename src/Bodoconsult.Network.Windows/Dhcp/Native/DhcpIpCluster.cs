// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System.Runtime.InteropServices;

namespace Bodoconsult.Network.Windows.Dhcp.Native;

/// <summary>
/// The DHCP_IP_CLUSTER structure defines the address and mast for a network cluster.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly struct DhcpIpCluster
{
    /// <summary>
    /// DHCP_IP_ADDRESS value that contains the IP address of the cluster.
    /// </summary>
    public readonly DhcpIpAddress ClusterAddress;
    /// <summary>
    /// Specifies the mask value for a cluster. This value should be set to 0xFFFFFFFF if the cluster is full.
    /// </summary>
    public readonly uint ClusterMask;
}
