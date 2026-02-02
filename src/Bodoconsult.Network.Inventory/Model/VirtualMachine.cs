using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Bodoconsult.Inventory.Model;

/// <summary>
/// Represents a virtual machine on a virtualization host
/// </summary>
[DataContract(Namespace = "http://bodoconsult/inventory")]
public class VirtualMachine
{

    /// <summary>
    /// Name of the virtual machine
    /// </summary>
    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string Ip { get; set; }

    [DataMember]
    [JsonIgnore]
    public string IpDisplay
    {
        get => Ip.Replace('.', '_');
        // ReSharper disable once ValueParameterNotUsed
        set { 
            // Do nothing
        }
    }

    [DataMember]
    public string GuestOs { get; set; }

    [DataMember]
    public string HostName { get; set; }

    [JsonIgnore]
    public VirtualHost Parent { get; set; }

}