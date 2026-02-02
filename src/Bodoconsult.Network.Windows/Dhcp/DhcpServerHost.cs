// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System.Text;
using Bodoconsult.Network.Windows.Dhcp.Native;

namespace Bodoconsult.Network.Windows.Dhcp;

public class DhcpServerHost : IDhcpServerHost
{
    public DhcpServerIpAddress Address { get; }
    public string NetBiosName { get; }
    public string ServerName { get; }

    private DhcpServerHost(DhcpServerIpAddress address, string netBiosName, string serverName)
    {
        Address = address;
        NetBiosName = netBiosName;
        ServerName = serverName;
    }

    public static DhcpServerHost Empty { get; } = new(DhcpServerIpAddress.Empty, null, null);

    internal static DhcpServerHost FromNative(ref DhcpHostInfo native)
    {
        return new DhcpServerHost(address: native.IpAddress.AsHostToIpAddress(),
            netBiosName: native.NetBiosName,
            serverName: native.ServerName);
    }

    internal static DhcpServerHost FromNative(DhcpHostInfo native)
    {
        return new DhcpServerHost(address: native.IpAddress.AsHostToIpAddress(),
            netBiosName: native.NetBiosName,
            serverName: native.ServerName);
    }

    internal DhcpHostInfoManaged ToNative() => new(Address.ToNativeAsNetwork(), NetBiosName, ServerName);

    public override string ToString()
    {
        var builder = new StringBuilder(Address);

        if (!string.IsNullOrEmpty(ServerName))
            builder.Append(" [").Append(ServerName).Append("]");
        else if (!string.IsNullOrEmpty(NetBiosName))
            builder.Append(" [").Append(NetBiosName).Append("]");

        return builder.ToString();
    }
}