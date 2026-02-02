// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;

namespace Bodoconsult.Network.Windows.Dhcp;

[Serializable]
public struct DhcpServerPacketOption
{
    private static DhcpServerOptionIdTypes[] _optionIdTypesCache;
    private readonly DhcpServerOptionIds _id;
    private readonly int _length;
    private readonly ulong _dataLong;
    private readonly byte[] _dataArray;

    public DhcpServerOptionIds Id => _id;
    public int DataLength => _length;

    internal DhcpServerPacketOption(DhcpServerOptionIds optionId)
    {
        _id = optionId;
        _dataLong = default;
        _dataArray = null;
        _length = 0;
    }

    internal DhcpServerPacketOption(DhcpServerOptionIds optionId, byte[] data)
    {
        _id = optionId;
        _dataLong = default;
        _dataArray = data;
        _length = data.Length;
    }

    internal DhcpServerPacketOption(DhcpServerOptionIds optionId, ulong data, int length)
    {
        if (length > 8)
            throw new ArgumentOutOfRangeException(nameof(length));

        _id = optionId;
        _dataLong = data;
        _dataArray = null;
        _length = length;
    }

    private static void EnsureTypesCache()
    {
        if (_optionIdTypesCache == null)
        {
            var cache = new DhcpServerOptionIdTypes[256];

            foreach (var field in typeof(DhcpServerOptionIds).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                var attributes = field
                    .GetCustomAttributes(typeof(DhcpServerOptionIdTypeAttribute), false);
                if (attributes.Length == 1 && attributes[0] is DhcpServerOptionIdTypeAttribute attribute)
                {
                    var id = (byte)field.GetValue(null);
                    cache[id] = attribute.Type;
                }
            }

            _optionIdTypesCache = cache;
        }
    }

    public DhcpServerOptionIdTypes Type
    {
        get
        {
            if (_optionIdTypesCache == null)
                EnsureTypesCache();

            return _optionIdTypesCache[(byte)_id];
        }
    }

    public byte[] DataAsBytes(int index, int length)
    {
        if (index + length > _length || index < 0)
            throw new ArgumentOutOfRangeException(nameof(length));

        if (_dataArray != null)
        {
            if (index == 0 && length == _dataArray.Length)
                return _dataArray;
            else if (length == 0)
                return BitHelper.EmptyByteArray;
            else
            {
                var data = new byte[length];
                Array.Copy(_dataArray, index, data, 0, length);
                return data;
            }
        }
        else if (length == 0)
            return BitHelper.EmptyByteArray;
        else
        {
            var data = new byte[length];
            BitHelper.Write(data, 0, _dataLong, index, length);
            return data;
        }
    }

    public byte[] DataAsBytes() => DataAsBytes(0, _length);

    public string DataAsAsciiString(int index, int length)
    {
        var data = DataAsBytes(index, length);
        return BitHelper.ReadAsciiString(data, 0, data.Length);
    }

    public string DataAsAsciiString() => DataAsAsciiString(0, _length);

    public string DataAsUtf8String(int index, int length)
    {
        var data = DataAsBytes(index, length);
        return BitHelper.ReadUtf8String(data, 0, data.Length);
    }

    public string DataAsUtf8String() => DataAsUtf8String(0, _length);

    public string DataAsDnsLabelsString(int index) => BitHelper.ReadDnsLabelsString(_dataArray, index);

    public string DataAsDnsLabelsString() => DataAsDnsLabelsString(0);

    public IEnumerable<string> DataAsDnsLabelsStrings(int index, int length) => BitHelper.ReadDnsLabelsStrings(_dataArray, index, length);

    public long DataAsInt64(int index)
    {
        if (_length < index + BitHelper.Int64Size)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (_dataArray == null)
            return (long)_dataLong; // index must be '0' to reach
        else
            return BitHelper.ReadInt64(_dataArray, index);
    }

    public long DataAsInt64()
    {
        if (_length != BitHelper.Int64Size)
            throw new InvalidOperationException("Invalid option length for this data type");

        return DataAsInt64(0);
    }

    public int DataAsInt32(int index)
    {
        if (_length < index + BitHelper.Int32Size)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (_dataArray == null)
            return (int)(_dataLong >> (32 - (index * 8)));
        else
            return BitHelper.ReadInt32(_dataArray, index);
    }

