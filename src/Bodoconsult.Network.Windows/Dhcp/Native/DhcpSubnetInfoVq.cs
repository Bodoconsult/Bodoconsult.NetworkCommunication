// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using System.Runtime.InteropServices;

namespace Bodoconsult.Network.Windows.Dhcp.Native;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct DhcpSubnetInfoVq : IDisposable
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
    private readonly IntPtr SubnetNamePointer;
    /// <summary>
    /// Unicode string that contains an optional comment particular to this subnet.
    /// </summary>
    private readonly IntPtr SubnetCommentPointer;
    /// <summary>
    /// DHCP_HOST_INFO structure that contains information about the DHCP server servicing this subnet.
    /// </summary>
    public readonly DhcpHostInfo PrimaryHost;
    /// <summary>
    /// DHCP_SUBNET_STATE enumeration value indicating the current state of the subnet (enabled/disabled).
    /// </summary>
    public readonly uint SubnetState;
    /// <summary>
    /// Integer value used as a BOOL to represent whether or not Quarantine is enabled for the subnet.
    /// If TRUE (0x00000001), Quarantine is turned ON on the DHCP server; if FALSE (0x00000000), it is turned OFF.
    /// </summary>
    public readonly uint QuarantineOn;
    /// <summary>
    /// Reserved for future use.
    /// </summary>
    public readonly uint Reserved1;
    /// <summary>
    /// Reserved for future use.
    /// </summary>
    public readonly uint Reserved2;
    /// <summary>
    /// Reserved for future use.
    /// </summary>
    public readonly ulong Reserved3;
    /// <summary>
    /// Reserved for future use.
    /// </summary>
    public readonly ulong Reserved4;

    /// <summary>
    /// Unicode string that specifies the network name of the subnet.
    /// </summary>
    public string SubnetName => Marshal.PtrToStringUni(SubnetNamePointer);
    /// <summary>
    /// Unicode string that contains an optional comment particular to this subnet.
    /// </summary>
    public string SubnetComment => Marshal.PtrToStringUni(SubnetCommentPointer);

    public void Dispose()
    {
        Api.FreePointer(SubnetNamePointer);

        // Freeing SubnetComment causes heap corruption ?!?!?
        // Api.FreePointer(ref SubnetCommentPointer);

        PrimaryHost.Dispose();
    }
}
