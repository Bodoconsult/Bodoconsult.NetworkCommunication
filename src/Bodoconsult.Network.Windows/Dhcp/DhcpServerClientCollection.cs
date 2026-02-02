// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


using System.Collections;

namespace Bodoconsult.Network.Windows.Dhcp;

public class DhcpServerClientCollection : IDhcpServerClientCollection
{
    public DhcpServer Server { get; }
    IDhcpServer IDhcpServerClientCollection.Server => Server;

    internal DhcpServerClientCollection(DhcpServer server)
    {
        Server = server;
    }

    public IEnumerator<IDhcpServerClient> GetEnumerator()
        => DhcpServerClient.GetClients(Server).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public void RemoveClient(IDhcpServerClient client)
        => client.Delete();
}