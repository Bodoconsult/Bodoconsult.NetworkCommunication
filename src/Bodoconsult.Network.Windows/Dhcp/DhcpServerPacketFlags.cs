// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


namespace Bodoconsult.Network.Windows.Dhcp;

[Flags]
public enum DhcpServerPacketFlags : ushort
{
    Broadcast = 0x8000
}