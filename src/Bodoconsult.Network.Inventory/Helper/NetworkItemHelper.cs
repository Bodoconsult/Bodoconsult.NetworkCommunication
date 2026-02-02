using System.IO;
using Bodoconsult.Inventory.Model;

namespace Bodoconsult.Inventory.Helper;

public class NetworkItemHelper
{
    /// <summary>
    /// Get the xml file path for a network item
    /// </summary>
    /// <param name="xmlPath"></param>
    /// <param name="ip"></param>
    /// <returns></returns>
    public static string GetXmlFilePath(string xmlPath, string ip)
    {
        return Path.Combine(xmlPath, $"result-{ip.Replace(".", "_")}.xml");
    }

    /// <summary>
    /// Check all paths for all network items
    /// </summary>
    /// <param name="network"></param>
    /// <param name="xmlTarget"></param>
    /// <param name="htmlTarget"></param>
    public static void CheckAllNetworkItemPaths(Network network, string xmlTarget, string htmlTarget)
    {
        foreach (var item in network.NetworkItems)
        {
            var ip = item.IpAddresses[0].Replace(".", "_");
            if (string.IsNullOrEmpty(item.XmlFile)) item.XmlFile = Path.Combine(xmlTarget, $"result-{ip}.xml");
            item.DetailFile = Path.Combine(htmlTarget, $"{ip}.htm");
            item.SummaryFile = Path.Combine(htmlTarget, $"{ip}_summary.htm");
        }
    }
}