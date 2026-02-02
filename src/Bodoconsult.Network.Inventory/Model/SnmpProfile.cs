using System.Collections.Generic;

namespace Bodoconsult.Inventory.Model;

/// <summary>
/// SNMP profile: used for checking of received OID data from a SNMP device
/// </summary>
public class SnmpProfile
{

    public SnmpProfile()
    {
        ProfileItems = new List<SnmpProfileItem>();
    }

    /// <summary>
    /// Name of the SNMP profile
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Profile items
    /// </summary>
    public IList<SnmpProfileItem> ProfileItems { get; set; }
}