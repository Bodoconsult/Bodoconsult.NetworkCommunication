using System.Linq;
using System.Threading.Tasks;
using Bodoconsult.Inventory.Enums;
using Bodoconsult.Inventory.Model;

namespace Bodoconsult.Inventory.Handler;

/// <summary>
///  Gets the data for complete inventory of all network computers
/// </summary>
public class InventoryHandler
{
    /// <summary>
    /// handles collecting of network data
    /// </summary>
    private NetworkHandler NetworkHandler { get; set; }


    /// <summary>
    /// Network data
    /// </summary>
    public Network Network
    {
        get { return NetworkHandler.Network; }
    }


    /// <summary>
    /// default ctor
    /// </summary>
    public InventoryHandler(GeneralSettings settings)
    {
        CurrentSettings = settings;
        NetworkHandler = new NetworkHandler(settings);
        NetworkHandler.LoadAdditionalItems();
    }


    /// <summary>
    /// General settings for the network handler
    /// </summary>
    public GeneralSettings CurrentSettings { get; private set; }


    /// <summary>
    /// Get domain data
    /// </summary>
    public void GetDomainData()
    {
        CheckXmlFiles();
        NetworkHandler.WmiPath = "\\\\{0}\\root\\cimv2";

        // Get basic domain data
        NetworkHandler.GetBasicDomainData();

        // Get full domain data
        NetworkHandler.GetFullDomainData();
    }


    /// <summary>
    /// Find all reachable network items
    /// </summary>
    public void FindAllNetworkItems()
    {

        // Get computer accounts from domain
        NetworkHandler.GetNetworkItemsFromDomain();

        // find all IP addresses in the network
        NetworkHandler.SearchNetworkItemsByIpAddress(CurrentSettings.IpRanges);

        // Get VM data
        NetworkHandler.GetVirtualInfrastructure();
        NetworkHandler.SaveInfrastructure();

    }

    /// <summary>
    /// Xml von nicht erreichbaren PCs nach 60 Tagen löschen
    /// </summary>
    private void CheckXmlFiles()
    {

        //GotStatus("CheckXmlFiles", "Clear xml files older than 60 days");

        //var dir = new DirectoryInfo(XmlTarget);

        //var date = DateTime.Now.AddDays(-60);

        //foreach (var file in dir.GetFiles("*.xml").Where(file => file.LastWriteTime < date))
        //{
        //    try
        //    {
        //        file.Delete();
        //    }
        //    // ReSharper disable EmptyGeneralCatchClause
        //    catch
        //    // ReSharper restore EmptyGeneralCatchClause
        //    {


        //    }
        //}
    }



    protected void SendMail(string msg)
    {
        //_IsStart = false;
        var x = CurrentSettings.MailStatusMessage;
        if (x != null) x(msg);
    }

    /// <summary>
    /// Get data from all network items found in the network
    /// </summary>
    public void GetDataForNetworkItems()
    {
        //NetworkHandler.Network.Domain.Name = Domain;

        //Parallel.ForEach(Network.NetworkItems, GetDataFromNetworkItem);

        // Fetch WMI data
        Task.WaitAll(NetworkHandler.Network.NetworkItems
            .Where(x => x.ItemType == NetworkItemType.Windows)
            .OrderBy(z => z.IpAddresses[0])
            .Select(z => Task.Factory.StartNew(() => NetworkHandler.GetWmiDataFromNetworkItem(z))).ToArray());

        //foreach (var item in NetworkHandler.Network.NetworkItems.
        //    Where(x => x.ItemType == NetworkItemType.Windows).OrderBy(z => z.IpAddresses[0]))
        //{
        //    NetworkHandler.GetWmiDataFromNetworkItem(item);
        //}

        // Fetch SNMP data
        NetworkHandler.GetSnmpRequests();
        NetworkHandler.GetSnmpRequestData();

        // User permissions
        NetworkHandler.GetDirectUserPermissions();

        // Check hostnames
        NetworkHandler.CheckHostNames();

        // Get server roles
        NetworkHandler.GetServerRoles();

        // Get warnings
        NetworkHandler.CheckWarnings();
    }


    protected void GotStatus(string modul, string msg)
    {
        //_IsStart = false;
        var x = CurrentSettings.StatusMessage;
        if (x != null) x(modul, msg);
    }


    //private void GetContentFromXml(NetworkItem item)
    //{
    //    var file = System.IO.Path.Combine(Path, item.DocFile);

    //    if (!File.Exists(file)) return;

    //    var content = new ContentHandler
    //        {
    //            NetworkItem = item, 
    //            HtmlTargetDir = HtmlTargetDir
    //        };
    //    content.LoadFile(file);
    //    content.GetOperatingSystem();
    //    content.GetSoftware();
    //    content.GetNetworkAdapters();
    //    content.GetDrives();
    //    content.GetRam();
    //    content.GetLogicalDrives();
    //    content.SaveAsHtml(Path);
    //}





    //public void Check()
    //{


    //    var oConn = new ConnectionOptions
    //        {
    //            //Authority = "NTLMDOMAIN:Scheiner",
    //            Username = @"Administrator",
    //            Password = "150amu169"
    //        };

    //    var oMs = new ManagementScope("\\\\192.168.185.2\\root\\cimv2", oConn);

    //    oMs.Connect();

    //    //get Fixed disk stats
    //    var oQuery = new ObjectQuery("select FreeSpace,Size,Name from Win32_LogicalDisk where DriveType=3");

    //    //Execute the query 
    //    var oSearcher = new ManagementObjectSearcher(oMs, oQuery);

    //    //Get the results
    //    var oReturnCollection = oSearcher.Get();

    //    //loop through found drives and write out info
    //    foreach (var o in oReturnCollection)
    //    {
    //        var oReturn = (ManagementObject) o;
    //        // Disk name
    //        Console.WriteLine("Name : " + oReturn["Name"]);
    //        // Free Space in bytes
    //        Console.WriteLine("FreeSpace: " + oReturn["FreeSpace"]);
    //        // Size in bytes
    //        Console.WriteLine("Size: " + oReturn["Size"]);
    //    }
    //}

    ///// <summary>
    ///// Append network item to network by IP address
    ///// </summary>
    ///// <param name="ipAddress">IP address of the network item to append</param>
    //public void AddIp(string ipAddress)
    //{
    //    NetworkHandler.AddIp(ipAddress);

    //}


    public void SaveNetwork()
    {
        NetworkHandler.SaveNetwork();
    }
}