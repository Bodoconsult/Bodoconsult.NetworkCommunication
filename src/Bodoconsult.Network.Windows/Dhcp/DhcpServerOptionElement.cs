// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System.Runtime.InteropServices;
using Bodoconsult.Network.Windows.Dhcp.Native;

namespace Bodoconsult.Network.Windows.Dhcp;

public abstract class DhcpServerOptionElement : IDhcpServerOptionElement
{
    public abstract DhcpServerOptionElementType Type { get; }

    public abstract object Value { get; }
    public abstract string ValueFormatted { get; }

    internal static List<DhcpServerOptionElement> CreateElement(byte value) => new(1) { new DhcpServerOptionElementByte(value) };
    internal static List<DhcpServerOptionElement> CreateElement(List<byte> values)
        => values.Select(v => (DhcpServerOptionElement)new DhcpServerOptionElementByte(v)).ToList();
    internal static List<DhcpServerOptionElement> CreateElement(short value) => new(1) { new DhcpServerOptionElementWord(value) };
    internal static List<DhcpServerOptionElement> CreateElement(List<short> values)
        => values.Select(v => (DhcpServerOptionElement)new DhcpServerOptionElementWord(v)).ToList();
    internal static List<DhcpServerOptionElement> CreateElement(int value) => new(1) { new DhcpServerOptionElementDWord(value) };
    internal static List<DhcpServerOptionElement> CreateElement(List<int> values)
        => values.Select(v => (DhcpServerOptionElement)new DhcpServerOptionElementDWord(v)).ToList();
    internal static List<DhcpServerOptionElement> CreateElement(long value) => new(1) { new DhcpServerOptionElementDWordDWord(value) };
    internal static List<DhcpServerOptionElement> CreateElement(List<long> values)
        => values.Select(v => (DhcpServerOptionElement)new DhcpServerOptionElementDWordDWord(v)).ToList();
    internal static List<DhcpServerOptionElement> CreateElement(DhcpServerIpAddress value) => new(1) { new DhcpServerOptionElementIpAddress(value) };
    internal static List<DhcpServerOptionElement> CreateElement(List<DhcpServerIpAddress> values)
        => values.Select(v => (DhcpServerOptionElement)new DhcpServerOptionElementIpAddress(v)).ToList();
    internal static List<DhcpServerOptionElement> CreateStringElement(string value)
        => new(1) { new DhcpServerOptionElementString(value) };
    internal static List<DhcpServerOptionElement> CreateStringElement(List<string> values)
        => values.Select(v => (DhcpServerOptionElement)new DhcpServerOptionElementString(v)).ToList();
    internal static List<DhcpServerOptionElement> CreateBinaryElement(byte[] value)
        => new(1) { new DhcpServerOptionElementBinary(DhcpServerOptionElementType.BinaryData, value) };
    internal static List<DhcpServerOptionElement> CreateBinaryElement(List<byte[]> values)
        => values.Select(v => (DhcpServerOptionElement)new DhcpServerOptionElementBinary(DhcpServerOptionElementType.BinaryData, v)).ToList();
    internal static List<DhcpServerOptionElement> CreateEncapsulatedElement(byte[] value)
        => new(1) { new DhcpServerOptionElementBinary(DhcpServerOptionElementType.EncapsulatedData, value) };
    internal static List<DhcpServerOptionElement> CreateEncapsulatedElement(List<byte[]> values)
        => values.Select(v => (DhcpServerOptionElement)new DhcpServerOptionElementBinary(DhcpServerOptionElementType.EncapsulatedData, v)).ToList();
    internal static List<DhcpServerOptionElement> CreateIpv6AddressElement(string value)
        => new(1) { new DhcpServerOptionElementIpv6Address(value) };
    internal static List<DhcpServerOptionElement> CreateIpv6AddressElement(List<string> values)
        => values.Select(v => (DhcpServerOptionElement)new DhcpServerOptionElementIpv6Address(v)).ToList();

    internal static IEnumerable<DhcpServerOptionElement> ReadNativeElements(DhcpOptionData elementArray)
    {
        foreach (var element in elementArray.Elements)
            yield return ReadNative(element);
    }

    internal static DhcpOptionDataManaged WriteNative(IEnumerable<IDhcpServerOptionElement> elements)
    {
        return new DhcpOptionDataManaged(elements.Select(e => ((DhcpServerOptionElement)e).ToNative()).ToArray());
    }

