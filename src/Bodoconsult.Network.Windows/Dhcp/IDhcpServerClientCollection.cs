// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


namespace Bodoconsult.Network.Windows.Dhcp;

public interface IDhcpServerClientCollection : IEnumerable<IDhcpServerClient>
{
    IDhcpServer Server { get; }

    void RemoveClient(IDhcpServerClient client);
}