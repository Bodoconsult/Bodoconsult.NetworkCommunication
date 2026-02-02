// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System.Runtime.InteropServices;

namespace Bodoconsult.Network.Windows.Dhcp.Native;

/// <summary>
/// The DHCP_CLIENT_INFO structure defines a client information record used by the DHCP server.
/// </summary>
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal readonly struct DhcpClientInfoManaged : IDisposable
{
    /// <summary>
    /// DHCP_IP_ADDRESS value that contains the assigned IP address of the DHCP client.
    /// </summary>
    public readonly DhcpIpAddress ClientIpAddress;
    /// <summary>
    /// DHCP_IP_MASK value that contains the subnet mask value assigned to the DHCP client.
    /// </summary>
    public readonly DhcpIpMask SubnetMask;
    /// <summary>
    /// DHCP_CLIENT_UID structure containing the MAC address of the client's network interface device.
    /// </summary>
    public readonly DhcpBinaryDataManaged ClientHardwareAddress;
    /// <summary>
    /// Unicode string that specifies the network name of the DHCP client. This member is optional.
    /// </summary>
    public readonly string ClientName;
    /// <summary>
    /// Unicode string that contains a comment associated with the DHCP client. This member is optional.
    /// </summary>
    public readonly string ClientComment;
    /// <summary>
    /// DATE_TIME structure that contains the date and time the DHCP client lease will expire, in UTC time.
    /// </summary>
    public readonly DateTime ClientLeaseExpires;
    /// <summary>
    /// DHCP_HOST_INFO structure that contains information on the DHCP server that assigned the IP address to the client. 
    /// </summary>
    public readonly DhcpHostInfoManaged OwnerHost;

    public DhcpClientInfoManaged(DhcpIpAddress clientIpAddress, DhcpIpMask subnetMask, DhcpBinaryDataManaged clientHardwareAddress, string clientName, string clientComment, DateTime clientLeaseExpires, DhcpHostInfoManaged ownerHost)
    {
        ClientIpAddress = clientIpAddress;
        SubnetMask = subnetMask;
        ClientHardwareAddress = clientHardwareAddress;
        ClientName = clientName;
        ClientComment = clientComment;
        ClientLeaseExpires = clientLeaseExpires;
        OwnerHost = ownerHost;
    }

    public void Dispose()
    {
        ClientHardwareAddress.Dispose();
    }
}
