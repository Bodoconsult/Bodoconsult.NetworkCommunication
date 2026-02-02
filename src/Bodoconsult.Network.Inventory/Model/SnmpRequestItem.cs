using Bodoconsult.Inventory.Enums;

namespace Bodoconsult.Inventory.Model;

/// <summary>
/// Represents data to query from a SNMP target
/// </summary>
public class SnmpRequestItem
{
    /// <summary>
    /// Item type to map from SNMP to WMI data
    /// </summary>
    public SnmpRequestItemType SnmpRequestItemType { get; set; }

    /// <summary>
    /// SNMP OID to fetch from target. If left empty, please set a <see cref="Value"/>. This may be used to set fixed values (no SNMP needed for)
    /// </summary>
    public string Oid { get; set; }

    /// <summary>
    /// Item value. Normally empty. If set with a valeu, leave <see cref="Oid"/>´empty. This may be used to set fixed values (no SNMP needed for)
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// Cleartext name of the request item. If <see cref="Oid"/> is used and a MIB exists the name derives from MIB.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Description of element. If <see cref="Oid"/> is used and a MIB exists the name derives from MIB.
    /// </summary>
    public string Description { get; set; }
}