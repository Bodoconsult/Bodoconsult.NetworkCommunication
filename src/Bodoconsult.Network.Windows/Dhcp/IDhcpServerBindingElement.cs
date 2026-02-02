// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


namespace Bodoconsult.Network.Windows.Dhcp;

public interface IDhcpServerBindingElement
{
    DhcpServerIpAddress AdapterPrimaryIpAddress { get; }
    DhcpServerIpMask AdapterSubnetAddress { get; }
    bool CantModify { get; }
    string InterfaceDescription { get; }
    Guid InterfaceGuidId { get; }
    byte[] InterfaceId { get; }
    bool IsBound { get; }
    IDhcpServer Server { get; }
}