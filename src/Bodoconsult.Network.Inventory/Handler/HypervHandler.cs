using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Xml.XPath;
using Bodoconsult.Inventory.Helper;
using Bodoconsult.Inventory.Model;

namespace Bodoconsult.Inventory.Handler;

/// <summary>
///  Class to retrieve information about virtual machines (VM) on a HyperV virtualization host
/// </summary>
public class HypervHandler
{

    public string Domain { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }


    internal ManagementScope WmiScope;

    internal ConnectionOptions ConnectionOptions;


    public bool Error { get; set; }

    /// <summary>
    /// IP address of the HyperV virtualization host.
    /// </summary>
    public string IpAddress { get; set; }


    public void Connect()
    {
        Error = false;
        try
        {

            var localIp = NetworkHelper.GetLocalIpAddress();
            var hostName = IpAddress == localIp ? "localhost" : IpAddress;

            ConnectionOptions = WmiHelper.GetWmiConnectionOptions(IpAddress, localIp, Domain, Username, Password);

            WmiScope = new ManagementScope($@"\\{hostName}\root\virtualization\v2", ConnectionOptions);
            WmiScope.Connect();



        }
        catch (Exception ex)
        {
            Error = true;

            var msg = $"{IpAddress} {ex.Message}";
            throw new Exception(msg);

        }
    }

    public void GetVirtualMachines(List<VirtualMachine> result)
    {
        // define the information we want to query - in this case, just grab all properties of the object
        var queryObj = new SelectQuery("SELECT * FROM Msvm_ComputerSystem WHERE Description='Microsoft Virtual Machine'");

        // connect and set up our search
        var vmSearcher = new ManagementObjectSearcher(WmiScope, queryObj);
        var vmCollection = vmSearcher.Get();

        // loop through the VMs
        foreach (var o in vmCollection)
        {
            var vm = (ManagementObject)o;

            if (vm == null)
            {
                continue;
            }

            var item = new VirtualMachine
            {
                Name = vm["ElementName"].ToString(),
                HostName = "unknown",
                GuestOs = "unknown",
                Ip = "unknown"
            };

            // Get IP

            var settings = vm.GetRelated("Msvm_VirtualSystemSettingData");
            foreach (ManagementObject setting in settings)
            {
                //Get hold of the settings:
                var adapters = setting.GetRelated("Msvm_SyntheticEthernetPortSettingData");
                foreach (ManagementObject adapter in adapters)
                {
                    //Look for the configured MAC address:
                    var configs = adapter.GetRelated("Msvm_GuestNetworkAdapterConfiguration");
                    foreach (ManagementObject config in configs)
                    {
                        var macAddress = (string[])config.GetPropertyValue("IPAddresses");
                        if (macAddress != null && macAddress.Length > 0) item.Ip = macAddress[0];
                    }

                }
            }


            var kvpExchangeComponents = vm.GetRelated("Msvm_KvpExchangeComponent");
            if (kvpExchangeComponents.Count != 1)
            {
                throw new Exception(
                    $"{kvpExchangeComponents.Count} instance of Msvm_KvpExchangeComponent was found");
            }




            foreach (ManagementObject kvpExchangeComponent in kvpExchangeComponents)
            {
                foreach (var exchangeDataItem in (string[])kvpExchangeComponent["GuestIntrinsicExchangeItems"])
                {

                    //Debug.Print(exchangeDataItem);

                    var xpathDoc = new XPathDocument(new StringReader(exchangeDataItem));
                    var navigator = xpathDoc.CreateNavigator();
                    navigator = navigator.SelectSingleNode("/INSTANCE/PROPERTY[@NAME='Name']/VALUE[child::text() = 'FullyQualifiedDomainName']");
                    if (navigator != null)
                    {
                        navigator = navigator.SelectSingleNode("/INSTANCE/PROPERTY[@NAME='Data']/VALUE/child::text()");
                        if (navigator != null)
                        {
                            item.HostName = navigator.Value;
                        }

                        //Debug.Print("Virtual machine {0} DNS name is: {1}", vm["ElementName"].ToString(), navigator.Value);
                        //break;
                    }

                    navigator = xpathDoc.CreateNavigator();
                    navigator = navigator.SelectSingleNode("/INSTANCE/PROPERTY[@NAME='Name']/VALUE[child::text() = 'OSName']");
                        
                    if (navigator == null)
                    {
                        continue;
                    }

                    navigator = navigator.SelectSingleNode("/INSTANCE/PROPERTY[@NAME='Data']/VALUE/child::text()");
                    item.GuestOs = navigator != null ? navigator.Value : "unknown";

                    //Debug.Print("Virtual machine {0} OS is: {1}", vm["ElementName"].ToString(), navigator.Value);
                    //                            break;

                }
            }




            result.Add(item);



            //// display VM details
            //Debug.Print("\nName: {0}\nStatus: {1}\nDescription: {2}\n",
            //    vm["ElementName"].ToString(),
            //    vm["EnabledState"].ToString(),
            //    vm["Description"].ToString());
        }
    }


    static bool VmRunning(ManagementObject vm)
    {
        const int Enabled = 2;

        var running = false;

        foreach (var operationStatus in (UInt16[])vm["OperationalStatus"])
        {
            if (operationStatus == Enabled)
            {
                running = true;
                break;
            }
        }

        return running;
    }

}