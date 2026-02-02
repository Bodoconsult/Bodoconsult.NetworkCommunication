// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


namespace Bodoconsult.Network.Windows.Dhcp.Native;

/// <summary>
/// The QuarantineStatus enumeration specifies possible health status values for the DHCPv4 client, as validated at the NAP server. 
/// </summary>
internal enum QuarantineStatus
{
    /// <summary>
    /// The DHCP client is compliant with the health policies defined by the administrator and has normal access to the network.
    /// </summary>
    Noquarantine = 0,
    /// <summary>
    /// The DHCP client is not compliant with the health policies defined by the administrator and is being quarantined with restricted access to the network.
    /// </summary>
    Restrictedaccess = 1,
    /// <summary>
    /// The DHCP client is not compliant with the health policies defined by the administrator and is being denied access to the network. The DHCP server does not grant an IP address lease to this client.
    /// </summary>
    Droppacket = 2,
    /// <summary>
    /// The DHCP client is not compliant with the health policies defined by the administrator and is being granted normal access to the network for a limited time.
    /// </summary>
    Probation = 3,
    /// <summary>
    /// The DHCP client is exempt from compliance with the health policies defined by the administrator and is granted normal access to the network.
    /// </summary>
    Exempt = 4,
    /// <summary>
    /// The DHCP client is put into the default quarantine state configured on the DHCP NAP server. When a network policy server (NPS) is unavailable, the DHCP client can be put in any of the states NOQUARANTINE, RESTRICTEDACCESS, or DROPPACKET, depending on the default setting on the DHCP NAP server.
    /// </summary>
    Defaultquarsetting = 5,
    /// <summary>
    /// No quarantine.
    /// </summary>
    Noquarinfo = 6,
}
