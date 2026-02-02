using System;
using System.Diagnostics;
using System.Globalization;
using System.Xml;
using Bodoconsult.Inventory.Helper;
using Bodoconsult.Inventory.Interfaces;
using Bodoconsult.Inventory.Model;

namespace Bodoconsult.Inventory.Converter;

public class WmiNetworkItemDataConverter
{

    public IWmiNetworkItemDataProvider Data { get; set; }


    public NetworkItem NetworkItem { get; set; }


    //public string HtmlTargetDir { get; set; }

    //public string AppPath { get; set; }


    public void GetIpAddresses()
    {
        var list = Data.GetNodes("Hardware", "NetworkAdapters");

        for (var i = 0; i < list.Count; i++)
        {
            var s =
                $"NetworkItemSubValue[attribute::name='{"NetworkConfiguration"}']/NetworkItemSubItem[attribute::name='{"NetworkConfiguration 1"}']/NetworkItemSubItemValue[attribute::name='{"IPAddress"}']";
            //, "NetworkConfiguration 1", "IPAddress");

            // /
            var node = list[i].SelectSingleNode(s);

            if (node == null) continue;

            s = node.InnerXml;

            foreach (var ip in s.Split(' '))
            {
                if (string.IsNullOrEmpty(ip)) continue;

                if (!NetworkItem.IpAddresses.Contains(ip))
                {
                    NetworkItem.IpAddresses.Add(ip);
                }
            }
        }


    }



    public void GetVmData()
    {
        Data.GetVmData();
        Data.Save();
    }

    public void GetDrives()
    {
        var list = Data.GetNodes("Hardware", "DiskDrives");

        for (var i = 0; i < list.Count; i++)
        {

            var item = new DriveItem();

            var node = SelectValue(list[i], "Index");
            if (node != null)
            {
                int id;
                int.TryParse(node, out id);
                item.DriveId = id;
            }


            node = SelectValue(list[i], "Size");
            if (node != null) item.Size = GetGigaBytes(node);

            node = SelectValue(list[i], "Name");
            if (node != null) item.Name = node;

            NetworkItem.Drives.Add(item);
        }

    }

    public void GetLogicalDrives()
    {
        var list = Data.GetNodes("Hardware", "LogicalDrives");

        for (var i = 0; i < list.Count; i++)
        {

            var node = SelectValue(list[i], "Name");
            if (node == null) continue;

            node = SelectValue(list[i], "DriveType");

            if (node == null) continue;

            var type = Convert.ToInt32(node);
            if (type != 3) continue;

            var item = new LogicalDriveItem { Type = type };

            node = SelectValue(list[i], "Size");
            if (node != null) item.Size = GetGigaBytes(node);

            node = SelectValue(list[i], "FreeSpace");
            if (node != null) item.FreeSpace = GetGigaBytes(node);

            node = SelectValue(list[i], "Name");
            if (node != null) item.Name = node;

            node = SelectValue(list[i], "FileSystem");
            if (node != null) item.FileSystem = node;

            NetworkItem.LogicalDrives.Add(item);
        }

    }




    public void GetNetworkAdapters()
    {
        var list = Data.GetNodes("Hardware", "NetworkAdapters");

        for (var i = 0; i < list.Count; i++)
        {

            var mainNode = list[i];

            var node = SelectValue(mainNode, "AdapterType");
            if (node == null) continue;

            var wert = SelectValue(mainNode, "AdapterType");
            if (!wert.Contains("Ethernet")) continue;

            var item = new NetworkAdapterItem();

            node = SelectValue(mainNode, "DeviceID");
            if (node != null) item.Id = Convert.ToInt32(node);

            node = SelectValue(mainNode, "MACAddress");
            if (node != null) item.MacAddress = node;

            node = SelectValue(mainNode, "Speed");
            if (node != null) item.Speed = GetNetworkSpeed(node);

            node = SelectValue(mainNode, "Name");
            if (node != null) item.Name = node;


            var subNode = mainNode.SelectSingleNode("NetworkItemValue[attribute::name='NetworkConfiguration']");
            node = SelectValue(subNode, "IpAddress");
            if (node != null) item.IpAddress = node;

            node = SelectValue(subNode, "DNSServerSearchOrder");
            if (node != null) item.DnsServer = node;

            node = SelectValue(subNode, "DHCPEnabled");
            if (node != null) item.DhcpEnabled = (node.ToLower() != "false");

            node = SelectValue(subNode, "DefaultIPGateway");
            if (node != null) item.DefaultIpGateway = node;

            NetworkItem.NetworkAdapters.Add(item);

        }



    }

