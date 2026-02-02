// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


namespace Bodoconsult.Network.Windows.Dhcp.Native;

/// <summary>
/// Flags used by DhcpV4FailoverSetRelationship
/// </summary>
[Flags]
internal enum DhcpFailoverRelationshipSetFlags
{
    /// <summary>
    /// The mclt member in pRelationship parameter structure is populated.
    /// </summary>
    Mclt = 0x00000001,
    /// <summary>
    /// The safePeriod member in pRelationship parameter structure is populated.
    /// </summary>
    Safeperiod = 0x00000002,
    /// <summary>
    /// The state member in pRelationship parameter structure is populated.
    /// </summary>
    Changestate = 0x00000004,
    /// <summary>
    /// The percentage member in pRelationship parameter structure is populated.
    /// </summary>
    Percentage = 0x00000008,
    /// <summary>
    /// The mode member in pRelationship parameter structure is populated.
    /// </summary>
    Mode = 0x00000010,
    /// <summary>
    /// The prevState member in pRelationship parameter structure is populated.
    /// </summary>
    Prevstate = 0x00000020,
}
