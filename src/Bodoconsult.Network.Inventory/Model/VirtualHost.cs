using System.Collections.Generic;
using System.Linq;
using System.Net;
using Vestris.VMWareLib;
using System.Runtime.Serialization;
using Bodoconsult.Inventory.Handler;
using Newtonsoft.Json;

namespace Bodoconsult.Inventory.Model;

[DataContract(Namespace = "http://bodoconsult/inventory")]
public class VirtualHost
{

    [DataMember]
    public string Url { get; set; }

    [JsonIgnore]
    public string UserName { get; set; }

    [JsonIgnore]
    public string Password { get; set; }

    [DataMember]
    public VirtualMachineType HostType { get; set; }

    [DataMember]
    public List<VirtualMachine> VirtualMachines { get; set; }

    /// <summary>
    /// Domain for the <see cref="UserName"/>. Leave empty if <see cref="UserName"/> is a local account not an domain account
    /// </summary>
    public string Domain { get; set; }


    public VirtualHost()
    {
        VirtualMachines = new List<VirtualMachine>();
    }

    /// <summary>
    /// Returns a new or existing virtual machine object by its name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public VirtualMachine AddOrUpdateVirtualMachine(string name)
    {
        var item = VirtualMachines.FirstOrDefault(x => x.Name == name);

        if (item != null)
        {
            return item;
        }

        item = new VirtualMachine {Name = name};
        VirtualMachines.Add(item);

        return item;
    }

    /// <summary>
    /// Returns a new or existing virtual machine object by its name
    /// </summary>
    /// <param name="vm">virtual machine</param>
    /// <returns></returns>
    public VirtualMachine AddOrUpdateVirtualMachine(VirtualMachine vm)
    {
        var item = VirtualMachines.FirstOrDefault(x => x.Name == vm.Name);

        if (item != null)
        {
            return item;
        }

        item = vm;
        VirtualMachines.Add(item);
        return item;
    }

    public void GetVirtualMachines()
    {

        if (string.IsNullOrEmpty(Url))
        {
            return;
        }

        if (HostType == VirtualMachineType.HyperV)
        {
            GetHypervVirtualMachines();
            return;
        }

        GetVmWareVirtualMachines();
    }

    private void GetHypervVirtualMachines()
    {
        var hh = new HypervHandler
        {
            Domain = Domain,
            Username = UserName,
            Password = Password,
            IpAddress = Url
        };

        hh.Connect();

        var result = new  List<VirtualMachine>();

        hh.GetVirtualMachines(result);

        foreach (var vm in result)
        {
            var data = AddOrUpdateVirtualMachine(vm);
            data.HostName = vm.HostName;
            data.GuestOs = vm.GuestOs;
            data.Ip = vm.Ip;
        }
    }

    private void GetVmWareVirtualMachines()
    {
        var virtualHost = new VMWareVirtualHost();
        virtualHost.ConnectToVMWareVIServer(Url, UserName, Password);

        var machines = virtualHost.RegisteredVirtualMachines;

        foreach (var machine in machines)
        {
            var o = AddOrUpdateVirtualMachine(machine.RuntimeConfigVariables["displayName"]);
            o.GuestOs = machine.RuntimeConfigVariables["guestOSAltName"];
            o.Ip = machine.GuestVariables["ip"];
            o.Name = machine.RuntimeConfigVariables["displayName"];
            o.HostName = GetHostNameByIp(o.Ip);
        }


        //var guestOS = new Vestris.VMWareLib.Tools.GuestOS(machine);
        //o.Ip = guestOS.IpAddress;
        //guestOS.Dispose();

        virtualHost.Disconnect();
    }


    /// <summary>
    /// Resolves the host name from the given IP address.
    /// </summary>
    /// <param name="ipAddress">The ip address for getting the host name.</param>
    /// <returns>The host name if exists else "no entry"</returns>
    public static string GetHostNameByIp(string ipAddress)
    {
        IPHostEntry host = null;

        try
        {
            IPAddress ip;
            if (IPAddress.TryParse(ipAddress, out ip))
                host = Dns.GetHostEntry(ip);
        }
        // ReSharper disable EmptyGeneralCatchClause
        catch
            // ReSharper restore EmptyGeneralCatchClause
        {
        }

        return (host == null) ? null : host.HostName;
    }

}