    private static DhcpServerOptionElement ReadNative(DhcpOptionDataElement element)
    {
        switch (element.OptionType)
        {
            case DhcpOptionDataType.DhcpByteOption:
                return DhcpServerOptionElementByte.ReadNative(element);
            case DhcpOptionDataType.DhcpWordOption:
                return DhcpServerOptionElementWord.ReadNative(element);
            case DhcpOptionDataType.DhcpDWordOption:
                return DhcpServerOptionElementDWord.ReadNative(element);
            case DhcpOptionDataType.DhcpDWordDWordOption:
                return DhcpServerOptionElementDWordDWord.ReadNative(element);
            case DhcpOptionDataType.DhcpIpAddressOption:
                return DhcpServerOptionElementIpAddress.ReadNative(element);
            case DhcpOptionDataType.DhcpStringDataOption:
                return DhcpServerOptionElementString.ReadNative(element);
            case DhcpOptionDataType.DhcpBinaryDataOption:
            case DhcpOptionDataType.DhcpEncapsulatedDataOption:
                return DhcpServerOptionElementBinary.ReadNative(element);
            case DhcpOptionDataType.DhcpIpv6AddressOption:
                return DhcpServerOptionElementIpv6Address.ReadNative(element);
            default:
                throw new InvalidCastException($"Unknown Option Data Type: {element.OptionType}");
        }
    }

    internal abstract DhcpOptionDataElementManaged ToNative();

    public override string ToString() => $"{Type}: {ValueFormatted}";

    public override int GetHashCode()
    {
        // GetHashCode must be overridden by all implementing classes
        return base.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;

        if (obj is DhcpServerOptionElement oe)
            return Equals(oe);

        return false;
    }

    public abstract bool Equals(IDhcpServerOptionElement other);
}

public class DhcpServerOptionElementByte : DhcpServerOptionElement
{
    public override DhcpServerOptionElementType Type => DhcpServerOptionElementType.Byte;
    public override object Value => RawValue;
    public override string ValueFormatted => BitHelper.ReadHexString(RawValue);

    public byte RawValue { get; }

    internal DhcpServerOptionElementByte(byte value)
    {
        RawValue = value;
    }

    internal static DhcpServerOptionElementByte ReadNative(DhcpOptionDataElement native) => new(native.ByteOption);

    internal override DhcpOptionDataElementManaged ToNative() => new(RawValue);

    public override bool Equals(IDhcpServerOptionElement other)
    {
        if (other == null)
            return false;

        if (other is DhcpServerOptionElementByte oe)
            return RawValue == oe.RawValue;

        return false;
    }

    public override int GetHashCode()
        => RawValue;
}

public class DhcpServerOptionElementWord : DhcpServerOptionElement
{
    public override DhcpServerOptionElementType Type => DhcpServerOptionElementType.Word;
    public override object Value => RawValue;
    public override string ValueFormatted => RawValue.ToString("N0");

    public short RawValue { get; }

    internal DhcpServerOptionElementWord(short value)
    {
        RawValue = value;
    }

    internal static DhcpServerOptionElementWord ReadNative(DhcpOptionDataElement native) => new(native.WordOption);

    internal override DhcpOptionDataElementManaged ToNative() => new(RawValue);

    public override bool Equals(IDhcpServerOptionElement other)
    {
        if (other == null)
            return false;

        if (other is DhcpServerOptionElementWord oe)
            return RawValue == oe.RawValue;

        return false;
    }

    public override int GetHashCode()
        => RawValue;
}

public class DhcpServerOptionElementDWord : DhcpServerOptionElement
{
    public override DhcpServerOptionElementType Type => DhcpServerOptionElementType.DWord;
    public override object Value => RawValue;
    public override string ValueFormatted => RawValue.ToString("N0");

    public int RawValue { get; }

    internal DhcpServerOptionElementDWord(int value)
    {
        RawValue = value;
    }

    internal static DhcpServerOptionElementDWord ReadNative(DhcpOptionDataElement native) => new(native.DWordOption);

    internal override DhcpOptionDataElementManaged ToNative() => new(RawValue);

    public override bool Equals(IDhcpServerOptionElement other)
    {
        if (other == null)
            return false;

        if (other is DhcpServerOptionElementDWord oe)
            return RawValue == oe.RawValue;

        return false;
    }

    public override int GetHashCode()
        => RawValue;
}

public class DhcpServerOptionElementDWordDWord : DhcpServerOptionElement
{
    public override DhcpServerOptionElementType Type => DhcpServerOptionElementType.DWordDWord;
    public override object Value => RawValue;
    public override string ValueFormatted => RawValue.ToString("N0");

    public long RawValue { get; }

    internal DhcpServerOptionElementDWordDWord(long value)
    {
        RawValue = value;
    }

    internal static DhcpServerOptionElementDWordDWord ReadNative(DhcpOptionDataElement native) =>
        new(native.DWordDWordOption);

    internal override DhcpOptionDataElementManaged ToNative() => new(RawValue);

    public override bool Equals(IDhcpServerOptionElement other)
    {
        if (other == null)
            return false;

        if (other is DhcpServerOptionElementDWordDWord oe)
            return RawValue == oe.RawValue;

        return false;
    }

