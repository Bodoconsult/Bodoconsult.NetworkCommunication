// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


namespace Bodoconsult.Network.Windows.Dhcp;

public interface IDhcpServerOptionValue
{
    string ClassName { get; }
    IDhcpServerOption Option { get; }
    int OptionId { get; }
    IDhcpServer Server { get; }
    IEnumerable<IDhcpServerOptionElement> Values { get; }
    string VendorName { get; }
}