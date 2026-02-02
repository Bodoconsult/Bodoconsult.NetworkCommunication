using System.Xml;

namespace Bodoconsult.Inventory.Interfaces;

public interface IWmiDomainDataProvider
{

    /// <summary>
    /// Get metadata for the domain item
    /// </summary>
    void GetMetaData();

    void GetUsers();
    void GetGroups();
    void GetUsers(XmlNode node, string groupName);
    void GetComputers();
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
    void Start(string rootElement);
    void Save();
    XmlNodeList GetNodes(string section, string subsection);
    XmlNode GetFirstNode(string section, string subsection);
}