    public override int GetHashCode()
        => ((int)(RawValue >> 32) * 397) ^ ((int)RawValue);
}

public class DhcpServerOptionElementIpAddress : DhcpServerOptionElement
{
    public override DhcpServerOptionElementType Type => DhcpServerOptionElementType.IpAddress;
    public override object Value => RawValue;
    public override string ValueFormatted => _address;

    private readonly DhcpServerIpAddress _address;

    public uint RawValue => _address.Native;

    internal DhcpServerOptionElementIpAddress(DhcpServerIpAddress value)
    {
        _address = value;
    }
    internal DhcpServerOptionElementIpAddress(DhcpIpAddress value)
        : this(value.AsNetworkToIpAddress()) { }

    internal static DhcpServerOptionElementIpAddress ReadNative(DhcpOptionDataElement native) =>
        new(native.IpAddressOption);

    internal override DhcpOptionDataElementManaged ToNative() => new(new DhcpIpAddress(RawValue));

    public override bool Equals(IDhcpServerOptionElement other)
    {
        if (other == null)
            return false;

        if (other is DhcpServerOptionElementIpAddress oe)
            return _address == oe._address;

        return false;
    }

    public override int GetHashCode()
        => _address.GetHashCode();
}

public class DhcpServerOptionElementString : DhcpServerOptionElement
{
    public override DhcpServerOptionElementType Type => DhcpServerOptionElementType.StringData;
    public override object Value => RawValue;
    public override string ValueFormatted => RawValue;

    public string RawValue { get; }

    internal DhcpServerOptionElementString(string value)
    {
        RawValue = value;
    }

    internal static DhcpServerOptionElementString ReadNative(DhcpOptionDataElement native) =>
        new(Marshal.PtrToStringUni(native.StringDataOption));

    internal override DhcpOptionDataElementManaged ToNative() => new((DhcpOptionDataType)Type, RawValue);

    public override bool Equals(IDhcpServerOptionElement other)
    {
        if (other == null)
            return false;

        if (other is DhcpServerOptionElementString oe)
            return string.Equals(RawValue, oe.RawValue, StringComparison.Ordinal);

        return false;
    }

    public override int GetHashCode()
        => RawValue.GetHashCode();
}

public class DhcpServerOptionElementBinary : DhcpServerOptionElement
{
    private readonly DhcpServerOptionElementType _type;

    public override DhcpServerOptionElementType Type => _type;
    public override object Value => RawValue;
    public override string ValueFormatted => (RawValue == null) ? null : BitHelper.ReadHexString(RawValue, ' ');

    public byte[] RawValue { get; }

    internal DhcpServerOptionElementBinary(DhcpServerOptionElementType type, byte[] value)
    {
        _type = type;
        RawValue = value;
    }

    internal static DhcpServerOptionElementBinary ReadNative(DhcpOptionDataElement native)
        => new((DhcpServerOptionElementType)native.OptionType, native.BinaryDataOption.Data);

    internal override DhcpOptionDataElementManaged ToNative()
        => new((DhcpOptionDataType)Type, new DhcpBinaryDataManaged(RawValue));

    public override bool Equals(IDhcpServerOptionElement other)
    {
        if (other == null)
            return false;

        if (other is DhcpServerOptionElementBinary oe)
        {
            if (RawValue == null && oe.RawValue == null)
                return true;

            if (RawValue == null || oe.RawValue == null)
                return false;

            if (RawValue.Length == 0 && oe.RawValue.Length == 0)
                return true;

            return Enumerable.SequenceEqual(RawValue, oe.RawValue);
        }

        return false;
    }

    public override int GetHashCode()
    {
        var result = 0;
        if (RawValue != null && RawValue.Length > 0)
        {
            foreach (var b in RawValue)
                result = (result * 397) ^ b;
        }
        return result;
    }
}

public class DhcpServerOptionElementIpv6Address : DhcpServerOptionElement
{
    public override DhcpServerOptionElementType Type => DhcpServerOptionElementType.Ipv6Address;
    public override object Value => RawValue;
    public override string ValueFormatted => RawValue;

    public string RawValue { get; }

    internal DhcpServerOptionElementIpv6Address(string value)
    {
        RawValue = value;
    }

    internal static DhcpServerOptionElementIpv6Address ReadNative(DhcpOptionDataElement native)
        => new(Marshal.PtrToStringUni(native.Ipv6AddressDataOption));

    internal override DhcpOptionDataElementManaged ToNative() => new((DhcpOptionDataType)Type, RawValue);

    public override bool Equals(IDhcpServerOptionElement other)
    {
        if (other == null)
            return false;

        if (other is DhcpServerOptionElementIpv6Address oe)
            return string.Equals(RawValue, oe.RawValue, StringComparison.Ordinal);

        return false;
    }

    public override int GetHashCode()
        => RawValue.GetHashCode();
}