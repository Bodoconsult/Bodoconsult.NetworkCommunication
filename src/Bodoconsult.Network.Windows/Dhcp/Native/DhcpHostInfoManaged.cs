// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System.Runtime.InteropServices;

namespace Bodoconsult.Network.Windows.Dhcp.Native;

/// <summary>
/// The DHCP_HOST_INFO structure defines information on a DHCP server (host).
/// </summary>
/// <remarks>
/// When this structure is populated by the DHCP Server, the HostName and NetBiosName members may be set to NULL.
/// </remarks>
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal readonly struct DhcpHostInfoManaged
{
    /// <summary>
    /// DHCP_IP_ADDRESS value that contains the IP address of the DHCP server.
    /// </summary>
    public readonly DhcpIpAddress IpAddress;
    /// <summary>
    /// Unicode string that contains the NetBIOS name of the DHCP server.
    /// </summary>
    private readonly string NetBiosName;
    /// <summary>
    /// Unicode string that contains the network name of the DHCP server.
    /// </summary>
    private readonly string ServerName;

    public DhcpHostInfoManaged(DhcpIpAddress ipAddress, string netBiosName, string serverName)
    {
        IpAddress = ipAddress;
        NetBiosName = netBiosName;
        ServerName = serverName;
    }
}
