using System.IO;
using Bodoconsult.Inventory.Formatter;
using Bodoconsult.Inventory.Helper;
using Bodoconsult.Inventory.Model;

namespace Bodoconsult.Inventory.Handler;

/// <summary>
/// Create a website documentation for a <see cref="Network"/> object
/// </summary>
public class WebsiteHandler
{

    private readonly Network _network;
    private readonly string _appPath;

    /// <summary>
    /// default ctor
    /// </summary>
    /// <param name="network">network data to publish</param>
    /// <param name="settings">General settings</param>
    public WebsiteHandler(Network network, GeneralSettings settings)
    {
        _network = network;
        _appPath = FileHelper.GetAppPath();
        CurrentSettings = settings;
    }

    /// <summary>
    /// Current settings to use
    /// </summary>
    public GeneralSettings CurrentSettings { get; private set; }

    /// <summary>
    /// Sends status messages
    /// </summary>
    public event UiStatusMessage Status;

    /// <summary>
    /// Create the HTML content for the pages and save it as HTML file
    /// </summary>
    public void CreateHtmlContent()
    {
        var source = Path.Combine(_appPath, "prototypes", "layout.css");

        var target = Path.Combine(CurrentSettings.HtmlTargetDir, "layout.css");

        if (File.Exists(target)) File.Delete(target);
        File.Copy(source, target);

        source = Path.Combine(_appPath, "prototypes", "down.png");
        target = Path.Combine(CurrentSettings.HtmlTargetDir, "down.png");

        if (File.Exists(target)) File.Delete(target);
        File.Copy(source, target);

        source = Path.Combine(_appPath, "prototypes", "right.png");
        target = Path.Combine(CurrentSettings.HtmlTargetDir, "right.png");

        if (File.Exists(target)) File.Delete(target);
        File.Copy(source, target);


        var html = new HtmlWebsiteFormatter(CurrentSettings, _network);

        GotStatus("CreateHtmlContent", "Pages for network items");
        html.PagesForNetworkItems();


        var sp = new StartPage();

        //Warnings.htm

        var item = new StartPageItem
        {
            PageItemType = StartPageItemType.Header2, 
            Title = "Warnings"
        };
        sp.PageItems.Add(item);

        item = new StartPageItem
        {
            PageItemType = StartPageItemType.HtmlContent,
            Title = "Warnings",
            FileName = "Warnings.htm",
            GetContentDelegate = html.Warnings
        };

        sp.PageItems.Add(item);

        item = new StartPageItem
        {
            PageItemType = StartPageItemType.Header2,
            Title = "Domain information"
        };
        sp.PageItems.Add(item);

        item = new StartPageItem
        {
            PageItemType = StartPageItemType.XmlContent,
            Title = "Domain information summary",
            FileName = "Domain_summary.htm",
            GetContentDelegate = html.DomainSummary,
        };

        sp.PageItems.Add(item);

        item = new StartPageItem
        {
            PageItemType = StartPageItemType.XmlContent,
            Title = "Domain information details",
            FileName = "Domain.htm",
            GetContentDelegate = html.DomainDetails
        };

        sp.PageItems.Add(item);

        item = new StartPageItem
        {
            PageItemType = StartPageItemType.XmlContent,
            Title = "Virtualization infrastructure",
            FileName = "VirtualInfrastructure.htm",
            GetContentDelegate = html.CreateInfrastructure
        };

        sp.PageItems.Add(item);


        item = new StartPageItem
        {
            PageItemType = StartPageItemType.HtmlContent,
            Title = "Unknown hosts (DNS server couldn't resolve hosts)",
            FileName = "UnknownHosts.htm",
            GetContentDelegate = html.UnknownHosts
        };

        sp.PageItems.Add(item);

        item = new StartPageItem
        {
            PageItemType = StartPageItemType.HtmlContent,
            Title = "Network items by IP address",
            FileName = "OverviewByIp.htm",
            GetContentDelegate = html.OverviewByIpAddress
        };

        sp.PageItems.Add(item);


        item = new StartPageItem
        {
            PageItemType = StartPageItemType.HtmlContent,
            Title = "Network items by host name",
            FileName = "OverviewByHostname.htm",
            GetContentDelegate = html.OverviewByHostname
        };

        sp.PageItems.Add(item);

        item = new StartPageItem
        {
            PageItemType = StartPageItemType.HtmlContent,
            Title = "Network items by operating system",
            FileName = "OverviewByOs.htm",
            GetContentDelegate = html.OverviewByOs
        };

        sp.PageItems.Add(item);

        item = new StartPageItem
        {
            PageItemType = StartPageItemType.HtmlContent,
            Title = "Network items by installed software",
            FileName = "OverviewBySoftware.htm",
            GetContentDelegate = html.OverviewBySoftware
        };

        sp.PageItems.Add(item);

        item = new StartPageItem
        {
            PageItemType = StartPageItemType.HtmlContent,
            Title = "Network items by installed software (only servers)",
            FileName = "OverviewBySoftwareServer.htm",
            GetContentDelegate = html.OverviewBySoftwareServer

        };

        sp.PageItems.Add(item);

        item = new StartPageItem
        {
            PageItemType = StartPageItemType.HtmlContent,
            Title = "Network items by installed software (only clients)",
            FileName = "OverviewBySoftwareClients.htm",
            GetContentDelegate = html.OverviewBySoftwareClient

        };

        sp.PageItems.Add(item);

        item = new StartPageItem
        {
            PageItemType = StartPageItemType.HtmlContent,
            Title = "Overview by domain role",
            FileName = "OverviewByDomainRole.htm",
            GetContentDelegate = html.OverviewByDomainRole
        };

        sp.PageItems.Add(item);

        item = new StartPageItem
        {
            PageItemType = StartPageItemType.HtmlContent,
            Title = "Overview by network speed",
            FileName = "OverviewByNetworkSpeed.htm",
            GetContentDelegate = html.OverviewByNetworkSpeed
        };

        sp.PageItems.Add(item);


        // security
        item = new StartPageItem
        {
            PageItemType = StartPageItemType.Header2,
            Title = "Domain security"
        };
        sp.PageItems.Add(item);

        item = new StartPageItem
        {
            PageItemType = StartPageItemType.HtmlContent,
            Title = "Disabled domain users",
            FileName = "DomainUserDisabled.htm",
            GetContentDelegate = html.DomainUserDisabled
        };

        sp.PageItems.Add(item);

        //Domain users with password not changable by user
        item = new StartPageItem
        {
            PageItemType = StartPageItemType.HtmlContent,
            Title = "Domain users with password not required",
            FileName = "DomainPwdNotRequired.htm",
            GetContentDelegate = html.DomainPwdNotRequired
        };

        sp.PageItems.Add(item);

        // Domain users with password not changable by user
        item = new StartPageItem
        {
            PageItemType = StartPageItemType.HtmlContent,
            Title = "Domain users with password not changable by user",
            FileName = "DomainPwdNotChangable.htm",
            GetContentDelegate = html.DomainPasswordCantChange

        };

        sp.PageItems.Add(item);

        //Domain users with password not expiring
        item = new StartPageItem
        {
            PageItemType = StartPageItemType.HtmlContent,
            Title = "Domain users with password not expiring",
            FileName = "DomainPwdNotExpiring.htm",
            GetContentDelegate = html.DontExpirePassword

        };

        sp.PageItems.Add(item);

        item = new StartPageItem
        {
            PageItemType = StartPageItemType.XmlContent,
            Title = "All shares with permissions",
            FileName = "DomainAllShares.htm",
            GetContentDelegate = html.AllShares
        };

        sp.PageItems.Add(item);




        //item = new StartPageItem
        //{
        //    PageItemType = StartPageItemType.XmlContent,
        //    Title = "Virtualization infrastructure",
        //    FileName = "VirtualInfrastructure.htm",
        //    GetContentDelegate = html.AllShares
        //};

        //sp.PageItems.Add(item);


        // security
        item = new StartPageItem
        {
            PageItemType = StartPageItemType.Header2,
            Title = "Others"
        };
        sp.PageItems.Add(item);

        item = new StartPageItem
        {
            PageItemType = StartPageItemType.HtmlContent,
            Title = "Missing software on clients",
            FileName = "MissingSoftware.htm",
            GetContentDelegate = html.MissingSoftwareClient

        };

        sp.PageItems.Add(item);

        var sph = new StartPageHandler(sp, CurrentSettings);
        sph.CreateStartPage();
    }



    protected void GotStatus(string modul, string msg)
    {
        //_IsStart = false;
        var x = Status;
        if (x != null) x(modul, msg);


    }
}