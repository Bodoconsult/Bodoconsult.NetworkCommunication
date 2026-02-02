// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System.Runtime.InteropServices;

namespace Bodoconsult.Network.Windows.Dhcp.Native;

[StructLayout(LayoutKind.Sequential, Size = 4)]
internal readonly struct DhcpIpMask
{
    private readonly uint IpMask;

    public DhcpIpMask(uint ipMask)
    {
        IpMask = ipMask;
    }

    public override string ToString() => BitHelper.IpAddressToString(IpMask);

    public static explicit operator uint(DhcpIpMask ipMask) => ipMask.IpMask;
    public static explicit operator DhcpIpMask(uint ipMask) => new(ipMask);
    public static explicit operator int(DhcpIpMask ipMask) => (int)ipMask.IpMask;
    public static explicit operator DhcpIpMask(int ipMask) => new((uint)ipMask);

    public DhcpServerIpMask AsHostToIpMask() => new(BitHelper.HostToNetworkOrder(IpMask));

    public DhcpServerIpMask AsNetworkToIpMask() => new(IpMask);
}
