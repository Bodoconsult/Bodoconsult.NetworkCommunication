// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System.Runtime.InteropServices;

namespace Bodoconsult.Network.Windows.Dhcp.Native;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal readonly struct DhcpSubnetInfoVqManaged
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
    private readonly string SubnetName;
    /// <summary>
    /// Unicode string that contains an optional comment particular to this subnet.
    /// </summary>
    private readonly string SubnetComment;
    /// <summary>
    /// DHCP_HOST_INFO structure that contains information about the DHCP server servicing this subnet.
    /// </summary>
    public readonly DhcpHostInfoManaged PrimaryHost;
    /// <summary>
    /// DHCP_SUBNET_STATE enumeration value indicating the current state of the subnet (enabled/disabled).
    /// </summary>
    public readonly DhcpSubnetState SubnetState;
    /// <summary>
    /// Integer value used as a BOOL to represent whether or not Quarantine is enabled for the subnet.
    /// If TRUE (0x00000001), Quarantine is turned ON on the DHCP server; if FALSE (0x00000000), it is turned OFF.
    /// </summary>
    public readonly uint QuarantineOn;
    /// <summary>
    /// Reserved for future use.
    /// </summary>
    private readonly uint Reserved1;
    /// <summary>
    /// Reserved for future use.
    /// </summary>
    private readonly uint Reserved2;
    /// <summary>
    /// Reserved for future use.
    /// </summary>
    private readonly ulong Reserved3;
    /// <summary>
    /// Reserved for future use.
    /// </summary>
    private readonly ulong Reserved4;

    public DhcpSubnetInfoVqManaged(DhcpIpAddress subnetAddress, DhcpIpMask subnetMask, string subnetName, string subnetComment, DhcpHostInfoManaged primaryHost, DhcpSubnetState subnetState, uint quarantineOn, uint reserved1, uint reserved2, ulong reserved3, ulong reserved4)
    {
        SubnetAddress = subnetAddress;
        SubnetMask = subnetMask;
        SubnetName = subnetName;
        SubnetComment = subnetComment;
        PrimaryHost = primaryHost;
        SubnetState = subnetState;
        QuarantineOn = quarantineOn;
        Reserved1 = reserved1;
        Reserved2 = reserved2;
        Reserved3 = reserved3;
        Reserved4 = reserved4;
    }
}
