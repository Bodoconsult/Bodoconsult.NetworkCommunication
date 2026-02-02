//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Management;
//using System.Net;
//using System.Net.NetworkInformation;
//using System.Text;
//using System.Threading.Tasks;
//using Bodoconsult.Inventory.Converter;
//using Bodoconsult.Inventory.Enums;
//using Bodoconsult.Inventory.Helper;
//using Bodoconsult.Inventory.Interfaces;
//using Bodoconsult.Inventory.Model;
//using Bodoconsult.Inventory.Provider;
//using Exception = System.Exception;

//namespace Bodoconsult.Inventory.Handler;

///// <summary>
///// Collects all the data needed for the <see cref="Network"/> object
///// </summary>
//public class NetworkHandler
//{
//    /// <summary>
//    /// default ctor
//    /// </summary>
//    public NetworkHandler(GeneralSettings settings)
//    {
//        CurrentSettings = settings;
//        Network = new Network();
//        SnmpRequests = new List<SnmpRequest>();

//        //if (File.Exists(NetworkJsonFileName)) Network = JsonHelper.LoadJsonFile<Network>(NetworkJsonFileName);
//    }

//    /// <summary>
//    /// General settings for the network handler
//    /// </summary>
//    public GeneralSettings CurrentSettings { get; private set; }


//    /// <summary>
//    /// Current WMI path to select
//    /// </summary>
//    public string WmiPath { get; set; }

//    /// <summary>
//    /// Snmp requests to fetch data for network items
//    /// </summary>
//    public IList<SnmpRequest> SnmpRequests { get; set; }


//    /// <summary>
//    /// The network object to fill with data
//    /// </summary>
//    public Network Network { get; set; }





//    /// <summary>
//    /// Load additional item from <see cref="GeneralSettings.JsonDir"/>
//    /// </summary>
//    public void LoadAdditionalItems()
//    {
//        try
//        {
//            var fileName = Path.Combine(FileHelper.JsonDir, "AdditionalItems.json");
//            if (!File.Exists(fileName)) return;

//            var data = JsonHelper.LoadJsonFile<List<AdditionalItem>>(fileName);

//            if (data.Count == 0) return;

//            foreach (var item in data)
//            {
//                var ni = AddOrUpdateNetworkItem(item.IpAddress);

//                ni.HyperVHost = item.HyperVHost;
//                ni.VmwareHost = item.VmwareHost;
//                ni.Password = item.GetPassword();
//                ni.Username = item.Username;
//                ni.ItemType = item.HyperVHost ? NetworkItemType.Windows : NetworkItemType.Others;

//                GotStatus("LoadAdditionalItems", $"Found item {ni.Ip}");
//            }
//        }
//        catch (Exception e)
//        {
//            System.Console.WriteLine(e);
//            throw;
//        }
//    }


//    /// <summary>
//    /// Get basic data from an Active Directory domain like domain name, PDC IP address and domain controllers
//    /// </summary>
//    public void GetBasicDomainData()
//    {
//        GotStatus("NetworkHandler", "Get basic domain data");

//        var dh = new DomainHelper();

//        Network.Domain.Name = dh.DomainName;
//        Network.Domain.DomainControllerIpAddress = dh.DomainControllerIpAddress;
//        Network.Domain.DomainControllerName = dh.DomainControllerName;

//        var dcs = dh.GetListOfDomainControllers();

//        foreach (var dc in dcs)
//        {
//            if (!CheckIp(dc.Ip)) Network.NetworkItems.Add(dc);
//        }
//    }


//    /// <summary>
//    /// Get full domain data via WMI from domain controller
//    /// </summary>
//    public void GetFullDomainData()
//    {
//        GotStatus("NetworkHandler", "Get full domain data via WMI");

//        var dwmi = new WmiCimDomainDataProvider
//        {
//            LocalIp = CurrentSettings.LocalIp,
//            Ip = Network.Domain.DomainControllerIpAddress,
//            HostName = Network.Domain.DomainControllerName,
//            XmlFileName = Path.Combine(CurrentSettings.XmlTargetDir, "result_domain.xml"),
//            Domain = Network.Domain.Name,
//            Username = CurrentSettings.Username,
//            Password = CurrentSettings.Password
//        };

//        dwmi.Start("NetworkItemRoot");
//        dwmi.GetMetaData();

//        GotStatus("NetworkHandler", "Get users from domain via WMI");
//        dwmi.GetUsers();

//        GotStatus("NetworkHandler", "Get groups from domain via WMI");
//        dwmi.GetGroups();

//        GotStatus("NetworkHandler", "Get user membership in groups from domain via WMI");
//        dwmi.GetComputers();
//        dwmi.Save();


