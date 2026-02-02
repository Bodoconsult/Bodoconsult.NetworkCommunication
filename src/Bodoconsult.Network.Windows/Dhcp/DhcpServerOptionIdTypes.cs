// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


namespace Bodoconsult.Network.Windows.Dhcp;

public enum DhcpServerOptionIdTypes : byte
{
    Hex = 0,
    Pad,
    End,
    IpAddress,
    IpAddressList,
    Byte,
    Int16,
    UInt16,
    Int32,
    UInt32,
    AsciiString,
    Utf8String,
    IpAddressAndSubnet,
    IpAddressAndIpAddress,
    UInt16List,
    DhcpMessageType,
    DhcpParameterRequestList,
    ZeroLengthFlag,
    ClientFqdn,
    DnsName,
    DnsNameList,
    ClientUuid,
    SipServers,
    StatusCode,
    DhcpState
}