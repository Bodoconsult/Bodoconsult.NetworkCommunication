// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


namespace Bodoconsult.Network.Windows.Dhcp.Native;

internal static class Constants
{

    public const uint DhcpFlagsOptionIsVendor = 0x03;

    public const uint DhcpEndpointFlagCantModify = 0x01;

    public const uint DnsFlagEnabled = 0x01;
    public const uint DnsFlagUpdateDownlevel = 0x02;
    public const uint DnsFlagCleanupExpired = 0x04;
    public const uint DnsFlagUpdateBothAlways = 0x10;
    public const uint DnsFlagUpdateDhcid = 0x20;
    public const uint DnsFlagDisablePtrUpdate = 0x40;

    /// <summary>
    /// The DHCPv4 client has been offered this IPv4 address.
    /// </summary>
    public const byte AddressStateOffered = 0x00;
    /// <summary>
    /// The IPv4 address is active and has an active DHCPv4 client lease record.
    /// </summary>
    public const byte AddressStateActive = 0x01;
    /// <summary>
    /// The IPv4 address request was declined by the DHCPv4 client; hence, it is a bad IPv4 address.
    /// </summary>
    public const byte AddressStateDeclined = 0x02;
    /// <summary>
    /// The IPv4 address is in DOOMED state and is due to be deleted.
    /// </summary>
    public const byte AddressStateDoom = 0x03;

    /// <summary>
    /// The address is leased to the DHCPv4 client without DHCID (sections 3 and 3.5 of [RFC4701]).
    /// </summary>
    public const byte AddressBitNoDhcid = 0x00;
    /// <summary>
    /// The address is leased to the DHCPv4 client with DHCID as specified in section 3.5.3 of [RFC4701].
    /// </summary>
    public const byte AddressBitDhcidNoClientidoption = 0x01;
    /// <summary>
    /// The address is leased to the DHCPv4 client with DHCID as specified in section 3.5.2 of [RFC4701].
    /// </summary>
    public const byte AddressBitDhcidWithClientidoption = 0x02;
    /// <summary>
    /// The address is leased to the DHCPv4 client with DHCID as specified in section 3.5.1 of [RFC4701].
    /// </summary>
    public const byte AddressBitDhcidWithDuid = 0x03;

    /// <summary>
    /// The DNS update for the DHCPv4 client lease record needs to be deleted from the DNS server when the lease is deleted.
    /// </summary>
    public const byte AddressBitCleanup = 0x10;
    /// <summary>
    /// The DNS update needs to be sent for both A and PTR resource records ([RFC1034] section 3.6).
    /// </summary>
    public const byte AddressBitBothRec = 0x20;
    /// <summary>
    /// The DNS update is not completed for the lease record.
    /// </summary>
    public const byte AddressBitUnregistered = 0x40;
    /// <summary>
    /// The address lease is expired, but the DNS updates for the lease record have not been deleted from the DNS server.
    /// </summary>
    public const byte AddressBitDeleted = 0x80;
}