//        GotStatus("NetworkHandler", "Convert domain data received by WMI to XML");

//        var wmiConverter = new WmiDomainDataConverter
//        {
//            Data = dwmi,
//            Domain = Network.Domain
//        };

//        GotStatus("NetworkHandler", "Get user data from XML");
//        wmiConverter.GetUsers();

//        GotStatus("NetworkHandler", "Get group data from XML");
//        wmiConverter.GetGroups();

//        GotStatus("NetworkHandler", "Get user membership in groups data from XML");
//        wmiConverter.GetUserGroups();

//        GotStatus("NetworkHandler", "Get computer accounts from XML");
//        wmiConverter.GetComputers();

//        GotStatus("NetworkHandler", "Domain data ready to use!");

//        //// Get network items from domain computers
//        //foreach (var adComputer in Network.Domain.Computers)
//        //{
//        //    // Network item
//        //    var computer = new NetworkItem
//        //    {
//        //        DnsHostName = adComputer.Name,
//        //        HostName = adComputer.Name
//        //    };
//        //    computer.IpAddresses.Add(NetworkHelper.GetIpAddress(adComputer.Name));

//        //    computer.ItemType = adComputer.OperatingSystem.ToLower().Contains("windows") ? NetworkItemType.Windows : NetworkItemType.Others;
//        //}
//    }

//    /// <summary>
//    /// Get network items by ping to IP address
//    /// </summary>
//    /// <param name="baseAddress">base adress with placeholder for IP address, i.e. 192.168.12.{0}</param>
//    public void SearchNetworkItemsByIpAddress(string baseAddress)
//    {

//        if (string.IsNullOrEmpty(baseAddress))
//        {
//            var ip = Network.Domain.DomainControllerIpAddress;
//            ip = ip.Substring(0, ip.LastIndexOf(".", StringComparison.Ordinal)) + ".{0}";

//            baseAddress = ip;
//        }

//        var ranges = baseAddress.Split(';');



//        if (CurrentSettings.ExcludeIp == null) CurrentSettings.ExcludeIp = "";

//        foreach (var range in ranges)
//        {

//            var r = range;

//            //for (var x = 1; x < 255; x++)
//            //{

//            Parallel.For(1, 255, x =>
//            {
//                var ip = string.Format(r, x);

//                PingCheck(ip);


//                //if (!string.IsNullOrEmpty(host))
//                //{
//                //    var ni = Network.NetworkItems.FirstOrDefault(h => h.HostName.ToLower() == host.ToLower() ||
//                //                                              h.HostName.ToLower() == host.ToLower() + "." + Network.Domain.Name.ToLower() ||
//                //                                              h.HostName.ToLower() == host.ToLower().Replace("." + Network.Domain.Name.ToLower(), ""));
//                //    if (ni != null)
//                //    {
//                //        if (ni.IpAddresses.IndexOf(ip) > -1) return;
//                //        if (!ip.Contains(":")) ni.IpAddresses.Add(ip);
//                //        // ReSharper disable once RedundantJumpStatement
//                //        return;
//                //    }

//                //    var item = new NetworkItem { HostName = host };
//                //    item.IpAddresses.Add(ip);
//                //    item.DetailFile = null;
//                //    item.SummaryFile = null;
//                //    Network.NetworkItems.Add(item);

//                //}
//                //else
//                //{
//                //    var item = new NetworkItem { HostName = ip };
//                //    item.IpAddresses.Add(ip);
//                //    item.DetailFile = null;
//                //    item.SummaryFile = null;
//                //    Network.NetworkItems.Add(item);
//                //}


//                //_test += host + "\r\n";

//                //var item = new NetworkItem { HostName = host ?? ip };
//                //item.IpAddresses.Add(ip);
//                //NetworkItems.Add(item);
//            });
//        }

//        // Check file paths for the items
//        foreach (var item in Network.NetworkItems)
//        {
//            var ip = item.IpAddresses[0].Replace(".", "_");
//            if (string.IsNullOrEmpty(item.XmlFile)) item.XmlFile = Path.Combine(CurrentSettings.XmlTargetDir,
//                $"result-{ip}.xml");
//            item.DetailFile = Path.Combine(CurrentSettings.HtmlTargetDir, $"{ip}.htm");
//            item.SummaryFile = Path.Combine(CurrentSettings.HtmlTargetDir, $"{ip}_summary.htm");
//        }
//    }

//    public void PingCheck(string ip)
//    {
//        if (CurrentSettings.ExcludeIp != null && CurrentSettings.ExcludeIp.Contains("{" + ip + "}"))
//        {
//            return;
//        }

