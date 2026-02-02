using System.Runtime.Serialization;

namespace Bodoconsult.Inventory.Model;

[DataContract(Namespace = "http://bodoconsult/inventory")]
public class GroupAccount
{

    [DataMember]
    public string TypeOfAccount { get; set; }


    [DataMember]
    public string Name { get; set; }


}