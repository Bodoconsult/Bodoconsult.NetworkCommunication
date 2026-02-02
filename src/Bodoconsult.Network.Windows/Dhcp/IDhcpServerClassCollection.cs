// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


namespace Bodoconsult.Network.Windows.Dhcp;

public interface IDhcpServerClassCollection : IEnumerable<IDhcpServerClass>
{
    IDhcpServer Server { get; }

    IDhcpServerClass GetClass(string name);
}