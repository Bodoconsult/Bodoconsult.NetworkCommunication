using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Bodoconsult.Inventory.Model;

[DataContract(Namespace = "http://bodoconsult/inventory")] 
public class AllShares
{
    [DataMember]
    public IEnumerable<ShareItem> Shares { get; set; }
}