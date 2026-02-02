using System.Runtime.Serialization;

namespace Bodoconsult.Inventory.Model;

/// <summary>
/// A role a network item plays in the network, i.e domain controller
/// </summary>
[DataContract(Namespace = "http://bodoconsult/inventory")]
public class RoleItem
{
    /// <summary>
    /// Name of a role a network item plays in the network, i.e domain controller
    /// </summary>
    [DataMember]
    public string Name { get; set; }

}