    public void GetOperatingSystem()
    {
        var node = Data.GetFirstNode("Software", "OperatingSystem");


        // Operating system

        //if (string.IsNullOrEmpty(NetworkItem.OperatingSystem)) 
        NetworkItem.OperatingSystem = SelectValue(node, "Caption");

        NetworkItem.FreeRam = GetMegaBytes(SelectValue(node, "FreePhysicalMemory"));


        // Install date
        var erg = SelectValue(node, "InstallDate");

        if (string.IsNullOrEmpty(erg)) return;

        erg = erg.Substring(0, 10);


        try
        {
            NetworkItem.InstallDate = Convert.ToDateTime(erg);
        }
        catch
        {
            try
            {
                NetworkItem.InstallDate = DateTime.Parse(string.Format("{2}.{1}.{0}", erg.Substring(0, 4), erg.Substring(4, 2), erg.Substring(6, 2)));
            }
            catch (Exception e)
            {
                Debug.Print("InstallDate:Error:" + erg);
            }
        }
    }


    public void GetComputerSystem()
    {
        var node = Data.GetFirstNode("Hardware", "BaseBoard");

        // Virtuelle Maschine
        var value = SelectValue(node, "Product");
        NetworkItem.VirtualMachine = (value == "Virtual Machine");     

        node = Data.GetFirstNode("Hardware", "BaseBoard");
        value = SelectValue(node, "SerialNumber");
        NetworkItem.SerialNumber = value;

        node = Data.GetFirstNode("Software", "ComputerSystem");
        value = SelectValue(node, "Manufacturer");
        NetworkItem.Manufacturer = value;

        value = SelectValue(node, "Model");
        NetworkItem.Model = value;


        //// Virtuelle Maschine
        //product = SelectValue(node, "Product");
        //NetworkItem.VirtualMachine = (product == "Virtual Machine");




        // Check VMWare
        if (NetworkItem.VirtualMachine == false)
        {
            var bios = Data.GetFirstNode("Hardware", "Bios");

            //SerialNumber

            value = SelectValue(bios, "SerialNumber");

            if (value != null && value.ToLower().Contains("vmware")) NetworkItem.VirtualMachine = true;

        }


        node = Data.GetFirstNode("Software", "ComputerSystem");

        // Domänenrolle
        NetworkItem.DomainRole = DomainHelper.TranslateDomainRole(SelectValue(node, "DomainRole"));
        NetworkItem.DomainRoleId = Convert.ToInt32(SelectValue(node, "DomainRole"));

        // Number of processors
        NetworkItem.NumberOfProcessors = Convert.ToInt32(SelectValue(node, "NumberOfProcessors"));

        // Dns-Server
        NetworkItem.DnsHostName = SelectValue(node, "DnsHostName");



        // Rollen
        if (node != null)
        {

            var testNode = node.SelectSingleNode("NetworkItemValue[attribute::name='Roles']");
            if (testNode != null)
            {
                var subNode = testNode.ChildNodes;

                for (var i = 0; i < subNode.Count; i++)
                {

                    node = subNode[i];

                    var role = new RoleItem
                    {
                        Name = node.InnerText
                    };

                    NetworkItem.Roles.Add(role);

                }
            }

        }
    }




    public void GetRam()
    {
        var list = Data.GetNodes("Hardware", "PhysicalMemory");

        long ram = 0;

        for (var i = 0; i < list.Count; i++)
        {
            var node = SelectValue(list[i], "Capacity");
            if (node == null) continue;

            ram += Convert.ToInt64(node);
        }

        NetworkItem.Ram = GetMegaBytes(ram);

    }


