using System.Runtime.Serialization;

namespace Bodoconsult.Inventory.Model;

[DataContract(Namespace = "http://bodoconsult/inventory")] 
public class SoftwareItem
{
    [DataMember]
    public string IdentifyingNumber { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string Vendor { get; set; }

    [DataMember]
    public string Version { get; set; }

    [DataMember]
    public string Remark { get; set; }

}