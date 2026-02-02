// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


namespace Bodoconsult.Network.Windows.Dns;

/// <summary>
/// Different types of DNS zone in MS DNS Server
/// </summary>
/// <remarks>For creation of new zones the list is different</remarks>
public enum NewZoneType
{
    Primary,
    Secondary,
    /// <remarks>Server 2003+ only</remarks>
    Stub,
    /// <remarks>Server 2003+ only</remarks>
    Forwarder
}