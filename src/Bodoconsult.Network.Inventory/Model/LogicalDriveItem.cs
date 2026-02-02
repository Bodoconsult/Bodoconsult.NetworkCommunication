using System.Runtime.Serialization;

namespace Bodoconsult.Inventory.Model;

[DataContract(Namespace = "http://bodoconsult/inventory")] 
public class LogicalDriveItem
{
    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public long Size { get; set; }

    [DataMember]
    public long FreeSpace { get; set; }

    [DataMember]
    public int Type { get; set; }

    [DataMember]
    public string FileSystem { get; set; }

    [DataMember]
    public string Remark { get; set; }

}