//using System;
//using System.Linq;
//using System.Text;
//using Bodoconsult.Inventory.Enums;
//using Bodoconsult.Inventory.Handler;
//using Bodoconsult.Inventory.Helper;
//using Bodoconsult.Inventory.Model;

//namespace Bodoconsult.Inventory.Provider;

///// <summary>
///// Create a XML data file from SNMP data request. Requires UDP port 161 and 162 outbound opened on the computer this software is running on.
///// </summary>
//public class SnmpNetworkItemDataProvider
//{

//    public SnmpNetworkItemDataProvider(SnmpRequestHandler snmp)
//    {
//        _snmp = snmp;
//    }

//    private readonly SnmpRequestHandler _snmp;

        

//    internal XmlChecker Xmlc = new XmlChecker { DoNotDeleteNodeNames = "BoInv" };

//    internal static Encoding Encoder = Encoding.GetEncoding("ISO-8859-2");

//    /// <summary>
//    /// The SNMP request to fulfill
//    /// </summary>
//    public SnmpRequest SnmpRequest { get; set; }

//    /// <summary>
//    /// XML file name to save XML in
//    /// </summary>
//    public string XmlFileName { get; set; }

//    /// <summary>
//    /// Did an error happen?
//    /// </summary>
//    public bool Error { get; set; }


//    public void Start(string rootElement)
//    {

//        Error = false;
//        try
//        {
//            Xmlc.XmlFileName = XmlFileName;
//            Xmlc.Start(rootElement);
//        }
//        catch (Exception ex)
//        {
//            Error = true;

//            var msg = $"{ex.Message}";
//            throw new Exception(msg);

//        }
//    }

//    /// <summary>
//    /// Get metadata for the network item
//    /// </summary>
//    public void GetMetaData()
//    {

//        if (Error) return;

//        Xmlc.CheckNode(
//            "NetworkItemRoot/NetworkItemSection[Metadata]/NetworkItemSubSection[Current]/NetworkItemItem/NetworkItemValue[IpAddress]",
//            SnmpRequest.IpAddresses[0]);

//        var host = NetworkHelper.GetHostNameByIp(SnmpRequest.IpAddresses[0]);
//        if (!string.IsNullOrEmpty(host)) host = SnmpRequest.IpAddresses[0];

//        Xmlc.CheckNode(
//            "NetworkItemRoot/NetworkItemSection[Metadata]/NetworkItemSubSection[Current]/NetworkItemItem/NetworkItemValue[Hostname]",
//            host);

//        Xmlc.CheckNode(
//            "NetworkItemRoot/NetworkItemSection[Metadata]/NetworkItemSubSection[Current]/NetworkItemItem/NetworkItemValue[Date]",
//            DateTime.Now.ToString("dd.MM.yyyy"));

//        // Add IP addresses as network adapters
//        var i = 1;
//        foreach (var ip in SnmpRequest.IpAddresses)
//        {
//            Xmlc.CheckNode(
//                $"NetworkItemRoot/NetworkItemSection[Hardware]/NetworkItemSubSection[NetworkAdapters]/NetworkItemItem[Network adapter {i}]/" +
//                "NetworkItemSubValue[NetworkConfiguration]/NetworkItemSubItem[NetworkConfiguration 1]/NetworkItemSubItemValue[IPAddress]",
//                ip);

//            // /NetworkItemItem[attribute::name='Network adapter {0}']/NetworkItemSubValue[attribute::name='NetworkConfiguration']/NetworkItemSubItem[attribute::name='NetworkConfiguration 1']/NetworkItemSubItemValue[attribute::name='IPAddress']
//        }

//    }
//    /// <summary>
//    /// Fetch the data from the SNMP source
//    /// </summary>
//    public void GetSnmpData()
//    {
//        if (!string.IsNullOrEmpty(SnmpRequest.Username) && !string.IsNullOrEmpty(SnmpRequest.Password))
//        {
//            _snmp.RunRequest(SnmpRequest);
//        }
//    }



//    /// <summary>
//    /// Create Xml content from SNMP data
//    /// </summary>
//    public void CreateXmlFromSnmpData()
//    {

