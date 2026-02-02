using System;
using System.Runtime.Serialization;

namespace Bodoconsult.Inventory.Model;

[DataContract(Namespace = "http://bodoconsult/inventory")]
public class DomainComputerItem
{

    //public DomainComputerItem()
    //{

    //}


    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string AdsPath { get; set; }

    [DataMember]
    public string OperatingSystem { get; set; }

    [DataMember]
    public string ServicePack { get; set; }


    [DataMember]
    public DateTime LastLogon { get; set; }
}