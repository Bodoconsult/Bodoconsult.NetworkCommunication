// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System.Net;
using Bodoconsult.Network.Windows.Dhcp.Native;

namespace Bodoconsult.Network.Windows.Dhcp;

public struct DhcpServerIpRange : IEquatable<DhcpServerIpRange>
{
    public const int DefaultBootpClientsAllocated = 0;
    public const int DefaultMaxBootpAllowed = -1;

#pragma warning disable IDE0032 // Use auto property
    private readonly DhcpServerIpAddress _startAddress;
    private readonly DhcpServerIpAddress _endAddress;
    private readonly DhcpServerIpRangeType _type;
    private readonly int _bootpClientsAllocated;
    private readonly int _maxBootpAllowed;
#pragma warning restore IDE0032 // Use auto property

    public DhcpServerIpAddress StartAddress => _startAddress;
    public DhcpServerIpAddress EndAddress => _endAddress;
    public DhcpServerIpRangeType Type => _type;

    /// <summary>
    /// Specifies the number of BOOTP clients with addresses served from this range.
    /// </summary>
    public int BootpClientsAllocated => _bootpClientsAllocated;
    /// <summary>
    /// Specifies the maximum number of BOOTP clients this range is allowed to serve.
    /// </summary>
    public int MaxBootpAllowed => _maxBootpAllowed;

    private DhcpServerIpRange(DhcpServerIpAddress startAddress, DhcpServerIpAddress endAddress, DhcpServerIpRangeType type, int bootpClientsAllocated, int maxBootpAllowed)
    {
        if (startAddress > endAddress)
            throw new ArgumentOutOfRangeException(nameof(endAddress), "The ip range start address cannot be greater than the end address");

        _startAddress = startAddress;
        _endAddress = endAddress;
        _type = type;
        _bootpClientsAllocated = bootpClientsAllocated;
        _maxBootpAllowed = maxBootpAllowed;
    }

    private DhcpServerIpRange(DhcpServerIpAddress startAddress, DhcpServerIpAddress endAddress, DhcpServerIpRangeType type)
        : this(startAddress, endAddress, type, DefaultBootpClientsAllocated, DefaultMaxBootpAllowed) { }

    public static DhcpServerIpRange AsDhcpAndBootpScope(DhcpServerIpAddress startAddress, DhcpServerIpAddress endAddress, int bootpClientsAllocated = DefaultBootpClientsAllocated, int maxBootpAllowed = DefaultMaxBootpAllowed)
        => new(startAddress, endAddress, DhcpServerIpRangeType.ScopeDhcpAndBootp, bootpClientsAllocated, maxBootpAllowed);
    public static DhcpServerIpRange AsDhcpAndBootpScope(DhcpServerIpAddress address, DhcpServerIpMask mask, int bootpClientsAllocated = DefaultBootpClientsAllocated, int maxBootpAllowed = DefaultMaxBootpAllowed)
        => FromMask(address, mask, DhcpServerIpRangeType.ScopeDhcpAndBootp, bootpClientsAllocated, maxBootpAllowed);
    public static DhcpServerIpRange AsDhcpAndBootpScope(string cidrSubnet, int bootpClientsAllocated = DefaultBootpClientsAllocated, int maxBootpAllowed = DefaultMaxBootpAllowed)
        => FromCidr(cidrSubnet, DhcpServerIpRangeType.ScopeDhcpAndBootp, bootpClientsAllocated, maxBootpAllowed);

    public static DhcpServerIpRange AsBootpScope(DhcpServerIpAddress startAddress, DhcpServerIpAddress endAddress, int bootpClientsAllocated = DefaultBootpClientsAllocated, int maxBootpAllowed = DefaultMaxBootpAllowed)
        => new(startAddress, endAddress, DhcpServerIpRangeType.ScopeBootpOnly, bootpClientsAllocated, maxBootpAllowed);
    public static DhcpServerIpRange AsBootpScope(DhcpServerIpAddress address, DhcpServerIpMask mask, int bootpClientsAllocated = DefaultBootpClientsAllocated, int maxBootpAllowed = DefaultMaxBootpAllowed)
        => FromMask(address, mask, DhcpServerIpRangeType.ScopeBootpOnly, bootpClientsAllocated, maxBootpAllowed);
    public static DhcpServerIpRange AsBootpScope(string cidrSubnet, int bootpClientsAllocated = DefaultBootpClientsAllocated, int maxBootpAllowed = DefaultMaxBootpAllowed)
        => FromCidr(cidrSubnet, DhcpServerIpRangeType.ScopeBootpOnly, bootpClientsAllocated, maxBootpAllowed);