//        GotStatus("Network.GetAllNetworkItems", $"Check IP {ip}...");

//        var reply = NetworkHelper.Ping(ip);
//        var status = reply == null ? IPStatus.DestinationHostUnreachable : reply.Status;

//        var host = NetworkHelper.GetHostNameByIp(ip);

//        var item = Network.NetworkItems.FirstOrDefault(x =>
//            x.IpAddresses.Contains(ip));

//        if (item == null && host != null)
//        {
//            item = Network.NetworkItems.FirstOrDefault(x =>
//                string.Equals(x.HostName, host, StringComparison.CurrentCultureIgnoreCase) ||
//                x.HostName.ToLower() == host.ToLower() + "." + Network.Domain.Name.ToLower() ||
//                x.HostName.ToLower() == host.ToLower().Replace("." + Network.Domain.Name.ToLower(), ""));
//        }

//        if (item == null && status == IPStatus.Success)
//        {
//            item = AddOrUpdateNetworkItem(ip);
//        }

//        if (item == null)
//        {
//            GotStatus("Network.GetAllNetworkItems", $"IP {ip} not found!");
//            return;
//        }

//        item.HostName = host;
//        if (string.IsNullOrEmpty(item.DnsHostName)) item.DnsHostName = item.HostName;
//        item.IsPingable = status == IPStatus.Success;

//        var dnsHost = item.DnsHostName ?? item.HostName;
//        var domain = string.IsNullOrEmpty(item.Username) ? Network.Domain.Name : dnsHost;
//        var username = string.IsNullOrEmpty(item.Username) ? CurrentSettings.Username : item.Username;
//        var password = string.IsNullOrEmpty(item.Password) ? CurrentSettings.Password : item.Password;


//        // Check CIM with priority
//        try
//        {

//            var session = WmiCimHelper.GetSession(domain, dnsHost, username, password);

//            var data = WmiCimHelper.ExecuteQuery(session, string.Format(WmiPath, dnsHost),
//                "select * from Win32_BaseBoard");

//            if (data == null)
//            {
//                item.IsCimObject = false;
//            }
//            else
//            {
//                item.ItemType = NetworkItemType.Windows;
//                item.IsCimObject = true;
//            }
//        }
//        catch //(Exception ex)
//        {
//            item.IsCimObject = false;
//        }

//        if (!item.IsCimObject) // Check WMI only if no CIM access
//        {

//            try
//            {
//                var options = WmiHelper.GetWmiConnectionOptions(ip, CurrentSettings.LocalIp, domain, username, password);

//                var wmiScope = new ManagementScope(string.Format(WmiPath, item.DnsHostName), options)
//                {
//                    Options = { Timeout = new TimeSpan(0, 0, 0, 0, 700) }
//                };
//                wmiScope.Connect();
//                item.ItemType = NetworkItemType.Windows;
//                item.IsWmiObject = true;
//            }
//            catch //(Exception ex)
//            {
//                item.IsWmiObject = false;
//            }
//        }

//        var xmlFile = NetworkItemHelper.GetXmlFilePath(CurrentSettings.XmlTargetDir, ip);

//        if (item.IsWmiObject || item.IsCimObject)
//        {
//            item.XmlFile = xmlFile;
//        }
//        else if (File.Exists(xmlFile))
//        {
//            item.XmlFile = xmlFile;
//        }
//        else
//        {
//            item.XmlFile = null;
//        }

//        GotStatus("Network.GetAllNetworkItems",
//            $"IP {ip} found! Ping; {item.IsPingable} CIM: {item.IsCimObject} WMI: {item.IsWmiObject}");

//    }

//    /// <summary>
//    /// Returns a new or existing network item by its IP address
//    /// </summary>
//    /// <param name="ipAddress"></param>
//    public NetworkItem AddOrUpdateNetworkItem(string ipAddress)
//    {

//        if (ipAddress.Contains(":")) return null;

//        var item = Network.NetworkItems.FirstOrDefault(x => x.IpAddresses.Any(y => y == ipAddress));

//        if (item != null) return item;

//        item = new NetworkItem
//        {
//            HostName = NetworkHelper.GetHostNameByIp(ipAddress),
//            ItemType = NetworkItemType.Windows
//        };
//        item.IpAddresses.Add(ipAddress);
//        Network.NetworkItems.Add(item);
//        return item;
//    }


//    /// <summary>
//    /// Returns a new or existing network item by its IP address
//    /// </summary>
//    /// <param name="request">SNMP request data</param>
//    public NetworkItem AddOrUpdateNetworkItem(SnmpRequest request)
//    {

//        if (request.IpAddresses[0].Contains(":")) return null;

