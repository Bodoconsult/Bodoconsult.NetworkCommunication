// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


namespace Bodoconsult.Network.Windows.Dhcp.Native;

/// <summary>
/// The DHCP_OPTION_SCOPE_TYPE enumeration defines the set of possible DHCP option scopes.
/// </summary>
internal enum DhcpOptionScopeType : uint
{
    /// <summary>
    /// The DHCP options correspond to the default scope.
    /// </summary>
    [Obsolete]
    DhcpDefaultOptions,
    /// <summary>
    /// The DHCP options correspond to the global scope.
    /// </summary>
    DhcpGlobalOptions,
    /// <summary>
    /// The DHCP options correspond to a specific subnet scope.
    /// </summary>
    DhcpSubnetOptions,
    /// <summary>
    /// The DHCP options correspond to a reserved IP address.
    /// </summary>
    DhcpReservedOptions,
    /// <summary>
    /// The DHCP options correspond to a multicast scope.
    /// </summary>
    DhcpMScopeOptions
}
