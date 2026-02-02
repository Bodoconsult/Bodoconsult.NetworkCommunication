// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System.Net;
using System.Runtime.InteropServices;
using Bodoconsult.Network.Windows.Dhcp.Native;

namespace Bodoconsult.Network.Windows.Dhcp;

[Serializable]
public struct DhcpServerIpAddress : IEquatable<DhcpServerIpAddress>, IEquatable<IPAddress>
{
#pragma warning disable IDE0032 // Use auto property
    /// <summary>
    /// IP Address stored in big-endian (network) order
    /// </summary>
    private readonly uint _address;
#pragma warning restore IDE0032 // Use auto property

    public DhcpServerIpAddress(uint nativeAddress)
    {
        _address = nativeAddress;
    }

    public DhcpServerIpAddress(string address)
    {
        _address = BitHelper.StringToIpAddress(address);
    }

    public DhcpServerIpAddress(IPAddress address)
    {
        if (address.AddressFamily != global::System.Net.Sockets.AddressFamily.InterNetwork)
        {
            throw new ArgumentOutOfRangeException(nameof(address), "Only IPv4 addresses are supported");
        }

#pragma warning disable CS0618 // Type or member is obsolete
        var ip = (uint)address.Address;
#pragma warning restore CS0618 // Type or member is obsolete

        // DhcpServerIpAddress always stores in network order
        // IPAddress stores in host order
        _address = BitHelper.HostToNetworkOrder(ip);
    }

    /// <summary>
    /// IP Address in network order
    /// </summary>
    public uint Native => _address;

    public byte[] GetBytes()
    {
        var buffer = new byte[4];
        BitHelper.Write(buffer, 0, _address);
        return buffer;
    }

    public byte GetByte(int index)
    {
        if (index < 0 || index > 3)
            throw new ArgumentOutOfRangeException(nameof(index));

        return (byte)(_address >> ((3 - index) * 8));
    }

    public static DhcpServerIpAddress Empty => new(0);
    public static DhcpServerIpAddress FromNative(uint nativeAddress) => new(nativeAddress);
    public static DhcpServerIpAddress FromNative(int nativeAddress) => new((uint)nativeAddress);
    internal static DhcpServerIpAddress FromNative(IntPtr pointer)
    {
        if (pointer == IntPtr.Zero)
            throw new ArgumentNullException(nameof(pointer));

        return new DhcpServerIpAddress((uint)BitHelper.HostToNetworkOrder(Marshal.ReadInt32(pointer)));
    }

    public static DhcpServerIpAddress FromString(string address) => new(address);

    public override string ToString() => BitHelper.IpAddressToString(_address);

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;

        if (obj is DhcpServerIpAddress sia)
            return Equals(sia);
        else if (obj is IPAddress ia)
            return Equals(ia);

        return false;
    }

    public override int GetHashCode() => (int)_address;

    public bool Equals(DhcpServerIpAddress other) => _address == other._address;

    public bool Equals(IPAddress other)
    {
        if (other == null || other.AddressFamily != global::System.Net.Sockets.AddressFamily.InterNetwork)
            return false;

#pragma warning disable CS0618 // Type or member is obsolete
        var otherIp = (uint)other.Address;
#pragma warning restore CS0618 // Type or member is obsolete

        return _address == BitHelper.HostToNetworkOrder(otherIp);
    }

    public static bool operator ==(DhcpServerIpAddress lhs, DhcpServerIpAddress rhs) => lhs.Equals(rhs);

    public static bool operator !=(DhcpServerIpAddress lhs, DhcpServerIpAddress rhs) => !lhs.Equals(rhs);

    public static bool operator ==(DhcpServerIpAddress lhs, IPAddress rhs) => lhs.Equals(rhs);

    public static bool operator !=(DhcpServerIpAddress lhs, IPAddress rhs) => !lhs.Equals(rhs);

    internal DhcpIpAddress ToNativeAsHost() => new(BitHelper.NetworkToHostOrder(_address));

    internal DhcpIpAddress ToNativeAsNetwork() => new(_address);

    public static explicit operator uint(DhcpServerIpAddress address) => address._address;
    public static explicit operator DhcpServerIpAddress(uint address) => new(address);
    public static explicit operator int(DhcpServerIpAddress address) => (int)address._address;
    public static explicit operator DhcpServerIpAddress(int address) => new((uint)address);

    public static implicit operator DhcpServerIpAddress(IPAddress address) => new(address);
    public static implicit operator IPAddress(DhcpServerIpAddress address)
        // IPAddress stores in host order; DhcpServerIpAddress always stores in network order
        => new(BitHelper.NetworkToHostOrder(address._address));

    public static explicit operator DhcpServerIpMask(DhcpServerIpAddress address) => new(address._address);
    public static implicit operator string(DhcpServerIpAddress address) => address.ToString();
    public static implicit operator DhcpServerIpAddress(string address) => FromString(address);

    public static bool operator >(DhcpServerIpAddress a, DhcpServerIpAddress b) => a._address > b._address;
    public static bool operator >=(DhcpServerIpAddress a, DhcpServerIpAddress b) => a._address >= b._address;
    public static bool operator <(DhcpServerIpAddress a, DhcpServerIpAddress b) => a._address < b._address;
    public static bool operator <=(DhcpServerIpAddress a, DhcpServerIpAddress b) => a._address <= b._address;
}