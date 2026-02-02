using System;
using System.IO;
using System.Xml;
using Bodoconsult.Inventory.Interfaces;

namespace Bodoconsult.Inventory.Provider;

public class WmiNetworkItemDataProvider : WmiBaseProvider, IWmiNetworkItemDataProvider
{

    public WmiNetworkItemDataProvider()
    {

        WmiPath = "\\\\{0}\\root\\cimv2";

    }


    /// <summary>
    /// Get metadata for the network item
    /// </summary>
    public void GetMetaData()
    {

        if (Error) return;

        Xmlc.CheckNode(
            "NetworkItemRoot/NetworkItemSection[Metadata]/NetworkItemSubSection[Current]/NetworkItemItem/NetworkItemValue[IpAddress]",
            Ip);

        Xmlc.CheckNode(
            "NetworkItemRoot/NetworkItemSection[Metadata]/NetworkItemSubSection[Current]/NetworkItemItem/NetworkItemValue[Hostname]",
            HostName);
        Xmlc.CheckNode(
            "NetworkItemRoot/NetworkItemSection[Metadata]/NetworkItemSubSection[Current]/NetworkItemItem/NetworkItemValue[Date]",
            DateTime.Now.ToString("dd.MM.yyyy"));

    }


    /// <summary>
    /// Get hardware for the network item
    /// </summary>
    public void GetHardware()
    {

        if (Error) return;

        Xmlc.CheckNode("NetworkItemRoot/NetworkItemSection[Hardware]", null);

        GetBaseBoard();
        GetBios();
        GetProcessors();
        GetVideoAdapters();
        GetSoundAdapters();

        GetPhysicalMemory();
        GetDrives();
        GetLogicalDrives();
        GetOtherDrives();


        GetNetworkAdapters();

    }

    /// <summary>
    /// Get metadata for the network item
    /// </summary>
    public void GetScheduledJobs()
    {
        CreateSubNodes("NetworkItemRoot/NetworkItemSection[Software]", "ScheduledJobs", "select * from Win32_ScheduledJobs", "Scheduled job {0}");
    }


    /// <summary>
    /// Get data for virtual machines for the network item
    /// </summary>
    public void GetVmData()
    {
        Xmlc.CheckNode(
            "NetworkItemRoot/NetworkItemSection[Metadata]/NetworkItemSubSection[Current]/NetworkItemItem/NetworkItemValue[VirtualMachine]",
            VirtualMachine.ToString());

        Xmlc.CheckNode(
            "NetworkItemRoot/NetworkItemSection[Metadata]/NetworkItemSubSection[Current]/NetworkItemItem/NetworkItemValue[VirtualMachineHost]",
            VirtualMachineHost);
    }


    /// <summary>
    /// Get software items for the network item
    /// </summary>
    public void GetSoftware()
    {
        if (Error) return;

        Xmlc.CheckNode("NetworkItemRoot/NetworkItemSection[Software]", null);

        GetOperatingSystem();
        GetComputerSystem();
        GetInstalledSoftware();
        GetShares();
        GetScheduledJobs();

    }

    /// <summary>
    /// Get operating system info for the network item
    /// </summary>
    public void GetOperatingSystem()
    {
        CreateSubNodes("NetworkItemRoot/NetworkItemSection[Software]", "OperatingSystem", "select * from Win32_OperatingSystem", "Operating system {0}");
    }

    /// <summary>
    /// Get info about computer system for the network item
    /// </summary>
    public void GetComputerSystem()
    {
        CreateSubNodes("NetworkItemRoot/NetworkItemSection[Software]", "ComputerSystem", "select * from Win32_ComputerSystem", "Computer system {0}");
    }


    /// <summary>
    /// Get info about installed software for the network item
    /// </summary>
    public void GetInstalledSoftware()
    {
        CreateSubNodes("NetworkItemRoot/NetworkItemSection[Software]", "InstalledSoftware", "select * from Win32_Product", "Software {0}");
    }


