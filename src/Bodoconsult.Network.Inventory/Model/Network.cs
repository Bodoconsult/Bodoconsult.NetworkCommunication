using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Bodoconsult.Inventory.Model;

public delegate void UiStatusMessage(string modul, string msg);

public delegate void MailStatusMessage(string msg);

public class Network
{

    /// <summary>
    /// default ctor
    /// </summary>
    public Network()
    {
        NetworkItems = new ConcurrentBag<NetworkItem>();
        Domain = new DomainItem();
        UnknownHosts = new List<string>();
        VirtualInfrastructure = new VirtualInfrastructure();
    }

        
    /// <summary>
    /// Domain related data
    /// </summary>
    public DomainItem Domain { get; set; }      

    /// <summary>
    /// All the network item like computers, laptops, servers etc.
    /// </summary>
    public ConcurrentBag<NetworkItem> NetworkItems { get; set; }

        

       
    /// <summary>
    /// Unknown hosts: OS not Windows
    /// </summary>
    public List<string> UnknownHosts { get; set; }


    /// <summary>
    /// Virtual infrastruktur  of the network
    /// </summary>
    public VirtualInfrastructure VirtualInfrastructure { get; set; }

    //private string _test = "";

     
}