    public void GetSoftware()
    {
        var list = Data.GetNodes("Software", "InstalledSoftware");

        for (var i = 0; i < list.Count; i++)
        {

            var mainNode = list[i];

            var software = new SoftwareItem();
            var node = SelectValue(mainNode, "IdentifyingNumber");
            software.IdentifyingNumber = string.IsNullOrEmpty(node) ? "unknown" : node;

            node = SelectValue(mainNode, "Name");
            if (node != null) software.Name = node;

            node = SelectValue(mainNode, "Vendor");
            software.Vendor = string.IsNullOrEmpty(node) ? "unknown" : node;

            node = SelectValue(mainNode, "Version");
            software.Version = string.IsNullOrEmpty(node) ? "unknown" : node;

            NetworkItem.Software.Add(software);

        }


    }


    private static string SelectValue(XmlNode node, string value)
    {
        if (node == null) return null;
        var n = node.SelectSingleNode($"NetworkItemValue[attribute::name='{value}']");
        return n == null ? null : n.InnerText;
    }

    private static long GetMegaBytes(long wert)
    {
        var erg = wert / 1024 / 1024;

        return erg;
    }

    private static long GetMegaBytes(string wert)
    {
        var erg = Convert.ToInt64(wert) / 1024 / 1024;

        return erg;
    }

    private static long GetGigaBytes(string wert)
    {
        var erg = Convert.ToInt64(wert) / 1024 / 1024 / 1024;

        return erg;
    }

    private static long GetNetworkSpeed(string wert)
    {

        if (string.IsNullOrEmpty(wert)) wert = "0";

        var erg = Convert.ToInt64(wert) / 1024 / 1024;

        if (erg < 100)
        {
            erg = 10;
        }
        else if (erg < 1000)
        {
            erg = 100;
        }
        else if (erg < 10000)
        {
            erg = 1000;
        }
        else
        {
            erg = 10000;
        }

        return erg;
    }


    private static long GetGigaBytes(long wert)
    {
        var erg = Convert.ToInt64(wert) / 1024 / 1024 / 1024;

        return erg;
    }


    //public void SaveAsHtml()
    //{


    //    try
    //    {
    //        var s = Web.Basics.SerializationHelper.DataContractSerialize(NetworkItem);

    //        var byteArray = Encoding.UTF8.GetBytes(s);

    //        var m = new MemoryStream(byteArray);




    //        var xsl = new XsltHandler { XslFile = Path.Combine(AppPath, "prototypes", "summary.xsl") };
    //        xsl.Start();

    //        foreach (var ip in NetworkItem.IpAddresses)
    //        {
    //            var target = Path.Combine(HtmlTargetDir, string.Format("{0}_summary.htm", ip.Replace(".", "_")));
    //            xsl.Transform(m, target);
    //        }


    //        //target = Path.Combine(HtmlTargetDir, String.Format("{0}_summary.xml", NetworkItem.Ip));

    //        //var sw = new StreamWriter(target, false, Encoding.UTF8);
    //        //m.WriteTo(sw.BaseStream);
    //        //sw.Close();
    //        //sw.Dispose();
    //    }
    //    catch //(Exception ex)
    //    {
    //        NetworkItem.SummaryFile = null;
    //    }


    //}




    private string TranslatePowerState(int code)
    {
        switch (code)
        {
            case 1: return "Full Power";
            case 2: return "Power Save - Low Power Mode";
            case 3: return "Power Save - Standby";
            case 4: return "Power Save - Unknown";
            case 5: return "Power Cycle";
            case 6: return "Power Off";
            case 7: return "Power Save - Warning";

            default: return "Unknown (" + code.ToString(CultureInfo.InvariantCulture) + ")";
        }

    }


    public string TranslatePowerSupplyState(int code)
    {

        switch (code)
        {
            case 1: return "Other";
            case 3: return "Safe";
            case 4: return "Warning";
            case 5: return "Critical";
            case 6: return "Non-recoverable";

            default: return "Unknown (" + code.ToString(CultureInfo.InvariantCulture) + ")";
        }

    }