    /// <summary>
    /// Get info about shared drives for the network item
    /// </summary>
    public void GetShares()
    {
        CreateSubNodes("NetworkItemRoot/NetworkItemSection[Software]", "Shares", "select * from Win32_Share",
            "Share {0}");
    }

    /// <summary>
    /// Get info about other drives for the network item
    /// </summary>
    public void GetOtherDrives()
    {
        CreateSubNodes("NetworkItemRoot/NetworkItemSection[Hardware]", "CDROM", "select * from Win32_CDROMDrive", "CDROM {0}");
        CreateSubNodes("NetworkItemRoot/NetworkItemSection[Hardware]", "Floppy", "select * from Win32_FloppyDrive", "Floppy {0}");
        CreateSubNodes("NetworkItemRoot/NetworkItemSection[Hardware]", "Tape", "select * from Win32_TapeDrive", "Tape {0}");
    }

    /// <summary>
    /// Get info about physical memory for the network item
    /// </summary>
    public void GetPhysicalMemory()
    {
        CreateSubNodes("NetworkItemRoot/NetworkItemSection[Hardware]", "PhysicalMemory", "select * from Win32_PhysicalMemory", "PhysicalMemory {0}");
    }

    /// <summary>
    /// Get info about sound cards for the network item
    /// </summary>
    public void GetSoundAdapters()
    {
        CreateSubNodes("NetworkItemRoot/NetworkItemSection[Hardware]", "SoundDevice", "select * from Win32_SoundDevice", "Sound device {0}");
    }

    /// <summary>
    /// Get info about network adapter for the network item
    /// </summary>
    public void GetNetworkAdapters()
    {
        CreateSubNodes("NetworkItemRoot/NetworkItemSection[Hardware]", "NetworkAdapters", "select * from Win32_NetworkAdapter", "Network adapter {0}", "select * from Win32_NetworkAdapterConfiguration where Index={0}", "Index", "NetworkConfiguration");
    }

    /// <summary>
    /// Get info about the base board for the network item
    /// </summary>
    public void GetBaseBoard()
    {
        CreateSubNodes("NetworkItemRoot/NetworkItemSection[Hardware]", "BaseBoard", "select * from Win32_BaseBoard", "Base Board {0}");
    }


    /// <summary>
    /// Get BIOS info for the network item
    /// </summary>
    public void GetBios()
    {
        CreateSubNodes("NetworkItemRoot/NetworkItemSection[Hardware]", "Bios", "select * from Win32_BIOS", "Bios {0}");
    }

    /// <summary>
    /// Get processor info for the network item
    /// </summary>
    public void GetProcessors()
    {
        CreateSubNodes("NetworkItemRoot/NetworkItemSection[Hardware]", "Processors", "select * from Win32_Processor", "Processor {0}");
    }

    /// <summary>
    /// Get video adapters info for the network item
    /// </summary>
    public void GetVideoAdapters()
    {
        CreateSubNodes("NetworkItemRoot/NetworkItemSection[Hardware]", "VideoAdapters", "select * from Win32_VideoController", "Video adapter {0}");
    }

    /// <summary>
    /// Get info about drives for the network item
    /// </summary>
    public void GetDrives()
    {
        CreateSubNodes("NetworkItemRoot/NetworkItemSection[Hardware]", "DiskDrives", "select * from Win32_DiskDrive", "Disk drive {0}");
    }


        

    public new void AddName(XmlNode node, string content)
    {
        var attr = Xmlc.Xml.CreateAttribute("name");
        attr.Value = content;
        node.Attributes.Append(attr);
    }


    /// <summary>
    /// Get info about logical info for the network item
    /// </summary>
    public void GetLogicalDrives()
    {
        CreateSubNodes("NetworkItemRoot/NetworkItemSection[Hardware]", "LogicalDrives", "select * from Win32_LogicalDisk where DriveType=3", "Logical drive {0}");
    }



    public Stream GetStream()
    {
        Stream stream = new MemoryStream();
        Xmlc.Xml.Save(stream);
        stream.Position = 0;
        return stream;

    }
}