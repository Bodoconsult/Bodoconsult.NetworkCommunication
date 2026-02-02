using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Bodoconsult.Inventory.Model;

[DataContract(Namespace = "http://bodoconsult/inventory")]
public class GroupItem
{

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string DistinguishedName { get; set; }

    [DataMember]
    public List<string> Users { get; set; }

    [DataMember]
    public List<string> MemberOfGroups { get; set; }


    [DataMember]
    public List<string> GroupMembers { get; set; }

    public GroupItem()
    {
        Users = new List<string>();
        MemberOfGroups = new List<string>();
        GroupMembers = new List<string>();
    }

}