    public static DhcpServerIpRange AsDhcpScope(DhcpServerIpAddress startAddress, DhcpServerIpAddress endAddress) =>
        new(startAddress, endAddress, DhcpServerIpRangeType.ScopeDhcpOnly);
    public static DhcpServerIpRange AsDhcpScope(DhcpServerIpAddress address, DhcpServerIpMask mask)
        => FromMask(address, mask, DhcpServerIpRangeType.ScopeDhcpOnly);
    public static DhcpServerIpRange AsDhcpScope(string cidrSubnet)
        => FromCidr(cidrSubnet, DhcpServerIpRangeType.ScopeDhcpOnly);

    public static DhcpServerIpRange AsExcluded(DhcpServerIpAddress startAddress, DhcpServerIpAddress endAddress) =>
        new(startAddress, endAddress, DhcpServerIpRangeType.Excluded);
    public static DhcpServerIpRange AsExcluded(DhcpServerIpAddress address, DhcpServerIpMask mask)
        => FromMask(address, mask, DhcpServerIpRangeType.Excluded);
    public static DhcpServerIpRange AsExcluded(string cidrSubnet)
        => FromCidr(cidrSubnet, DhcpServerIpRangeType.Excluded);

    internal static DhcpServerIpRange FromMask(DhcpServerIpAddress address, DhcpServerIpMask mask, DhcpServerIpRangeType type)
        => FromMask(address, mask, type, DefaultBootpClientsAllocated, DefaultMaxBootpAllowed);

    private static DhcpServerIpRange FromMask(DhcpServerIpAddress address, DhcpServerIpMask mask, DhcpServerIpRangeType type, int bootpClientsAllocated, int maxBootpAllowed)
    {
        var startAddressNative = address.Native & mask.Native;
        var endAddressNative = (address.Native & mask.Native) | ~mask.Native;

        if (type == DhcpServerIpRangeType.ScopeDhcpOnly ||
            type == DhcpServerIpRangeType.ScopeDhcpAndBootp ||
            type == DhcpServerIpRangeType.ScopeBootpOnly)
        {
            // remove subnet id and broadcast address from range
            startAddressNative++;
            endAddressNative--;
        }

        return new DhcpServerIpRange(startAddress: DhcpServerIpAddress.FromNative(startAddressNative),
            endAddress: DhcpServerIpAddress.FromNative(endAddressNative),
            type: type,
            bootpClientsAllocated: bootpClientsAllocated,
            maxBootpAllowed: maxBootpAllowed);
    }

    private static DhcpServerIpRange FromCidr(string cidrSubnet, DhcpServerIpRangeType type)
        => FromCidr(cidrSubnet, type, DefaultBootpClientsAllocated, DefaultMaxBootpAllowed);
    private static DhcpServerIpRange FromCidr(string cidrSubnet, DhcpServerIpRangeType type, int bootpClientsAllocated, int maxBootpAllowed)
    {
        if (string.IsNullOrEmpty(cidrSubnet))
            throw new ArgumentNullException(nameof(cidrSubnet));

        var slashIndex = cidrSubnet.IndexOf('/');
        if (slashIndex < 7 || !BitHelper.TryParseByteFromSubstring(cidrSubnet, ++slashIndex, cidrSubnet.Length - slashIndex, out var significantBits))
            throw new ArgumentException("Invalid CIDR subnet notation format");

        var address = DhcpServerIpAddress.FromNative(BitHelper.StringToIpAddress(cidrSubnet, 0, --slashIndex));
        var mask = DhcpServerIpMask.FromSignificantBits(significantBits);

        return FromMask(address, mask, type, bootpClientsAllocated, maxBootpAllowed);
    }

    public DhcpServerIpMask SmallestMask
    {
        get
        {
            var dif = _startAddress.Native ^ _endAddress.Native;
            var bits = BitHelper.HighInsignificantBits(dif);

            return DhcpServerIpMask.FromSignificantBits(bits);
        }
    }

    public bool Contains(IPAddress address) => Contains((DhcpServerIpAddress)address);
    public bool Contains(string address) => Contains(DhcpServerIpAddress.FromString(address));
    public bool Contains(int address) => Contains((DhcpServerIpAddress)address);
    public bool Contains(uint address) => Contains((DhcpServerIpAddress)address);
    public bool Contains(DhcpServerIpRange range) => range._endAddress >= range._startAddress && range._startAddress >= _startAddress && range._endAddress <= _endAddress;

    public bool Contains(DhcpServerIpAddress address) => address >= _startAddress && address <= _endAddress;

