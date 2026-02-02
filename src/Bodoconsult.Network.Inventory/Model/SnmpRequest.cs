using System.Collections.Generic;
using Bodoconsult.Snmp.Models;

namespace Bodoconsult.Inventory.Model;

/// <summary>
/// Setup for a SNMP request to a non-Windows network item
/// </summary>
public class SnmpRequest: SnmpBaseRequest
{
    /// <summary>
    /// default ctor
    /// </summary>
    public SnmpRequest(): base()
    {

        SnmpRequestItems = new List<SnmpRequestItem>();
        Roles = new List<string>();
        Warnings = new List<Warning>();
    }


    /// <summary>
    /// Name of the SNMP profile to use (may be empty)
    /// </summary>
    public string ProfileName { get; set; }


    /// <summary>
    /// Network roles of the network item
    /// </summary>
    public IList<string> Roles { get; private set; }


    /// <summary>
    /// Warnings resulting from SNMP request by comparing with a SNMP profile
    /// </summary>
    public IList<Warning> Warnings { get; private set; }

    /// <summary>
    /// Item to query from the SNMP target
    /// </summary>
    public IList<SnmpRequestItem> SnmpRequestItems { get; set; }



    /// <summary>
    /// Base oid of company: some thing like 1.3.6.1.4.1.6574 for Synology
    /// </summary>
    public string CompanyBaseOid { get; set; }


    /// <summary>
    /// Get a default JSON file name for the SNMP request
    /// </summary>
    /// <returns></returns>
    public string GetDefaultJsonFileName()
    {
        var f = $"SNMP_{IpAddresses[0].Replace(".", "_")}.json";
        return f;
    }

}