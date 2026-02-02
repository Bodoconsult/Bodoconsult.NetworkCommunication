// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


namespace Bodoconsult.Network.Windows.Dhcp;

public interface IDhcpServerOptionElement : IEquatable<IDhcpServerOptionElement>
{
    DhcpServerOptionElementType Type { get; }
    object Value { get; }
    string ValueFormatted { get; }
}