//        var item = Network.NetworkItems.FirstOrDefault(x => x.IpAddresses.Any(y => y == request.IpAddresses[0]));

//        if (item == null)
//        {
//            item = new NetworkItem
//            {
//                //HostName = NetworkHelper.GetHostNameByIp(request.IpAddresses[0]),
//                ItemType = NetworkItemType.Snmp
//            };
//            item.IpAddresses.Add(request.IpAddresses[0]);
//            Network.NetworkItems.Add(item);
//        }



//        // Add roles
//        foreach (var role in request.Roles)
//        {
//            if (!item.Roles.Exists(x => x.Name == role)) item.Roles.Add(new RoleItem { Name = role });
//        }

//        // Add other information
//        foreach (var data in request.SnmpRequestItems)
//        {
//            switch (data.SnmpRequestItemType)
//            {
//                case SnmpRequestItemType.Name:
//                    break;
//                case SnmpRequestItemType.Caption:
//                    break;
//                case SnmpRequestItemType.Model:
//                    item.Model = data.Value;
//                    break;
//                case SnmpRequestItemType.Manufacturer:
//                    item.Manufacturer = data.Value;
//                    break;
//                case SnmpRequestItemType.SerialNumber:
//                    item.SerialNumber = data.Value;
//                    break;
//                case SnmpRequestItemType.TemperaturStatus:
//                    break;
//                case SnmpRequestItemType.SystemStatus:
//                    break;
//                case SnmpRequestItemType.Hostname:
//                    item.HostName = data.Value;
//                    break;
//                case SnmpRequestItemType.OperatingSystem:
//                    item.OperatingSystem = data.Value;
//                    break;
//                case SnmpRequestItemType.DomainRole:
//                    item.DomainRole = DomainHelper.TranslateDomainRole(data.Value);
//                    break;
//                default:
//                    throw new ArgumentOutOfRangeException();
//            }
//        }

//        return item;
//    }


//    /// <summary>
//    /// Check if IP already exists
//    /// </summary>
//    /// <param name="ipAddress"></param>
//    public bool CheckIp(string ipAddress)
//    {

//        if (ipAddress.Contains(":")) return true;

//        var item = Network.NetworkItems.FirstOrDefault(x => x.IpAddresses.Any(y => y == ipAddress));

//        return item != null;
//    }


//    /// <summary>
//    /// Get computer accounts from
//    /// </summary>
//    public void GetNetworkItemsFromDomain()
//    {
//        //WmiCimHelper.GetSession(Network.Domain.Name, "BCGS02DC.BCG-AD.DE", Username, Password);
//        var session = WmiCimHelper.GetSession(Network.Domain.Name, Network.Domain.DomainControllerName, 
//            CurrentSettings.Username, 
//            CurrentSettings.Password);

//        var allComputers = WmiCimHelper.ExecuteQuery(session,
//            $"\\\\{Network.Domain.DomainControllerName}\\root\\directory\\LDAP", "select * from ds_computer").ToList();

//        foreach (var computer in allComputers)
//        {

//            try
//            {
//                var dnsHostName = computer.CimInstanceProperties["DS_dNSHostName"].Value.ToString();

//                GotStatus("Network.GetItemsFromDomain", $"Check host {dnsHostName}");

//                var hostName = computer.CimInstanceProperties["DS_cn"].Value.ToString().Replace(Network.Domain.Name, "");

//                IPHostEntry ipEntry;

//                try
//                {
//                    ipEntry = Dns.GetHostEntry(dnsHostName);
//                }
//                catch
//                {
//                    Network.UnknownHosts.Add(dnsHostName);
//                    continue;
//                }


//                //if (NetworkItems.Any(h => h.HostName.ToLower() == hostName.ToLower() ||
//                //                          h.HostName.ToLower() == hostName.ToLower() + "." + DomainName.ToLower() ||
//                //                          h.HostName.ToLower() == hostName.ToLower().Replace("." + DomainName.ToLower(), "")))

//                //    continue;


//                var addr = ipEntry.AddressList;

//                //var pc = new NetworkItem { DnsHostName = dnsHostName, HostName = hostName };

//                NetworkItem pc = null;

//                foreach (
//                    var ip in
//                    addr.Where(
//                        ip =>
//                            !ip.IsIPv6LinkLocal && !ip.IsIPv6Multicast && !ip.IsIPv6SiteLocal &&
//                            !ip.IsIPv6Teredo))
//                {
//                    if (!ip.ToString().Contains(":")) pc = AddOrUpdateNetworkItem(ip.ToString());
//                }

