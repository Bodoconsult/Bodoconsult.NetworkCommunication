using System.Runtime.Serialization;

namespace Bodoconsult.Inventory.Model;

[DataContract(Namespace = "http://bodoconsult/inventory")] 
public class DriveItem
{
    [DataMember]
    public int DriveId { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public long Size { get; set; }

    [DataMember]
    public long SizeUsed { get; set; }

    [DataMember]
    public string Remark { get; set; }

}