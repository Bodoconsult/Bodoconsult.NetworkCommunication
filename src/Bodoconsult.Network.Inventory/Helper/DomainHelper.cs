using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.DirectoryServices.ActiveDirectory;
using System.Globalization;
using System.Runtime.InteropServices;
using Bodoconsult.Inventory.Enums;
using Bodoconsult.Inventory.Model;

namespace Bodoconsult.Inventory.Helper;

/// <summary>
/// Helper to get current domain data
/// Thanks to http://pinvoke.net/default.aspx/netapi32/DsGetDcName.html
/// </summary>
public class DomainHelper
{
    /// <summary>
    /// default ctor: collects current domain data
    /// </summary>
    public DomainHelper()
    {
        var domain = GetDomainInfo();
        DomainControllerName = domain.DomainControllerName.Replace("\\", "");
        DnsForestName = domain.DnsForestName;
        DomainControllerSiteName = domain.DcSiteName;
        ClientSiteName = domain.ClientSiteName;
        DomainName = domain.DomainName;
        DomainControllerIpAddress = NetworkHelper.GetIpAddress(domain.DomainControllerName);
    }


    /// <summary>
    /// Name of the currently used domain controller
    /// </summary>
    public string DomainControllerName { get; set; }

    /// <summary>
    /// DNS forest name
    /// </summary>
    public string DnsForestName { get; set; }

    /// <summary>
    /// Domain controller site name
    /// </summary>
    public string DomainControllerSiteName { get; set; }


    /// <summary>
    /// Client site name
    /// </summary>
    public string ClientSiteName { get; set; }


    /// <summary>
    /// IP address of the domain controller
    /// </summary>
    public string DomainControllerIpAddress { get; set; }


    /// <summary>
    /// Name of the domain
    /// </summary>
    public string DomainName { get; set; }


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DomainControllerInfo
    {
        [MarshalAs(UnmanagedType.LPTStr)]
        public readonly string DomainControllerName;
        [MarshalAs(UnmanagedType.LPTStr)]
        public readonly string DomainControllerAddress;
        public readonly uint DomainControllerAddressType;
        public readonly Guid DomainGuid;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string DomainName;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string DnsForestName;
        public uint Flags;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string DcSiteName;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string ClientSiteName;
    }

    [DllImport("Netapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int DsGetDcName
    (
        [MarshalAs(UnmanagedType.LPTStr)]
        string computerName,
        [MarshalAs(UnmanagedType.LPTStr)]
        string domainName,
        [In] int domainGuid,
        [MarshalAs(UnmanagedType.LPTStr)]
        string siteName,
        [MarshalAs(UnmanagedType.U4)]
        DsgetdcnameFlags flags,
        out IntPtr pDomainControllerInfo
    );

    [DllImport("Netapi32.dll", SetLastError = true)]
    private static extern int NetApiBufferFree(IntPtr buffer);

    [Flags]
    public enum DsgetdcnameFlags : uint
    {
        DsForceRediscovery = 0x00000001,
        DsDirectoryServiceRequired = 0x00000010,
        DsDirectoryServicePreferred = 0x00000020,
        DsGcServerRequired = 0x00000040,
        DsPdcRequired = 0x00000080,
        DsBackgroundOnly = 0x00000100,
        DsIpRequired = 0x00000200,
        DsKdcRequired = 0x00000400,
        DsTimeservRequired = 0x00000800,
        DsWritableRequired = 0x00001000,
        DsGoodTimeservPreferred = 0x00002000,
        DsAvoidSelf = 0x00004000,
        DsOnlyLdapNeeded = 0x00008000,
        DsIsFlatName = 0x00010000,
        DsIsDnsName = 0x00020000,
        DsReturnDnsName = 0x40000000,
        DsReturnFlatName = 0x80000000
    }

    private static DomainControllerInfo GetDomainInfo()
    {
        DomainControllerInfo domainInfo;
        const int errorSuccess = 0;
        var pDci = IntPtr.Zero;
        try
        {
            var val = DsGetDcName("", "", 0, "",
                DsgetdcnameFlags.DsDirectoryServiceRequired |
                DsgetdcnameFlags.DsReturnDnsName |
                DsgetdcnameFlags.DsIpRequired, out pDci);
            //check return value for error
            if (errorSuccess == val)
            {
                domainInfo = (DomainControllerInfo)Marshal.PtrToStructure(pDci, typeof(DomainControllerInfo));
            }
            else
            {
                throw new Win32Exception(val);
            }
        }
        finally
        {
            NetApiBufferFree(pDci);
        }
        return domainInfo;
    }

    /// <summary>
    /// Get a list of all domain controllers
    /// </summary>
    public IList<NetworkItem> GetListOfDomainControllers()
    {
        try
        {

            var computers = new List<NetworkItem>();

            var domain = Domain.GetCurrentDomain();

                

            foreach (DomainController dc in domain.DomainControllers)
            {
                var ni = new NetworkItem
                {
                    HostName = dc.Name,
                    IsDomainController = true,
                    ItemType = NetworkItemType.Windows,
                    DomainRoleId=4,
                    DomainRole=TranslateDomainRole("4")
                };

                if (dc.IPAddress == domain.PdcRoleOwner.IPAddress)
                {
                    ni.IsPrimaryDomainController = true;
                    ni.DomainRoleId = 5;
                    ni.DomainRole = TranslateDomainRole("5");
                }

                ni.AddIp(dc.IPAddress);

                computers.Add(ni);

                //Debug.Print("Name: " + dc.Name);
                //Debug.Print("Operating Systme: " + dc.OSVersion);
                //Debug.Print("IP Address: " + dc.IPAddress);
                //Debug.Print("Site Name: " + dc.SiteName);
            }


            return computers;

            //DirectoryEntry domainEntry = new DirectoryEntry("LDAP://DC=Work2008,DC=local", "Administrator", "password");
            //DirectorySearcher searcher = new DirectorySearcher(domainEntry)
            //{
            //    Filter = "(&(objectCategory=computer)(objectClass=computer)(userAccountControl:1.2.840.113556.1.4.803:=8192))"
            //};

            //searcher.PropertiesToLoad.AddRange(new[] { "name", "operatingsystem" });

            //foreach (SearchResult result in searcher.FindAll())
            //{
            //    Console.WriteLine("Name: " + result.Properties["name"][0]);
            //    Console.WriteLine("Operating Systme: " + result.Properties["operatingsystem"][0]);
            //}
        }
        catch //(Exception ex)
        {
            return null;
        }
    }

    /// <summary>
    /// Transfer the domain data code to a clear text role
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public static string TranslateDomainRole(string code)
    {

        switch (Convert.ToInt32(code))
        {
            case 0:
                return "Standalone Workstation";
            case 1:
                return "Member Workstation";
            case 2:
                return "Standalone Server";
            case 3:
                return "Member Server";
            case 4:
                return "Backup Domain Controller";
            case 5:
                return "Primary Domain Controller";
            default:
                return "Unknown (" + code.ToString(CultureInfo.InvariantCulture) + ")";
        }
    }

}