    public int DataAsInt32()
    {
        if (_length != BitHelper.Int32Size)
            throw new InvalidOperationException("Invalid option length for this data type");

        return DataAsInt32(0);
    }

    public short DataAsInt16(int index)
    {
        if (_length < index + BitHelper.Int16Size)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (_dataArray == null)
            return (short)(_dataLong >> (48 - (index * 8)));
        else
            return BitHelper.ReadInt16(_dataArray, index);
    }

    public short DataAsInt16()
    {
        if (_length != BitHelper.Int16Size)
            throw new InvalidOperationException("Invalid option length for this data type");

        return DataAsInt16(0);
    }

    public byte DataAsByte(int index)
    {
        if (_length < index + BitHelper.ByteSize)
            throw new ArgumentOutOfRangeException(nameof(index));

        if (_dataArray == null)
            return (byte)(_dataLong >> (56 - (index * 8)));
        else
            return _dataArray[index];
    }

    public byte DataAsByte()
    {
        if (_length != BitHelper.ByteSize)
            throw new InvalidOperationException("Invalid option length for this data type");

        return DataAsByte(0);
    }

    public IList<DhcpServerPacketOption> DataAsEncapsulatedOptions()
    {
        var data = DataAsBytes();
        return ParseAll(data, 0, data.Length);
    }

    public DhcpServerIpAddress DataAsIpAddress(int index) => DhcpServerIpAddress.FromNative(DataAsInt32(index));

    public DhcpServerIpAddress DataAsIpAddress() => DhcpServerIpAddress.FromNative(DataAsInt32());

    public string DataAsHexString(int index, int length)
    {
        if (length == 0)
            return string.Empty;
        else
        {
            if (_dataArray == null)
                return BitHelper.ReadHexString(_dataLong, index, length);
            else
                return BitHelper.ReadHexString(_dataArray, index, length);
        }
    }

    public string DataAsHexString() => DataAsHexString(0, _length);

    public string DataAsHexString(int index, int length, char seperator)
    {
        if (length == 0)
            return string.Empty;
        else
        {
            if (_dataArray == null)
                return BitHelper.ReadHexString(_dataLong, index, length, seperator);
            else
                return BitHelper.ReadHexString(_dataArray, index, length, seperator);
        }
    }

    public string DataAsHexString(char seperator) => DataAsHexString(0, _length, seperator);

    public string DataAsFormatted()
    {
        var builder = new StringBuilder();
        DataAsFormatted(builder);
        return builder.ToString();
    }