    public string TranslateWakeUp(int code)
    {
        switch (code)
        {
            case 0: return "Reserved";
            case 1: return "Other";
            case 3: return "APM Timer";
            case 4: return "Modem Ring";
            case 5: return "LAN Remote";
            case 6: return "Power Switch";
            case 7: return "PCI PME#";
            case 8: return "AC Power Restored";

            default: return "Unknown (" + code.ToString(CultureInfo.InvariantCulture) + ")";
        }
    }


    public string TranslateDebugInfoType(int code)
    {
        switch (code)
        {
            case 0: return "None";
            case 1: return "Complete memory dump";
            case 2: return "Kernel memory dump";
            case 3: return "Small memory dump";
            default: return "Other (" + code.ToString(CultureInfo.InvariantCulture) + ")";
        }
    }

    public string TranslateOdbcRegistraton(int code)
    {
        switch (code)
        {
            case 0: return "Per machine";
            case 1: return "Per user";

            default: return "Other (" + code.ToString(CultureInfo.InvariantCulture) + ")";
        }
    }

    public string TranslateConseqCapabilities(int code)
    {
        switch (code)
        {
            case 1: return "Other";
            case 2: return "Sequential Access";
            case 3: return "Random Access";
            case 4: return "Supports Writing";
            case 5: return "Encryption";
            case 6: return "Compression";
            case 7: return "Supports Removable Media";
            case 8: return "Manual Cleaning";
            case 9: return "Automatic Cleaning";
            case 10: return "SMART Notification";
            case 11: return "Supports Dual Sided Media";
            case 12: return "Predismount Eject Not Required";
            default: return "Unknown (" + code.ToString(CultureInfo.InvariantCulture) + ")";
        }
    }

    public string TranslateAnswerMode(int code)
    {
        switch (code)
        {
            case 2: return "Other";
            case 3: return "Disabled";
            case 4: return "Manual Answer";
            case 5: return "Auto Answer";
            case 6: return "Auto Answer with Call-Back";
            default: return "Unknown (" + code.ToString(CultureInfo.InvariantCulture) + ")";
        }
    }

    public string TranslateDialTone(int code)
    {
        switch (code)
        {
            case 1: return "Tone";
            case 2: return "Pulse";
            default: return "Unknown (" + code.ToString(CultureInfo.InvariantCulture) + ")";
        }
    }

    public string TranslateModemPort(string code)
    {
        switch (int.Parse(code.Substring(2, 1)))
        {
            case 0: return "Parallel Port";
            case 1: return "Serial Port";
            case 2: return "Modem";

            default: return "Other (" + code.ToString(CultureInfo.InvariantCulture) + ")";
        }
    }

    public string TranslatePrintColor(int code)
    {
        switch (code)
        {
            case 1: return "Monochrome";
            case 2: return "Color";

            default: return "Other (" + code.ToString(CultureInfo.InvariantCulture) + ")";
        }
    }


    private string TranslatePrintDitherType(int code)
    {


        switch (code)
        {
            case 1: return "No Dithering";
            case 2: return "Coarse Brush";
            case 3: return "Fine Brush";
            case 4: return "Line Art";
            case 5: return "Greyscale";
            default: return "Other (" + code.ToString(CultureInfo.InvariantCulture) + ")";
        }
    }

    private string TranslatePrintIcmIntent(int code)
    {


        switch (code)
        {
            case 1: return "Saturation";
            case 2: return "Contrast";
            case 3: return "Exact Color";
            default: return "Other (" + code.ToString(CultureInfo.InvariantCulture) + ")";
        }
    }

    private string TranslatePrintIcmMethod(int code)
    {
        switch (code)
        {
            case 1: return "Disabled";
            case 2: return "Windows";
            case 3: return "Device Driver";
            case 4: return "Device";
            default: return "Other (" + code.ToString(CultureInfo.InvariantCulture) + ")";
        }
    }

    private string TranslatePrintMediaType(int code)
    {
        switch (code)
        {
            case 1: return "Standard";
            case 2: return "Transparency";
            case 3: return "Glossy";
            default: return "Other (" + code.ToString(CultureInfo.InvariantCulture) + ")";
        }
    }