//                if (pc == null)
//                {
//                    pc = new NetworkItem
//                    {
//                        ItemType = NetworkItemType.Windows,
//                    };
//                }
//                pc.DnsHostName = dnsHostName;
//                pc.HostName = hostName;
//                pc.IsAdObject = true;


//                var osNode = computer.CimInstanceProperties["ds_OperatingSystem"].Value;
//                if (osNode != null)
//                {
//                    var osName = osNode.ToString();
//                    if (!string.IsNullOrEmpty(osName)) pc.ItemType = osName.ToLower().Contains("windows") ? NetworkItemType.Windows : NetworkItemType.Others;
//                    if (string.IsNullOrEmpty(pc.OperatingSystem) || pc.OperatingSystem == "unknown") pc.OperatingSystem = osName;
//                }

//                //_test += hostName + "\r\n";


//                //foreach (var item in addr.Where(t => !t.IsIPv6LinkLocal && !t.IsIPv6Multicast && !t.IsIPv6SiteLocal && !t.IsIPv6Teredo).Select(t => new NetworkItem { DnsHostName = dnsHostName, AddIp = t.ToString(), HostName = hostName }))
//                //{
//                //    NetworkItems.Add(item);
//                //}
//            }
//            // ReSharper disable EmptyGeneralCatchClause
//            catch (Exception ex)
//                // ReSharper restore EmptyGeneralCatchClause
//            {
//                Debug.Print(ex.Message);
//            }

//        }
//    }

//    /// <summary>
//    /// Get the virtual infrastructure
//    /// </summary>
//    public void GetVirtualInfrastructure()
//    {
//        GotStatus("GetVirtualInfrastructure", "Get virtualization infrastructure");

//        // HyperV virtual hosts
//        GotStatus("GetHyperVInfrastructure", "Get HyperV infrastructure");
//        foreach (var item in Network.NetworkItems.Where(x => x.HyperVHost))
//        {
//            GetHypervInfrastructure(item);
//        }

//        // VMWare virtual hosts
//        GotStatus("GetVmwareInfrastructure", "Get VMWare infrastructure");
//        foreach (var item in Network.NetworkItems.Where(x => x.VmwareHost))
//        {
//            GetVmwareInfrastructure(item);
//        }
//    }


//    /// <summary>
//    /// Get the VMWare infrastructure in the network
//    /// </summary>
//    /// <param name="item">IP address of the VMWare hosts separated by semicolon</param>
//    public void GetVmwareInfrastructure(NetworkItem item)
//    {
//        if (item == null)
//        {
//            return;
//        }

//        if (string.IsNullOrEmpty(item.Ip))
//        {
//            return;
//        }

//        try
//        {
//            var x = Network.VirtualInfrastructure.AddOrUpdateVirtualHost(item.Ip);
//            x.Url = item.Ip;
//            x.UserName = item.Username;
//            x.Password = item.Password;
//            x.HostType = VirtualMachineType.VmWare;

//            x.GetVirtualMachines();
//        }
//        catch (Exception e)
//        {
//            Debug.Print(e.Message);
//        }


//    }

//    /// <summary>
//    /// Get the HyperV infrastructure in the network
//    /// </summary>
//    /// <param name="item">HyperV host</param>
//    public void GetHypervInfrastructure(NetworkItem item)
//    {
//        if (item == null)
//        {
//            return;
//        }

//        if (string.IsNullOrEmpty(item.Ip))
//        {
//            return;
//        }

//        var x = Network.VirtualInfrastructure.AddOrUpdateVirtualHost(item.Ip);
//        x.Url = item.Ip;
//        x.Domain = string.IsNullOrEmpty(item.Username) ? Network.Domain.Name : "";
//        x.UserName = string.IsNullOrEmpty(item.Username) ? CurrentSettings.Username : item.Username;
//        x.Password = string.IsNullOrEmpty(item.Password) ? CurrentSettings.Password : item.Password;
//        x.HostType = VirtualMachineType.HyperV;
//        x.GetVirtualMachines();

//        item.Roles.Add(new RoleItem { Name = "HyperV-Host" });

//    }

//    /// <summary>
//    /// Save infrastructure as XML
//    /// </summary>
//    public void SaveInfrastructure()
//    {
//        if (Network.VirtualInfrastructure == null)
//        {
//            return;
//        }

//        try
//        {
//            var s = Web.Basics.SerializationHelper.DataContractSerialize(Network.VirtualInfrastructure);

//            if (!Directory.Exists(CurrentSettings.XmlTargetDir))
//            {
//                Directory.CreateDirectory(CurrentSettings.XmlTargetDir);
//            }

