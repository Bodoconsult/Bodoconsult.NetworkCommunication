using System.Runtime.Serialization;

namespace Bodoconsult.Inventory.Model;

[DataContract(Namespace = "http://bodoconsult/inventory")] 
public class NetworkAdapterItem
{
    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public string Type { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public long Speed { get; set; }

    [DataMember]
    public string MacAddress { get; set; }

    [DataMember]
    public string Remark { get; set; }

    [DataMember]
    public string IpAddress { get; set; }

    [DataMember]
    public string DnsServer { get; set; }

    [DataMember]
    public bool DhcpEnabled { get; set; }

    [DataMember]
    public string DefaultIpGateway { get; set; }

        

}