    public void DataAsFormatted(StringBuilder builder)
    {
        var isExpected = false;

        switch (Type)
        {
            case DhcpServerOptionIdTypes.Pad:
                builder.Append($"[padding]");
                return;
            case DhcpServerOptionIdTypes.End:
                builder.Append($"[end]");
                return;
            case DhcpServerOptionIdTypes.IpAddress:
                if (_length == 4)
                {
                    builder.Append(DataAsIpAddress().ToString());
                    return;
                }
                break;
            case DhcpServerOptionIdTypes.IpAddressList:
                if (_length > 0 && _length % 4 == 0)
                {
                    for (var i = 0; i < _length; i += 4)
                    {
                        if (i != 0)
                            builder.Append("; ");

                        builder.Append(DataAsIpAddress(i));
                    }
                    return;
                }
                break;
            case DhcpServerOptionIdTypes.Byte:
                if (_length == 1)
                {
                    builder.Append(DataAsByte());
                    return;
                }
                break;
            case DhcpServerOptionIdTypes.Int16:
                if (_length == 2)
                {
                    builder.Append(DataAsInt16());
                    return;
                }
                break;
            case DhcpServerOptionIdTypes.UInt16:
                if (_length == 2)
                {
                    builder.Append((ushort)DataAsInt16());
                    return;
                }
                break;
            case DhcpServerOptionIdTypes.Int32:
                if (_length == 4)
                {
                    builder.Append(DataAsInt32());
                    return;
                }
                break;
            case DhcpServerOptionIdTypes.UInt32:
                if (_length == 4)
                {
                    builder.Append((uint)DataAsInt32());
                    return;
                }
                break;
            case DhcpServerOptionIdTypes.AsciiString:
                builder.Append(DataAsAsciiString());
                return;
            case DhcpServerOptionIdTypes.Utf8String:
                builder.Append(DataAsUtf8String());
                return;
            case DhcpServerOptionIdTypes.IpAddressAndSubnet:
                if (_length > 0 && _length % 8 == 0)
                {
                    for (var i = 0; i < _length; i += 8)
                    {
                        if (i != 0)
                            builder.Append("; ");

                        builder.Append(DataAsIpAddress(i));
                        builder.Append('/');
                        builder.Append(((DhcpServerIpMask)DataAsIpAddress(i + 4)).SignificantBits);
                    }
                    return;
                }
                break;
            case DhcpServerOptionIdTypes.IpAddressAndIpAddress:
                if (_length > 0 && _length % 8 == 0)
                {
                    for (var i = 0; i < _length; i += 8)
                    {
                        if (i != 0)
                            builder.Append("; ");

                        builder.Append(DataAsIpAddress(i));
                        builder.Append(" => ");
                        builder.Append(DataAsIpAddress(i + 4));
                    }
                    return;
                }
                break;
            case DhcpServerOptionIdTypes.UInt16List:
                if (_length > 0 && _length % 2 == 0)
                {
                    for (var i = 0; i < _length; i += 2)
                    {
                        if (i != 0)
                            builder.Append("; ");

                        builder.Append((ushort)DataAsInt16(i));
                    }
                    return;
                }
                break;
            case DhcpServerOptionIdTypes.DhcpMessageType:
                if (_length == 1)
                {
                    var type = DataAsByte();
                    builder.Append(((DhcpServerPacketMessageTypes)type).ToString());
                    builder.Append(" [");
                    builder.Append(type);
                    builder.Append("]");
                    return;
                }
                break;
            case DhcpServerOptionIdTypes.DhcpParameterRequestList:
                if (_length > 0)
                {
                    for (var i = 0; i < _length; i++)
                    {
                        if (i != 0)
                            builder.Append("; ");

                        var optionId = DataAsByte(i);
                        builder.Append(((DhcpServerOptionIds)optionId).ToString());
                        builder.Append(" [");
                        builder.Append(optionId);
                        builder.Append("]");
                    }
                    return;
                }
                break;
            case DhcpServerOptionIdTypes.ZeroLengthFlag:
                if (_length == 0)
                {
                    builder.Append($"[present]");
                    return;
                }
                break;
            case DhcpServerOptionIdTypes.ClientFqdn:
                if (_length >= 3)
                {
                    var flags = DataAsByte(0);
                    //var rcode1 = DataAsByte(1);
                    //var rcode2 = DataAsByte(2);
                    if ((flags & 0b0100) != 0) // canonical encoding
                    {
                        builder.AppendDnsLabelsString(DataAsBytes(), 3);
                    }
                    else
                    {
                        builder.Append(DataAsAsciiString(3, _length - 3));
                    }
                    if (flags != 0)
                    {
                        builder.Append("[flags: ");
                        builder.Append(Convert.ToString(flags, 2).Substring(0, 4));
                        if ((flags & 0b1000) != 0)
                            builder.Append('N');
                        else
                            builder.Append('n');
                        if ((flags & 0b0100) != 0)
                            builder.Append('E');
                        else
                            builder.Append('e');
                        if ((flags & 0b0010) != 0)
                            builder.Append('O');
                        else
                            builder.Append('o');
                        if ((flags & 0b0001) != 0)
                            builder.Append('S');
                        else
                            builder.Append('s');
                        builder.Append(']');
                    }

                    return;
                }
                break;
            case DhcpServerOptionIdTypes.DnsName:
                if (_length > 0)
                {
                    builder.AppendDnsLabelsString(DataAsBytes(), 0);
                    return;
                }
                break;
            case DhcpServerOptionIdTypes.DnsNameList:
                if (_length > 0)
                {
                    builder.AppendDnsLabelsStrings(DataAsBytes(), 0, _length, "; ");
                    return;
                }
                break;
            case DhcpServerOptionIdTypes.ClientUuid:
                if (_length == 17 && DataAsByte(0) == 0)
                {
                    builder.Append(new Guid(DataAsBytes(1, 16)).ToString("B"));
                    return;
                }
                break;
            case DhcpServerOptionIdTypes.SipServers:
                if (_length > 1)
                {
                    var encoding = DataAsByte(0);
                    if (encoding == 0)
                    {
                        builder.AppendDnsLabelsStrings(DataAsBytes(1, _length - 1), 0, _length - 1, "; ");
                        return;
                    }
                    else if (encoding == 1 && (_length - 1) % 4 == 0)
                    {
                        for (var i = 1; i < _length; i += 4)
                        {
                            if (i != 1)
                                builder.Append("; ");

                            builder.Append(DataAsIpAddress(i));
                        }
                        return;
                    }
                }
                break;
            case DhcpServerOptionIdTypes.StatusCode:
                if (_length > 0)
                {
                    var status = DataAsByte(0);
                    if (status <= 4)
                    {
                        switch (status)
                        {
                            case 0:
                                builder.Append("Success");
                                break;
                            case 1:
                                builder.Append("Failure");
                                break;
                            case 2:
                                builder.Append("Query Terminated");
                                break;
                            case 3:
                                builder.Append("Malformed Query");
                                break;
                            case 4:
                                builder.Append("Not Allowed");
                                break;
                        }
                        if (_length > 1)
                        {
                            builder.Append(": ");
                            builder.Append(DataAsUtf8String(1, _length - 1));
                        }
                        return;
                    }
                }
                break;
            case DhcpServerOptionIdTypes.DhcpState:
                if (_length == 1)
                {
                    var state = DataAsByte(0);
                    if (state != 0 && state <= 8)
                    {
                        switch (state)
                        {
                            case 1:
                                builder.Append($"Available [1]");
                                break;
                            case 2:
                                builder.Append($"Active [2]");
                                break;
                            case 3:
                                builder.Append($"Expired [3]");
                                break;
                            case 4:
                                builder.Append($"Released [4]");
                                break;
                            case 5:
                                builder.Append($"Abandoned [5]");
                                break;
                            case 6:
                                builder.Append($"Reset [6]");
                                break;
                            case 7:
                                builder.Append($"Remote [7]");
                                break;
                            case 8:
                                builder.Append($"Transitioning [8]");
                                break;
                        }
                        return;
                    }
                }
                break;
            case DhcpServerOptionIdTypes.Hex: // default
            default:
                isExpected = true;
                break;
        }

        if (!isExpected)
            builder.Append("unexpected value: ");

        builder.Append(DataAsHexString(' '));
        builder.Append(" [length: ");
        builder.Append(_length);
        builder.Append("]");
    }

