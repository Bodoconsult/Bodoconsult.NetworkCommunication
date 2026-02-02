using System.IO;
using System.Xml;

namespace Bodoconsult.Inventory.Interfaces;

/// <summary>
/// Provide WMI data for a network item
/// </summary>
public interface IWmiNetworkItemDataProvider
{
    /// <summary>
    /// Get metadata for the network item
    /// </summary>
    void GetMetaData();
    /// <summary>
    /// Get hardware for the network item
    /// </summary>
    void GetHardware();
    /// <summary>
    /// Get metadata for the network item
    /// </summary>
    void GetScheduledJobs();
    /// <summary>
    /// Get data for virtual machines for the network item
    /// </summary>
    void GetVmData();
    /// <summary>
    /// Get software items for the network item
    /// </summary>
    void GetSoftware();
    /// <summary>
    /// Get operating system info for the network item
    /// </summary>
    void GetOperatingSystem();
    /// <summary>
    /// Get info about computer system for the network item
    /// </summary>
    void GetComputerSystem();
    /// <summary>
    /// Get info about installed software for the network item
    /// </summary>
    void GetInstalledSoftware();
    /// <summary>
    /// Get info about shared drives for the network item
    /// </summary>
    void GetShares();
    /// <summary>
    /// Get info about other drives for the network item
    /// </summary>
    void GetOtherDrives();
    /// <summary>
    /// Get info about physical memory for the network item
    /// </summary>
    void GetPhysicalMemory();

    /// <summary>
    /// Get info about sound cards for the network item
    /// </summary>
    void GetSoundAdapters();
    /// <summary>
    /// Get info about network adapter for the network item
    /// </summary>
    void GetNetworkAdapters();
    /// <summary>
    /// Get info about the base board for the network item
    /// </summary>
    void GetBaseBoard();
    /// <summary>
    /// Get BIOS info for the network item
    /// </summary>
    void GetBios();
    /// <summary>
    /// Get processor info for the network item
    /// </summary>
    void GetProcessors();
    /// <summary>
    /// Get video adapters info for the network item
    /// </summary>
    void GetVideoAdapters();
    /// <summary>
    /// Get info about drives for the network item
    /// </summary>
    void GetDrives();
    /// <summary>
    /// Get info about logical info for the network item
    /// </summary>
    void GetLogicalDrives();
    Stream GetStream();
    string LocalIp { get; set; }
    bool Error { get; set; }
    string WmiPath { get; set; }
    string Ip { get; set; }
    string Username { get; set; }
    string Password { get; set; }
    string XmlFileName { get; set; }
    string HostName { get; set; }
    string Domain { get; set; }
    bool VirtualMachine { get; set; }
    string VirtualMachineHost { get; set; }
    void AddName(XmlNode node, string content);
    void CreateSubNodes(string root, string mainNodeCaption, string wql, string caption);
    void CreateSubNodes(string root, string mainNodeCaption, string wql, string caption, string subWql, string fieldName, string subItemName);
    void Start(string rootElement);
    void Save();
    XmlNodeList GetNodes(string section, string subsection);
    XmlNode GetFirstNode(string section, string subsection);
}