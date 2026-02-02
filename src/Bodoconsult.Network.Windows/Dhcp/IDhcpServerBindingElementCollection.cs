// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


namespace Bodoconsult.Network.Windows.Dhcp;

public interface IDhcpServerBindingElementCollection : IEnumerable<IDhcpServerBindingElement>
{
    IDhcpServer Server { get; }
}