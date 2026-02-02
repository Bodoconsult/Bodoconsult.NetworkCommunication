using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Bodoconsult.Inventory.Enums;
using Newtonsoft.Json;

namespace Bodoconsult.Inventory.Model;

/// <summary>
/// Represents a computer, a laptop, a notebook or a server 
/// </summary>
[DataContract(Namespace = "http://bodoconsult/inventory")]
public class NetworkItem
{
    private string _hostName;
    private int _installYear;
    private DateTime _installDate;

    /// <summary>
    /// default ctor
    /// </summary>
    public NetworkItem()
    {
        HostName = "unknown";
        OperatingSystem = "unknown";
        VirtualMachine = false;
        Software = new List<SoftwareItem>();
        NetworkAdapters = new List<NetworkAdapterItem>();
        Drives = new List<DriveItem>();
        LogicalDrives = new List<LogicalDriveItem>();
        DomainRole = "Client";
        Roles = new List<RoleItem>();
        Shares = new List<ShareItem>();
        IpAddresses = new List<string>();
        ItemType = NetworkItemType.Unknown;
        Warnings = new List<Warning>();
    }

    /// <summary>
    /// type of network item (unknown, Windows, Others)
    /// </summary>
    [DataMember]
    public NetworkItemType ItemType { get; set; }



    [DataMember]
    public IList<Warning> Warnings{ get; private set; }


    /// <summary>
    /// IP addresses of the network item
    /// </summary>
    [DataMember]
    public List<string> IpAddresses { get; private set; }

    [DataMember]
    public string HostName
    {
        get { return (string.IsNullOrEmpty(_hostName)) ? IpAddresses[0] : _hostName; }
        set { _hostName = value; }
    }

    /// <summary>
    /// Item is available in AD computers
    /// </summary>
    [JsonIgnore]
    public bool IsAdObject { get; set; }

    /// <summary>
    /// Item is pingable
    /// </summary>
    [JsonIgnore]
    public bool IsPingable { get; set; }

    /// <summary>
    /// Item is accessible via WMI
    /// </summary>
    [JsonIgnore]
    public bool IsWmiObject { get; set; }

    /// <summary>
    /// Item is accessible via CIM
    /// </summary>
    [JsonIgnore]
    public bool IsCimObject { get; set; }


    /// <summary>
    /// Path to the XML documentation data file
    /// </summary>
    public string XmlFile { get; set; }

    public string DetailFile { get; set; }

    public string SummaryFile { get; set; }

    /// <summary>
    /// Get default IP address of the network item
    /// </summary>
    [DataMember]
    public string Ip
    {
        get
        {
            if (IpAddresses != null && IpAddresses.Count > 0)
            {
                return IpAddresses[0];
            }

            return null;
        }

        set { AddIp(value); }
    }


    [DataMember]
    public string DomainRole { get; set; }


    public int DomainRoleId { get; set; }


    [DataMember]
    public DateTime InstallDate
    {
        get { return _installDate; }
        set
        {
            _installDate = value;
            _installYear = _installDate.Year;
        }
    }

    public int InstallYear
    {
        get { return _installYear; }
        set { _installYear = value; }
    }


    [DataMember]
    public int NumberOfProcessors { get; set; }

    [DataMember]
    public bool VirtualMachine { get; set; }

    [DataMember]
    public string VmHost { get; set; }

    [DataMember]
    public string Manufacturer { get; set; }

    [DataMember]
    public string SerialNumber { get; set; }


    [DataMember]
    public string Model { get; set; }


    [DataMember]
    public string OperatingSystem { get; set; }

    [DataMember]
    public List<SoftwareItem> Software { get; private set; }

    [DataMember]
    public long Ram { get; set; }

    [DataMember]
    public long FreeRam { get; set; }

    [DataMember]
    public List<DriveItem> Drives { get; private set; }

    [DataMember]
    public List<ShareItem> Shares { get; private set; }

    /// <summary>
    /// Built-in networl adapters of the network item
    /// </summary>
    [DataMember]
    public List<NetworkAdapterItem> NetworkAdapters { get; private set; }

    /// <summary>
    /// Network roles of the nework item, i.e. Domaincontroller
    /// </summary>
    [DataMember]
    public List<RoleItem> Roles { get; private set; }


    /// <summary>
    /// Logigal drives of the nework item
    /// </summary>
    [DataMember]
    public List<LogicalDriveItem> LogicalDrives { get; private set; }

    /// <summary>
    /// Remark for the network items
    /// </summary>
    [DataMember]
    public string Remark { get; set; }

    [DataMember]
    public string RoleDescription { get; set; }

    [DataMember]
    public string HandlingInstructions { get; set; }

    [DataMember]
    public string DnsHostName { get; set; }

    /// <summary>
    /// Is the network item a domain controller
    /// </summary>
    [DataMember]
    public bool IsDomainController { get; set; }

    /// <summary>
    /// Is the network item the primary domain controller
    /// </summary>
    [DataMember]
    public bool IsPrimaryDomainController { get; set; }


    /// <summary>
    /// Add an IP address zu the network item
    /// </summary>
    /// <param name="ipAddress">IP address</param>
    public void AddIp(string ipAddress)
    {
        if (IpAddresses.Any(x => x == ipAddress)) return;
        IpAddresses.Add(ipAddress);
    }

    /// <summary>
    /// Username for accessing the network item via WMI
    /// </summary>
    public string Username { get; set; }



    private string _password;

    /// <summary>
    /// Password for accessing the network item via WMI
    /// </summary>
    public string Password
    {
        get { return _password; }
        set { _password = value; }
    }


    //internal string GetPassword()
    //{
    //    return _password;
    //}

    /// <summary>
    /// Is it a HyperV host?
    /// </summary>
    public bool HyperVHost { get; set; }

    /// <summary>
    /// Is it a VMWare host?
    /// </summary>
    public bool VmwareHost { get; set; }


    /// <summary>
    /// IP address for sorting. I.e. IP address 192.168.10.1 will be delivered as 192.168.010.001
    /// </summary>
    public string IpForSorting
    {
        get
        {
            if (IpAddresses.Count == 0) return null;
            var ip = "";
            var s = IpAddresses[0].Split('.');

            foreach (var part in s)
            {
                ip += part.PadLeft(3, '0')+".";
            }

            return ip.Substring(0, ip.Length - 1);
        }
    }
}