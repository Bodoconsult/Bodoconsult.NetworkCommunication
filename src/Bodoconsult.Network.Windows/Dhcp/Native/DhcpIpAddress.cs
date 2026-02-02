// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System.Runtime.InteropServices;

namespace Bodoconsult.Network.Windows.Dhcp.Native;

[StructLayout(LayoutKind.Sequential, Size = 4)]
internal readonly struct DhcpIpAddress
{
    private readonly uint IpAddress;

    public DhcpIpAddress(uint ipAddress)
    {
        IpAddress = ipAddress;
    }

    public static DhcpIpAddress FromString(string ipAddress) => new(BitHelper.StringToIpAddress(ipAddress));
    public override string ToString() => BitHelper.IpAddressToString(IpAddress);

    public DhcpServerIpAddress AsHostToIpAddress() => new(BitHelper.HostToNetworkOrder(IpAddress));
    public DhcpServerIpAddress AsNetworkToIpAddress() => new(IpAddress);

    public static explicit operator DhcpIpAddress(int ipAddress) => new((uint)ipAddress);
    public static explicit operator int(DhcpIpAddress ipAddress) => (int)ipAddress.IpAddress;

    public static DhcpIpAddress operator &(DhcpIpAddress address, DhcpIpMask mask)
        => (DhcpIpAddress)((int)address & (int)mask);
}
