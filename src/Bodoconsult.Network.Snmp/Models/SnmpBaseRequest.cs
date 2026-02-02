using System.Collections.Generic;
using Bodoconsult.Inventory.Model;
using SnmpSharpNet;

namespace Bodoconsult.Snmp.Models
{
    /// <summary>
    /// Setup for a SNMP request to a non-Windows network item
    /// </summary>
    public class SnmpBaseRequest
    {
        /// <summary>
        /// default ctor
        /// </summary>
        public SnmpBaseRequest()
        {
            IpAddresses = new List<string>();
            AuthenticationType = AuthenticationDigests.MD5;
            PrivacyProtocols = PrivacyProtocols.None;
            Results = new List<SnmpResult>();
            Oids = new List<string>();
        }

        /// <summary>
        /// All results of the request in details
        /// </summary>
        public IList<SnmpResult> Results { get; set; }

        /// <summary>
        /// IP address of the network item to query via SNMP.
        /// </summary>
        public IList<string> IpAddresses { get; set; }

        /// <summary>
        /// SNMP V3 Authentication type
        /// </summary>
        public AuthenticationDigests AuthenticationType { get; set; }

        /// <summary>
        /// SNMP V3 privacy protocol 
        /// </summary>
        public PrivacyProtocols PrivacyProtocols { get; set; }

        /// <summary>
        /// SNMP V3 username 
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// SNMP V3 password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// List all requested OIDs 
        /// </summary>
        public IList<string> Oids { get; set; }
    }
}
