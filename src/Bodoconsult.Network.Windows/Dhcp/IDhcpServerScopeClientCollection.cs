// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


namespace Bodoconsult.Network.Windows.Dhcp;

public interface IDhcpServerScopeClientCollection : IEnumerable<IDhcpServerClient>
{
    IDhcpServerScope Scope { get; }
    IDhcpServer Server { get; }

    IDhcpServerClient AddClient(DhcpServerIpAddress address, DhcpServerHardwareAddress hardwareAddress);
    IDhcpServerClient AddClient(DhcpServerIpAddress address, DhcpServerHardwareAddress hardwareAddress, string name);
    IDhcpServerClient AddClient(DhcpServerIpAddress address, DhcpServerHardwareAddress hardwareAddress, string name, string comment);
    IDhcpServerClient AddClient(DhcpServerIpAddress address, DhcpServerHardwareAddress hardwareAddress, string name, string comment, DateTime leaseExpires);
    IDhcpServerClient AddClient(DhcpServerIpAddress address, DhcpServerHardwareAddress hardwareAddress, string name, string comment, DateTime leaseExpires, IDhcpServerHost ownerHost);
    IDhcpServerClient AddClient(DhcpServerIpAddress address, DhcpServerHardwareAddress hardwareAddress, string name, string comment, DateTime leaseExpires, IDhcpServerHost ownerHost, DhcpServerClientTypes clientType, DhcpServerClientAddressStates addressState, DhcpServerClientQuarantineStatuses quarantineStatus, DateTime probationEnds, bool quarantineCapable);
    void RemoveClient(IDhcpServerClient client);
}