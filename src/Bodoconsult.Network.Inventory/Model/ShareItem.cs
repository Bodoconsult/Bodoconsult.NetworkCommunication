using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Bodoconsult.Inventory.Model;

[DataContract(Namespace = "http://bodoconsult/inventory")] 
public class ShareItem
{
    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string Description { get; set; }

    [DataMember]
    public string Path { get; set; }

    [DataMember]
    public string Type { get; set; }

    [DataMember]
    public List<AccessControl> AccessControlList { get; set; }

    public ShareItem()
    {
        AccessControlList = new List<AccessControl>();
    }

}