    internal static DhcpServerIpRange FromNative(DhcpSubnetElementData native)
    {
        switch (native.ElementType)
        {
            case DhcpSubnetElementType.DhcpIpRanges:
            case DhcpSubnetElementType.DhcpExcludedIpRanges:
                var bootpIpRange = native.ReadIpRange();
                // translate legacy 'DhcpIpRanges' -> 'DhcpIpRangesDhcpOnly'
                var type = (DhcpServerIpRangeType)(native.ElementType == DhcpSubnetElementType.DhcpIpRanges ? DhcpSubnetElementType.DhcpIpRangesDhcpOnly : native.ElementType);
                return new DhcpServerIpRange(startAddress: bootpIpRange.StartAddress.AsNetworkToIpAddress(),
                    endAddress: bootpIpRange.EndAddress.AsNetworkToIpAddress(),
                    type: type);
            default:
                throw new DhcpServerException(nameof(DhcpSubnetElementDataV5), DhcpErrors.ErrorInvalidParameter, "An unexpected subnet element type was encountered");
        }
    }

    internal static DhcpServerIpRange FromNative(DhcpSubnetElementDataV5 native)
    {
        switch (native.ElementType)
        {
            case DhcpSubnetElementType.DhcpIpRanges:
            case DhcpSubnetElementType.DhcpIpRangesDhcpOnly:
            case DhcpSubnetElementType.DhcpIpRangesDhcpBootp:
            case DhcpSubnetElementType.DhcpIpRangesBootpOnly:
                var bootpIpRange = native.ReadBootpIpRange();
                // translate legacy 'DhcpIpRanges' -> 'DhcpIpRangesDhcpOnly'
                var type = (DhcpServerIpRangeType)(native.ElementType == DhcpSubnetElementType.DhcpIpRanges ? DhcpSubnetElementType.DhcpIpRangesDhcpOnly : native.ElementType);
                return new DhcpServerIpRange(startAddress: bootpIpRange.StartAddress.AsNetworkToIpAddress(),
                    endAddress: bootpIpRange.EndAddress.AsNetworkToIpAddress(),
                    type: type,
                    bootpClientsAllocated: bootpIpRange.BootpAllocated,
                    maxBootpAllowed: bootpIpRange.MaxBootpAllowed);
            case DhcpSubnetElementType.DhcpExcludedIpRanges:
                var ipRange = native.ReadIpRange();
                return new DhcpServerIpRange(startAddress: ipRange.StartAddress.AsNetworkToIpAddress(),
                    endAddress: ipRange.EndAddress.AsNetworkToIpAddress(),
                    type: DhcpServerIpRangeType.Excluded);
            default:
                throw new DhcpServerException(nameof(DhcpSubnetElementDataV5), DhcpErrors.ErrorInvalidParameter, "An unexpected subnet element type was encountered");
        }
    }

    internal static DhcpServerIpRange FromNative(ref DhcpIpRange native, DhcpServerIpRangeType type)
    {
        return new DhcpServerIpRange(startAddress: native.StartAddress.AsNetworkToIpAddress(),
            endAddress: native.EndAddress.AsNetworkToIpAddress(),
            type: type);
    }

    internal static DhcpServerIpRange FromNative(ref DhcpBootpIpRange native, DhcpServerIpRangeType type)
    {
        return new DhcpServerIpRange(startAddress: native.StartAddress.AsNetworkToIpAddress(),
            endAddress: native.EndAddress.AsNetworkToIpAddress(),
            type: type,
            bootpClientsAllocated: native.BootpAllocated,
            maxBootpAllowed: native.MaxBootpAllowed);
    }

    internal DhcpBootpIpRange ToNativeBootpIpRange()
        => new(StartAddress.ToNativeAsNetwork(), EndAddress.ToNativeAsNetwork(), BootpClientsAllocated, MaxBootpAllowed);

    internal DhcpIpRange ToNativeIpRange() => new(StartAddress.ToNativeAsNetwork(), EndAddress.ToNativeAsNetwork());

    public override string ToString() => $"{_startAddress} - {_endAddress} [{_type}]";

    public override int GetHashCode()
    {
        var result = _startAddress.GetHashCode();
        result = (result * 397) ^ _endAddress.GetHashCode();
        result = (result * 397) ^ (int)_type;
        result = (result * 397) ^ _bootpClientsAllocated;
        result = (result * 397) ^ _maxBootpAllowed;
        return result;
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;

        if (obj is DhcpServerIpRange ha)
            return Equals(ha);

        return false;
    }

    public bool Equals(DhcpServerIpRange other)
    {
        return
            _startAddress == other._startAddress &&
            _endAddress == other._endAddress &&
            _type == other._type &&
            _bootpClientsAllocated == other._bootpClientsAllocated &&
            _maxBootpAllowed == other._maxBootpAllowed;
    }

    public static bool operator ==(DhcpServerIpRange lhs, DhcpServerIpRange rhs) => lhs.Equals(rhs);

    public static bool operator !=(DhcpServerIpRange lhs, DhcpServerIpRange rhs) => !lhs.Equals(rhs);
}