    internal static DhcpServerPacketOption Parse(byte[] buffer, ref int index)
    {
        var id = (DhcpServerOptionIds)buffer[index++];

        switch (id)
        {
            case DhcpServerOptionIds.Pad:
            case DhcpServerOptionIds.End:
                // 0-byte fixed length
                return new DhcpServerPacketOption(id);
            default:
                // variable length
                if (index >= buffer.Length)
                    return new DhcpServerPacketOption(DhcpServerOptionIds.End);

                var dataLength = buffer[index++];

                if (dataLength <= 8)
                {
                    var bufferL = default(ulong);
                    for (var i = 0; i < dataLength; i++)
                    {
                        bufferL |= ((ulong)buffer[index + i] << (56 - (i * 8)));
                    }
                    index += dataLength;
                    return new DhcpServerPacketOption(id, bufferL, dataLength);
                }
                else
                {
                    var bufferA = new byte[dataLength];
                    Array.Copy(buffer, index, bufferA, 0, Math.Min(dataLength, buffer.Length - index));
                    index += dataLength;
                    return new DhcpServerPacketOption(id, bufferA);
                }
        }
    }

    internal static ReadOnlyCollection<DhcpServerPacketOption> ParseAll(byte[] buffer, int index, int length)
    {
        if (buffer.Length < length)
            length = buffer.Length;

        var results = new List<DhcpServerPacketOption>();

        for (var i = index; i < length;)
        {
            var option = Parse(buffer, ref i);

            if (option.Id == DhcpServerOptionIds.End)
                break;

            if (option.Id != DhcpServerOptionIds.Pad)
                results.Add(option);
        }

        return results.AsReadOnly();
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.Append(_id.ToString());
        sb.Append("[");
        sb.Append((byte)_id);
        sb.Append("]: ");

        DataAsFormatted(sb);

        return sb.ToString();
    }
}