//            if (File.Exists(CurrentSettings.InfrastructureXmlFile))
//            {
//                File.Delete(CurrentSettings.InfrastructureXmlFile);
//            }
//            var sw = new StreamWriter(CurrentSettings.InfrastructureXmlFile, false, Encoding.GetEncoding("utf-8"));
//            sw.Write(s);
//            sw.Close();
//        }
//        catch //(Exception e)
//        {
//            // Do nothing
//        }
//    }


//    /// <summary>
//    /// Get direct user permissions for shares (access permission not via security group but directly)
//    /// </summary>
//    public void GetDirectUserPermissions()
//    {
//        var shares = Network.NetworkItems.SelectMany(x => x.Shares).ToList().Where(x => x.Type != "Disk Drive Admin").ToList();

//        foreach (var share in shares)
//        {
//            foreach (var acl in share.AccessControlList)
//            {
//                var y = acl.Identifier;
//                var user = Network.Domain.Users.Find(x => x.Fullname == y);

//                if (user == null) continue;

//                if (user.DirectPermissions.IndexOf(share.Name) < 0)
//                {
//                    user.DirectPermissions.Add(share.Name);
//                }
//            }
//        }
//    }

//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="item"></param>
//    public void GetWmiDataFromNetworkItem(NetworkItem item)
//    {
//        //try
//        //{

//        if (item.ItemType != NetworkItemType.Windows) return;

//        IWmiNetworkItemDataProvider wmiNetworkItem;

//        //if (string.IsNullOrEmpty(item.Username)) wmiNetworkItem = new WmiCimNetworkItemDataProvider();
//        //else wmiNetworkItem = new WmiNetworkItemDataProvider();

//        if (item.IsCimObject)
//        {

//            GotStatus("GetDataFromNetworkItem",
//                $"Get CIM data from {item.HostName} ({item.IpAddresses[0]})");

//            // ReSharper disable once UseObjectOrCollectionInitializer
//            wmiNetworkItem = new WmiCimNetworkItemDataProvider();
//            wmiNetworkItem.LocalIp = CurrentSettings.LocalIp;
//            wmiNetworkItem.Ip = item.IpAddresses[0];
//            wmiNetworkItem.HostName = item.DnsHostName;
//            wmiNetworkItem.XmlFileName = item.XmlFile;
//            wmiNetworkItem.Domain = string.IsNullOrEmpty(item.Username) ? Network.Domain.Name : item.DnsHostName;
//            wmiNetworkItem.Username = string.IsNullOrEmpty(item.Username) ? CurrentSettings.Username : item.Username;
//            wmiNetworkItem.Password = string.IsNullOrEmpty(item.Password) ? CurrentSettings.Password : item.Password;

//            wmiNetworkItem.Start("NetworkItemRoot");
//            wmiNetworkItem.GetMetaData();
//            wmiNetworkItem.GetHardware();
//            wmiNetworkItem.GetSoftware();
//            wmiNetworkItem.Save();
//        }
//        else
//        {
//            if (item.IsWmiObject)
//            {
//                GotStatus("GetDataFromNetworkItem",
//                    $"Get WMI data from {item.HostName} ({item.IpAddresses[0]})");

//                // ReSharper disable once UseObjectOrCollectionInitializer
//                wmiNetworkItem = new WmiNetworkItemDataProvider();
//                wmiNetworkItem.LocalIp = CurrentSettings.LocalIp;
//                wmiNetworkItem.Ip = item.IpAddresses[0];
//                wmiNetworkItem.HostName = item.DnsHostName;
//                wmiNetworkItem.XmlFileName = item.XmlFile;
//                wmiNetworkItem.Domain =
//                    string.IsNullOrEmpty(item.Username) ? Network.Domain.Name : item.DnsHostName;
//                wmiNetworkItem.Username = string.IsNullOrEmpty(item.Username) ? CurrentSettings.Username : item.Username;
//                wmiNetworkItem.Password = string.IsNullOrEmpty(item.Password) ? CurrentSettings.Password : item.Password;

//                wmiNetworkItem.Start("NetworkItemRoot");
//                wmiNetworkItem.GetMetaData();
//                wmiNetworkItem.GetHardware();
//                wmiNetworkItem.GetSoftware();
//                wmiNetworkItem.Save();
//            }
//            else
//            {
//                if (!File.Exists(item.XmlFile))
//                {
//                    item.SummaryFile = null;
//                    item.DetailFile = null;
//                }

//                return;
//            }
//        }


//        if (wmiNetworkItem.Error)
//        {
//            GotStatus("GetDataFromNetworkItem", $"Error getting data from {item.HostName} ({item.IpAddresses[0]})");
//            if (!File.Exists(item.XmlFile))
//            {
//                item.SummaryFile = null;
//                item.DetailFile = null;
//            }
//            return;
//        }