//        // Add roles to XML "ComputerSystem" "Computer system 1" "Roles"
//        var i = 1;
//        foreach (var role in SnmpRequest.Roles)
//        {
//            var path = "NetworkItemRoot/NetworkItemSection[Software]/NetworkItemSubSection[ComputerSystem]/" +
//                       $"NetworkItemItem[Computer system 1]/NetworkItemValue[Roles]/NetworkItemSubItem[SubItem {i}]";

//            Xmlc.CheckNode(path, role);

//            i++;
//        }


//        // Add SNMP results
//        foreach (var item in SnmpRequest.SnmpRequestItems.Where(x => !string.IsNullOrEmpty(x.Value)))
//        {
//            var path = "";

//            switch (item.SnmpRequestItemType)
//            {
//                case SnmpRequestItemType.Name:
//                    path = "NetworkItemRoot/NetworkItemSection[Software]/NetworkItemSubSection[ComputerSystem]/" +
//                           "NetworkItemItem[Computer system 1]/NetworkItemValue[Name]";
//                    break;
//                case SnmpRequestItemType.Caption:
//                    path = "NetworkItemRoot/NetworkItemSection[Software]/NetworkItemSubSection[ComputerSystem]/" +
//                           "NetworkItemItem[Computer system 1]/NetworkItemValue[Caption]";
//                    break;
//                case SnmpRequestItemType.Model:
//                    path = "NetworkItemRoot/NetworkItemSection[Software]/NetworkItemSubSection[ComputerSystem]/" +
//                           "NetworkItemItem[Computer system 1]/NetworkItemValue[Model]";
//                    break;
//                case SnmpRequestItemType.Manufacturer:
//                    path = "NetworkItemRoot/NetworkItemSection[Software]/NetworkItemSubSection[ComputerSystem]/" +
//                           "NetworkItemItem[Computer system 1]/NetworkItemValue[Manufacturer]";
//                    break;
//                case SnmpRequestItemType.SerialNumber:
//                    path = "NetworkItemRoot/NetworkItemSection[Software]/NetworkItemSubSection[OperatingSystem]/" +
//                           "NetworkItemItem[Operating system 1]/NetworkItemValue[SerialNumber]";
//                    break;
//                case SnmpRequestItemType.OperatingSystem:
//                    path = "NetworkItemRoot/NetworkItemSection[Software]/NetworkItemSubSection[OperatingSystem]/" +
//                           "NetworkItemItem[Operating system 1]/NetworkItemValue[Caption]";
//                    break;
//                case SnmpRequestItemType.DomainRole:
//                    path = "NetworkItemRoot/NetworkItemSection[Software]/NetworkItemSubSection[OperatingSystem]/" +
//                           "NetworkItemItem[Operating system 1]/NetworkItemValue[DomainRole]";
//                    break;
//                case SnmpRequestItemType.Hostname:
//                    path = "NetworkItemRoot/NetworkItemSection[Metadata]/NetworkItemSubSection[Current]"+
//                           "/NetworkItemItem/NetworkItemValue[Hostname]";
//                    break;
//                case SnmpRequestItemType.TemperaturStatus:
//                    break;
//                case SnmpRequestItemType.SystemStatus:
//                    break;
//                default:
//                    throw new ArgumentOutOfRangeException();
//            }

//            if (!string.IsNullOrEmpty(path)) Xmlc.CheckNode(path, item.Value);
//        }

//        foreach (var result in SnmpRequest.Results.OrderBy(x => x.Oid))
//        {
//            var path = "NetworkItemRoot/NetworkItemSection[Snmp]/NetworkItemSubSection[Current]" +
//                       $"/NetworkItemItem/NetworkItemValue[{result.Oid}]";

//            Xmlc.CheckNode(path, "");


//            var subPath = path + "/NetworkItemSubItem[Value]";
//            Xmlc.CheckNode(subPath, result.Value);

//            subPath = path + "/NetworkItemSubItem[FullName]";
//            Xmlc.CheckNode(subPath, result.FullName);

//            subPath = path + "/NetworkItemSubItem[Description]";
//            Xmlc.CheckNode(subPath, result.Description);

//            //var data = string.Format("Value: {0} FullName: {1} Description: {2}", result.Value, result.FullName, result.Description.Replace("\"",""));

                
//        }
//    }


//    /// <summary>
//    /// Save XML as file
//    /// </summary>
//    public void Save()
//    {
//        if (Error) return;
//        //Xml.Save(Path);

//        Xmlc.Save();
//    }
//}