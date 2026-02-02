using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Bodoconsult.Inventory.Model;

[DataContract(Namespace = "http://bodoconsult/inventory")] 
public class VirtualInfrastructure
{
    [DataMember]
    public List<VirtualHost> VirtualHosts { get; } = new List<VirtualHost>();


    public VirtualHost AddOrUpdateVirtualHost(string ip)
    {
        if (VirtualHosts.Any())
        {
            return null;
        }

        var item = VirtualHosts.FirstOrDefault(x => x.Url == ip);

        if (item != null)
        {
            return item;
        }

        item = new VirtualHost {Url = ip};
        VirtualHosts.Add(item);
        return item;
    }

}