//        var wmc = new WmiNetworkItemDataConverter
//        {
//            Data = wmiNetworkItem,
//            NetworkItem = item
//        };

//        wmc.GetIpAddresses();

//        string vmHost;
//        var vm = CheckIfNetworkItemIsVirtualMachine(item, out vmHost);

//        wmiNetworkItem.VirtualMachine = vm;
//        wmiNetworkItem.VirtualMachineHost = vmHost;

//        wmc.GetVmData();
//        wmc.GetDrives();
//        wmc.GetLogicalDrives();
//        wmc.GetNetworkAdapters();
//        wmc.GetRam();
//        wmc.GetOperatingSystem();
//        wmc.GetComputerSystem();
//        wmc.GetSoftware();
//        wmc.GetShares();

//        GotStatus("GetDataFromNetworkItem", $"Got data from {item.HostName} ({item.IpAddresses[0]})");

//        //}
//        //catch (Exception ex)
//        //{
//        //    Debug.Print(ex.Message);
//        //    // ignored
//        //}
//    }

//    private bool CheckIfNetworkItemIsVirtualMachine(NetworkItem item, out string vmHost)
//    {
//        var vm = false;
//        vmHost = null;

//        foreach (var h in Network.VirtualInfrastructure.VirtualHosts)
//        {
//            if (h.VirtualMachines.Exists(x => item.IpAddresses.Contains(x.Ip))) vm = true;
//            item.VirtualMachine = vm;

//            if (!vm)
//            {
//                item.VmHost = null;
//                continue;
//            }

//            vmHost = $"{h.HostType}-Host {h.Url}";
//            item.VmHost = vmHost;
//            break;
//        }

//        return vm;
//    }

//    /// <summary>
//    /// Get server roles from windows servers
//    /// </summary>
//    public void GetServerRoles()
//    {
//        foreach (var item in Network.NetworkItems.Where(x => x.DomainRoleId > 2))
//        {
//            GetServerRolesForItem(item);
//        }
//    }

//    private void GetServerRolesForItem(NetworkItem item)
//    {

//        try
//        {
//            var ws = new WindowsServerHandler
//            {
//                Domain = string.IsNullOrEmpty(item.Username) ? Network.Domain.Name : "",
//                Username = string.IsNullOrEmpty(item.Username) ? CurrentSettings.Username : item.Username,
//                Password = string.IsNullOrEmpty(item.Password) ? CurrentSettings.Password : item.Password,
//                Ip = item.Ip
//            };

//            var result = ws.GetInstalledRoles();

//            foreach (var role in result)
//            {
//                if (!item.Roles.Exists(x => x.Name.ToLower() == role.ToLower()))
//                {
//                    item.Roles.Add(new RoleItem { Name = role });
//                }
//            }
//        }
//        catch (Exception e)
//        {
//            Debug.Print(e.Message);
//            throw;
//        }

//    }


//    protected void GotStatus(string modul, string msg)
//    {
//        //_IsStart = false;
//        var x = CurrentSettings.StatusMessage;
//        if (x != null) x(modul, msg);
//    }

//    /// <summary>
//    /// Collect all SNMP request from <see cref="GeneralSettings.JsonDir"/>. Files must start with SNMP_.
//    /// </summary>
//    public void GetSnmpRequests()
//    {
//        foreach (var file in new DirectoryInfo(FileHelper.JsonDir).GetFiles("*.json"))
//        {
//            if (!file.Name.ToLower().StartsWith("snmp_")) continue;

//            SnmpRequest request = null;

//            try
//            {
//                request = JsonHelper.LoadJsonFile<SnmpRequest>(file.FullName);
//            }
//            catch (Exception e)
//            {
//                Debug.Print(e.Message);
//            }

//            if (request == null) continue;

//            try
//            {
//                NetworkHelper.Ping(request.IpAddresses[0]);
//            }
//            catch
//            {
//                // ignored
//            }

//            SnmpRequests.Add(request);

//        }
//    }


//    /// <summary>
//    /// Collect data from snmp sources
//    /// </summary>
//    public void GetSnmpRequestData()
//    {

//        if (SnmpRequests == null || SnmpRequests.Count==0) return;

//        var snmp = new SnmpRequestHandler();
            
//        foreach (var snmpRequest in SnmpRequests)
//        {
//            GetSnmpRequestDataFromItem(snmp, snmpRequest);
//        }
//    }

