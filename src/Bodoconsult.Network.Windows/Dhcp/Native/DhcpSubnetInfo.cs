using System.Runtime.InteropServices;

namespace Bodoconsult.Network.Windows.Dhcp.Native;

/// <summary>
/// The DHCP_SUBNET_INFO structure defines information describing a subnet.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal readonly struct DhcpSubnetInfo : IDisposable
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
    public readonly DhcpSubnetState SubnetState;

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
