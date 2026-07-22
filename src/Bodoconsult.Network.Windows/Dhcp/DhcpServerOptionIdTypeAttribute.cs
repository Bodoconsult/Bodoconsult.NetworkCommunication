// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


namespace Bodoconsult.Network.Windows.Dhcp;

[AttributeUsage(AttributeTargets.Field)]
public class DhcpServerOptionIdTypeAttribute : Attribute
{
    public DhcpServerOptionIdTypes Type;

    public DhcpServerOptionIdTypeAttribute(DhcpServerOptionIdTypes type)
    {
        Type = type;
    }
}