//    /// <summary>
//    /// Collect data from a snmp source
//    /// </summary>
//    /// <param name="snmp"></param>
//    /// <param name="snmpRequest">SNMP request</param>
//    public void GetSnmpRequestDataFromItem(SnmpRequestHandler snmp, SnmpRequest snmpRequest)
//    {
//        try
//        {
//            GotStatus("GetSnmpRequestDataFromItem", $"Get SNMP data from {snmpRequest.IpAddresses[0]}");

//            var fileName = NetworkItemHelper.GetXmlFilePath(CurrentSettings.XmlTargetDir, snmpRequest.IpAddresses[0]);

//            var pr = new SnmpNetworkItemDataProvider(snmp)
//            {
//                SnmpRequest = snmpRequest,
//                XmlFileName = fileName
//            };

//            pr.Start("NetworkItemRoot");
//            pr.GetMetaData();

//            pr.GetSnmpData();
//            pr.CreateXmlFromSnmpData();

//            pr.Save();

//            var item = AddOrUpdateNetworkItem(snmpRequest);

//            if (item == null) return;

//            item.XmlFile = fileName;

//            foreach (var warning in snmpRequest.Warnings)
//            {
//                item.Warnings.Add(warning);
//            }               

//            GotStatus("GetSnmpRequestDataFromItem", $"Get SNMP data from {snmpRequest.IpAddresses[0]}");
//        }
//        catch (Exception e)
//        {
//            Debug.Print(e.Message);

//        }

//    }


//    /// <summary>
//    /// Save the network data as JSON for next run of app
//    /// </summary>
//    public void SaveNetwork()
//    {
//        JsonHelper.SaveAsFile(FileHelper.NetworkJsonFileName, Network);
//    }


//    /// <summary>
//    /// Check and create warnings for network items
//    /// </summary>
//    public void CheckWarnings()
//    {
//        CheckDiskSpaceWarnings();
//    }

//    /// <summary>
//    /// Check disk spaces of the network items and create warnings
//    /// </summary>
//    public void CheckDiskSpaceWarnings()
//    {
//        // Check absolute disk space
//        var items = Network.NetworkItems.Where(item => item.DomainRole.ToLower().Contains("server")
//                                                       && item.LogicalDrives.Any(disk =>
//                                                           (double) disk.FreeSpace <=
//                                                           CurrentSettings.DiskLimitAbsolut));

//        foreach (var item in items)
//        {
//            var disks = item.LogicalDrives.Where(disk =>
//                (double) disk.FreeSpace <=
//                CurrentSettings.DiskLimitAbsolut);
//            foreach (var disk in disks)
//            {
//                var warning = new Warning
//                {
//                    WarningSourceType = WarningSourceType.Hardware,
//                    WarningSeverityLevel = WarningSeverityLevel.High,
//                    Message =
//                        $"Network item {item.HostName} [IP {item.Ip}]: free disk size [{disk.FreeSpace} GB] lower than limit [{CurrentSettings.DiskLimitAbsolut} GB]"
//                };

//                item.Warnings.Add(warning);
//            }
//        }

//        // Check relative disk space
//        items = Network.NetworkItems.Where(item => item.DomainRole.ToLower().Contains("server")
//                                                   && item.LogicalDrives.Any(disk =>
//                                                       (double)disk.FreeSpace / (double)disk.Size 
//                                                       <= CurrentSettings.DiskLimitRelative));

//        foreach (var item in items)
//        {
//            var disks = item.LogicalDrives.Where(disk =>
//                (double)disk.FreeSpace / (double)disk.Size <= CurrentSettings.DiskLimitRelative);
//            foreach (var disk in disks)
//            {
//                var warning = new Warning
//                {
//                    WarningSourceType = WarningSourceType.Hardware,
//                    WarningSeverityLevel = WarningSeverityLevel.High,
//                    Message =
//                        $"Network item {item.HostName} [IP {item.Ip}]: free disk size [{disk.FreeSpace} GB] lower than limit of {CurrentSettings.DiskLimitRelative * 100} % of volume space"
//                };

//                item.Warnings.Add(warning);
//            }
//        }
//    }

//    /// <summary>
//    /// Load network data from JSON
//    /// </summary>
//    /// <param name="jsonFile"></param>
//    public void LoadNetworkDataFromJson(string jsonFile)
//    {
//        if (!File.Exists(jsonFile))
//        {
//            throw new Exception("JSON file with network data not found!");
//        }

//        Network = JsonHelper.LoadJsonFile<Network>(jsonFile);
//    }

//    /// <summary>
//    /// Check the item if the hostnames is a real NETBIOS name (and not the fully qualified AD host name)
//    /// </summary>
//    public void CheckHostNames()
//    {

//        foreach (var item in Network.NetworkItems)
//        {
//            item.HostName = item.HostName.Replace("." + Network.Domain.Name, "");
//        }
//    }
//}