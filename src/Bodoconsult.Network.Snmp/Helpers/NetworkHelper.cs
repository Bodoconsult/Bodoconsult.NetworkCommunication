using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Bodoconsult.Snmp.Helpers
{
    /// <summary>
    /// Helper class for network related stuff
    /// </summary>
    internal class NetworkHelper
    {

        private static string _ipAddress;

        /// <summary>
        /// Get the local IP address
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIpAddress()
        {
            if (!string.IsNullOrEmpty(_ipAddress)) return _ipAddress;

            _ipAddress = Dns.GetHostEntry(Dns.GetHostName())
                .AddressList.First(
                    o =>
                        o.AddressFamily == AddressFamily.InterNetwork && !o.IsIPv6LinkLocal &&
                        !o.IsIPv6Multicast && !o.IsIPv6SiteLocal && !o.IsIPv6Teredo)
                .ToString();

            return _ipAddress;
        }


        /// <summary>
        /// Get the IP address for a hostname
        /// </summary>
        /// <returns></returns>
        public static string GetIpAddress(string hostname)
        {
            return Dns.GetHostEntry(hostname.Replace("\\", ""))
                .AddressList.First(
                    o =>
                        o.AddressFamily == AddressFamily.InterNetwork && !o.IsIPv6LinkLocal &&
                        !o.IsIPv6Multicast && !o.IsIPv6SiteLocal && !o.IsIPv6Teredo)
                .ToString();
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
            catch
            {
                // ignored
            }

            return host == null ? null : host.HostName;
        }


        /// <summary>
        /// Ip-Adresse anpingen
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public static PingReply Ping(string ipAddress)
        {

            try
            {
                var p = new Ping().Send(ipAddress, 700);

                return p;
            }
            catch
            {
                return null;
            }
        }
    }
}