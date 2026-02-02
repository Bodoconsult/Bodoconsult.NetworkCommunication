using System.Diagnostics;
using System.IO;
using Bodoconsult.Inventory.Helper;

namespace Bodoconsult.Inventory.Model;

/// <summary>
/// General settings for Bodoconsult.Inventory
/// </summary>
public class GeneralSettings
{
    /// <summary>
    /// default ctor
    /// </summary>
    public GeneralSettings()
    {
        DiskLimitAbsolut = 100;
        DiskLimitRelative = 0.2;
        _localIp = NetworkHelper.GetLocalIpAddress();

        var dir = FileHelper.JsonDir;
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

        dir = FileHelper.ProfileDir;
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

        dir = FileHelper.MibPath;
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
    }

    /// <summary>
    /// Minimum disk space limit in GB
    /// </summary>
    public double DiskLimitAbsolut { get; set; }

    /// <summary>
    /// Minimum disk space limit in percent of disk space
    /// </summary>
    public double DiskLimitRelative { get; set; }


    /// <summary>
    /// Username for WMI access
    /// </summary>
    public string Username { get; set; }
    /// <summary>
    /// Password for WMI access
    /// </summary>
    public string Password { get; set; }


    private string _htmlTarget;

    /// <summary>
    /// Target directory for HTML files of the documentation website
    /// </summary>
    /// <summary>
    /// Folder for the HTML export files. Sets <see cref="XmlTargetDir"/> automatically to subfolder \Xml.
    /// </summary>
    public string HtmlTargetDir
    {

        get { return _htmlTarget; }
        set
        {


            _htmlTarget = value;
            if (!Directory.Exists(_htmlTarget)) Directory.CreateDirectory(_htmlTarget);

            XmlTargetDir = Path.Combine(value, @"Xml");
            if (!Directory.Exists(XmlTargetDir)) Directory.CreateDirectory(XmlTargetDir);

            InfrastructureXmlFile = Path.Combine(XmlTargetDir, "virtualinfrastructure.xml");
        }
    }


    /// <summary>
    /// Local IP address (internal)
    /// </summary>
    private readonly string _localIp;

    /// <summary>
    /// Local IP address
    /// </summary>
    public string LocalIp
    {
        get { return _localIp; }
    }


    /// <summary>
    /// File name of the homapage
    /// </summary>
    public string HomepageFileName
    {
        get { return Path.Combine(_htmlTarget, "index.htm"); }
    }


    /// <summary>
    /// Target directory for HTML files of the documentation website
    /// </summary>
    public string XmlTargetDir { get; private set; }



    /// <summary>
    /// Name of softwares to be installed on all machines separated by semicolon
    /// </summary>
    public string SoftwareAllClients { get; set; }


    /// <summary>
    /// IP addresses to be excluded separated by semicolon
    /// </summary>
    public string ExcludeIp { get; set; }

    /// <summary>
    /// IP ranges to check. Default: Class-C-network of the domain controller, i.e. 192.168.12.{0}
    /// </summary>
    public string IpRanges { get; set; }

    /// <summary>
    /// Path for the infrastructure XML file
    /// </summary>
    public string InfrastructureXmlFile { get; private set; }


    /// <summary>
    /// Send a mail message
    /// </summary>
    public MailStatusMessage MailStatusMessage;

    /// <summary>
    /// Sends a status message
    /// </summary>
    public UiStatusMessage StatusMessage;

}