    private string TranslatePrintOrientation(int code)
    {
        switch (code)
        {
            case 1: return "Portrait";
            case 2: return "Landscape";
            default: return "Other (" + code.ToString(CultureInfo.InvariantCulture) + ")";
        }
    }

    private string TranslatePrintTrueTypeOption(int code)
    {
        switch (code)
        {
            case 1: return "TrueType fonts as graphics";
            case 2: return "TrueType fonts as soft fonts (PCL printers)";
            case 3: return "Substitute device fonts for TrueType fonts";
            default: return "Other (" + code.ToString(CultureInfo.InvariantCulture) + ")";
        }
    }

    private string TranslateServicePhilosophy(int code)
    {
        switch (code)
        {
            case 0: return "Unknown";
            case 2: return "Service From Top";
            case 3: return "Service From Front";
            case 4: return "Service From Back";
            case 5: return "Service From Side";
            case 6: return "Sliding Trays";
            case 7: return "Removable Sides";
            case 8: return "Moveable";
            default: return "Other (" + code.ToString(CultureInfo.InvariantCulture) + ")";
        }
    }

    private string TranslateChassisType(int code)
    {
        switch (code)
        {
            case 2: return "Unknown";
            case 3: return "Desktop";
            case 4: return "Low Profile Desktop";
            case 5: return "Pizza Box";
            case 6: return "Mini Tower";
            case 7: return "Tower";
            case 8: return "Portable";
            case 9: return "Laptop";
            case 10: return "Notebook";
            case 11: return "Hand Held";
            case 12: return "Docking Station";
            case 13: return "All in One";
            case 14: return "Sub Notebook";
            case 15: return "Space-Saving";
            case 16: return "Lunch Box";
            case 17: return "Main System Chassis";
            case 18: return "Expansion Chassis";
            case 19: return "SubChassis";
            case 20: return "Bus Expansion Chassis";
            case 21: return "Peripheral Chassis";
            case 22: return "Storage Chassis";
            case 23: return "Rack Mount Chassis";
            case 24: return "Sealed-Case PC";
            default: return "Other (" + code.ToString(CultureInfo.InvariantCulture) + ")";
        }
    }


    private static string TranslateShareType(string code)
    {
        switch (Convert.ToInt64(code))
        {
            case 0:
                return "Disk Drive";
            case 1:
                return "Print Queue";
            case 2:
                return "Device";
            case 3:
                return "IPC";
            case 2147483648:
                return "Disk Drive Admin";
            case 2147483649:
                return "Print Queue Admin";
            case 2147483650:
                return "Device Admin";
            case 2147483651:
                return "IPC Admin";
        }
        return null;
    }


    public void GetShares()
    {
        var list = Data.GetNodes("Software", "Shares");

        for (var i = 0; i < list.Count; i++)
        {

            var item = new ShareItem();

            var node = SelectValue(list[i], "Type");
            if (node != null) item.Type = TranslateShareType(node);

            switch (item.Type)
            {
                case "Print Queue Admin":
                case "Disk Drive Admin":
                case "IPC Admin":
                    continue;
                //default:
                //    break;
            }

            node = SelectValue(list[i], "Description");
            if (node != null) item.Description = node;

            node = SelectValue(list[i], "Path");
            if (node != null) item.Path = node;

            node = SelectValue(list[i], "Name");
            if (node != null)
            {

                if (node.ToLower().Contains("print$")) continue;

                item.Name =
                    $@"\\{((string.IsNullOrEmpty(NetworkItem.HostName)) ? NetworkItem.IpAddresses[0] : NetworkItem.HostName)}\{node}";
            }


            try
            {
                var d = new DirectoryAccessReader
                {
                    DirectoryPath = item.Name
                };


                d.ReadData();

                item.AccessControlList = d.AccessControlList;
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch
                // ReSharper restore EmptyGeneralCatchClause
            {

            }


            NetworkItem.Shares.Add(item);
        }

    }


}