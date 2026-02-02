using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Bodoconsult.Inventory.Model;

/// <summary>
/// Keeps domain data
/// </summary>
[DataContract(Namespace = "http://bodoconsult/inventory")] 
public class DomainItem
{

    public DomainItem()
    {
        Users = new List<UserItem>();
        Groups = new List<GroupItem>();
        Computers = new List<DomainComputerItem>();
    }

    /// <summary>
    /// Name of the domain
    /// </summary>
    [DataMember]
    public string Name { get; set; }
    /// <summary>
    /// All user accounts in the domain
    /// </summary>
    [DataMember]
    public List<UserItem> Users { get; set; }

    /// <summary>
    /// All group accounts in the domain
    /// </summary>
    [DataMember]
    public List<GroupItem> Groups { get; set; }

    /// <summary>
    /// All computer accounts in the domain
    /// </summary>
    [DataMember]
    public List<DomainComputerItem> Computers { get; set; }


    /// <summary>
    /// Name of the currently used domain controller
    /// </summary>
    [DataMember]
    public string DomainControllerName { get; set; }

    /// <summary>
    /// DNS forest name
    /// </summary>
    [DataMember]
    public string DnsForestName { get; set; }

    /// <summary>
    /// Domain controller site name
    /// </summary>
    [DataMember]
    public string DomainControllerSiteName { get; set; }


    /// <summary>
    /// IP address of the domain controller
    /// </summary>
    [DataMember]
    public string DomainControllerIpAddress { get; set; }


}