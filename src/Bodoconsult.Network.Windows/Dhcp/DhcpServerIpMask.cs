// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System.Runtime.InteropServices;
using Bodoconsult.Network.Windows.Dhcp.Native;

namespace Bodoconsult.Network.Windows.Dhcp;

[Serializable]
public struct DhcpServerIpMask
{
#pragma warning disable IDE0032// Use auto property
    /// <summary>
    /// IP Mask stored in big-endian (network) order
    /// </summary>
    private readonly uint _mask;
#pragma warning restore IDE0032 // Use auto property

    public DhcpServerIpMask(uint mask)
    {
        _mask = mask;
    }
    public DhcpServerIpMask(string mask)
    {
        _mask = BitHelper.StringToIpAddress(mask);
    }

    /// <summary>
    /// IP Mask in network order
    /// </summary>
    public uint Native => _mask;

    public static DhcpServerIpMask Empty => new(0);
    public static DhcpServerIpMask FromNative(uint nativeMask) => new(nativeMask);

    public static DhcpServerIpMask FromNative(int nativeMask) => new((uint)nativeMask);

    internal static DhcpServerIpMask FromNative(IntPtr pointer)
    {
        if (pointer == IntPtr.Zero)
            throw new ArgumentNullException(nameof(pointer));

        return new DhcpServerIpMask((uint)BitHelper.HostToNetworkOrder(Marshal.ReadInt32(pointer)));
    }

    public static DhcpServerIpMask FromString(string mask) => new(mask);

    public static DhcpServerIpMask FromSignificantBits(int bitCount)
    {
        if (bitCount > 32 || bitCount < 0)
            throw new ArgumentOutOfRangeException(nameof(bitCount));

        if (bitCount == 0)
            return new DhcpServerIpMask(0U);

        var m = unchecked((int)0x8000_0000) >> (bitCount - 1); // signed int

        return new DhcpServerIpMask((uint)m);
    }

    public int SignificantBits => BitHelper.HighSignificantBits(_mask);

    public DhcpServerIpRange GetDhcpIpRange(DhcpServerIpAddress address) => DhcpServerIpRange.AsDhcpScope(address, this);
    public DhcpServerIpRange GetDhcpAndBootpIpRange(DhcpServerIpAddress address) => DhcpServerIpRange.AsDhcpAndBootpScope(address, this);
    public DhcpServerIpRange GetBootpIpRange(DhcpServerIpAddress address) => DhcpServerIpRange.AsBootpScope(address, this);
    internal DhcpServerIpRange GetIpRange(DhcpServerIpAddress address, DhcpServerIpRangeType type) => DhcpServerIpRange.FromMask(address, this, type);

    internal DhcpIpMask ToNativeAsHost() => new(BitHelper.NetworkToHostOrder(_mask));
    internal DhcpIpMask ToNativeAsNetwork() => new(_mask);

    public override string ToString() => BitHelper.IpAddressToString(_mask);

    public static explicit operator DhcpServerIpAddress(DhcpServerIpMask mask) => new(mask._mask);
    public static explicit operator DhcpServerIpMask(string mask) => new(mask);
}