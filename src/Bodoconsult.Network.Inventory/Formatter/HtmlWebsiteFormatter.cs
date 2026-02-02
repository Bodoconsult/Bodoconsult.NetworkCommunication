using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Bodoconsult.Inventory.Handler;
using Bodoconsult.Inventory.Helper;
using Bodoconsult.Inventory.Model;
using Bodoconsult.Inventory.Util;
using Bodoconsult.Web.Html;

namespace Bodoconsult.Inventory.Formatter;

/// <summary>
/// Creates the HTML for the documentation website
/// </summary>
public class HtmlWebsiteFormatter
{

    private readonly XsltHandler _xsl;

    private static Network _network;

    private readonly string _appPath;

    private readonly GeneralSettings _currentSettings;


    public HtmlWebsiteFormatter(GeneralSettings settings, Network network)
    {
        _currentSettings = settings;
        _network = network;

        _appPath = FileHelper.GetAppPath();
        _xsl = new XsltHandler { XslFile = Path.Combine(_appPath, @"prototypes\AllDetails.xsl") };
        _xsl.Start();
    }

    ///// <summary>
    ///// Create homepage
    ///// </summary>
    //public void Index()
    //{
    //    var html = new HtmlHandler(_htmlMaster);

    //    var prototype = ReadPrototype("Index.prot");
    //    html.Title = "Overview network items";

    //    html.Target = Path.Combine(_currentSettings.HtmlTargetDir, "index.htm");
    //    html.Body = prototype;
    //    html.SaveHtml();
    //}

    /// <summary>
    /// Create unknown hosts page
    /// </summary>
    public string UnknownHosts(StartPageItem item)
    {
        var body = new StringBuilder();

        foreach (var host in _network.UnknownHosts.OrderBy(x => x))
        {
            body.AppendFormat("<p>{0}</p>", host);
        }
        return body.ToString();
    }

    /// <summary>
    /// Create the summary page for the domain
    /// </summary>
    public string DomainDetails(StartPageItem item)
    {
        var source = Path.Combine(_currentSettings.XmlTargetDir, "result_domain.xml");
        var target = Path.Combine(_currentSettings.HtmlTargetDir, item.FileName);
        _xsl.Transform(source, target);
        return null;
    }



    /// <summary>
    /// Create the summary page for the domain
    /// </summary>
    public string DomainSummary(StartPageItem item)
    {
        var dh = new DomainDataHandler(_network.Domain);
        dh.ExportAsHtml(Path.Combine(_currentSettings.HtmlTargetDir, item.FileName));
        dh.ExportAsXml(Path.Combine(_currentSettings.HtmlTargetDir, "domain.xml"));
        return null;
    }


    /// <summary>
    /// Create HTML pages for each network item
    /// </summary>
    public void PagesForNetworkItems()
    {
        foreach (var networkItem in _network.NetworkItems)
        {
            PagesForNetworkItem(networkItem);
        }
    }

    /// <summary>
    /// Create HTML page for a network item
    /// </summary>
    /// <param name="item"></param>
    public void PagesForNetworkItem(NetworkItem item)
    {
        try
        {
            var dh = new NetworkItemDataHandler(item, _xsl);

            if (!string.IsNullOrEmpty(item.SummaryFile)) dh.ExportAsHtml(item.SummaryFile);

            //dh.ExportAsXml();
            if (!string.IsNullOrEmpty(item.DetailFile)) dh.ExportDetailsAsHtml(item.DetailFile);
        }
        catch //(Exception ex)
        {
            item.SummaryFile = null;
        }
    }


    /// <summary>
    /// Overview page by IP address
    /// </summary>
    public string OverviewByIpAddress(StartPageItem item)
    {
        var list = _network.NetworkItems.Where(x => !string.IsNullOrEmpty(x.SummaryFile) && !string.IsNullOrEmpty(x.DetailFile) && x.HostName != x.IpAddresses[0]).OrderBy(x => x.IpForSorting).ToList();

        if (!list.Any())
        {
            return "<p>No data found!</p>";
        }

        var body = new StringBuilder();

        var erg = list.Select(host => new DetailTableRow
        {
            HostAddress = host.HostName,
            IpAddress = host.IpAddresses[0],
            Summary =
                $@"<a href=""{Path.GetFileName(host.SummaryFile)}""><img src=""down.png"" alt=""Go to summary""/></a>",
            Details =
                $@"<a href=""{Path.GetFileName(host.DetailFile)}""><img src=""down.png"" alt=""Go to details""/></a>"
        }).ToList();


        body.Append(GetTable(erg.ToDataTable()));
        return body.ToString();
    }

    /// <summary>
    /// Overview page by hostname
    /// </summary>
    public string OverviewByHostname(StartPageItem item)
    {

        //var prototype = ReadPrototype("OverviewByHostname.prot");

        var list = _network.NetworkItems.Where(x => !string.IsNullOrEmpty(x.SummaryFile) &&
                                                    !string.IsNullOrEmpty(x.DetailFile)
                                                    && x.HostName != x.IpAddresses[0])
            .OrderBy(x => (string.IsNullOrEmpty(x.HostName)) ? x.IpForSorting : x.HostName).ToList();

        if (!list.Any())
        {
            return "<p>No data found!</p>";
        }

        var body = new StringBuilder();

        var erg = list.Select(host => new DetailTableRow
        {
            HostAddress = host.HostName,
            IpAddress = host.IpAddresses[0],
            Summary =
                $@"<a href=""{Path.GetFileName(host.SummaryFile)}""><img src=""down.png"" alt=""Go to summary""/></a>",
            Details =
                $@"<a href=""{Path.GetFileName(host.DetailFile)}""><img src=""down.png"" alt=""Go to details""/></a>"
        }).ToList();


        body.Append(GetTable(erg.ToDataTable()));

        return body.ToString();
    }

    /// <summary>
    /// Overview page by operating system
    /// </summary>
    internal string OverviewByOs(StartPageItem item)
    {

        var list = _network.NetworkItems.Where(x => !string.IsNullOrEmpty(x.SummaryFile) && !string.IsNullOrEmpty(x.DetailFile)
            && x.HostName != x.IpAddresses[0]).GroupBy(x => x.OperatingSystem).ToList();

        //    .OrderBy(x => x.OperatingSystem).ToList();

        var body = new StringBuilder();

        AddGroupView(body, list);

        return body.ToString();

    }


    /// <summary>
    /// Overview page by software
    /// </summary>
    internal string OverviewBySoftware(StartPageItem item)
    {

        var list = (from i in _network.NetworkItems where !string.IsNullOrEmpty(i.SummaryFile) && !string.IsNullOrEmpty(i.DetailFile) from s in i.Software group i by s.Name into e select e).ToList();

        var body = new StringBuilder();

        AddGroupView(body, list);

        return body.ToString();
    }


    /// <summary>
    /// Overview page not via WMI accessible network items
    /// </summary>
    internal string OverviewNoWmiAccess(StartPageItem item)
    {

        var list = (from i in _network.NetworkItems where !string.IsNullOrEmpty(i.SummaryFile) && !string.IsNullOrEmpty(i.DetailFile) from s in i.Software group i by s.Name into e select e).ToList();

        var body = new StringBuilder();

        AddGroupView(body, list);

        return body.ToString();
    }


    /// <summary>
    /// Overview page servers by software
    /// </summary>
    internal string OverviewBySoftwareServer(StartPageItem item)
    {

        var list = (from i in _network.NetworkItems.Where(x => !string.IsNullOrEmpty(x.SummaryFile) && !string.IsNullOrEmpty(x.DetailFile) && (x.DomainRole == "Backup Domain Controller" || x.DomainRole == "Member Server" || x.DomainRole == "Primary Domain Controller")) from s in i.Software group i by s.Name into e select e).ToList();

        var body = new StringBuilder();

        AddGroupView(body, list);

        return body.ToString();

    }

    /// <summary>
    /// Overview page clients by software
    /// </summary>
    internal string OverviewBySoftwareClient(StartPageItem item)
    {

        var list = (from i in _network.NetworkItems.Where(x => !string.IsNullOrEmpty(x.SummaryFile) && !string.IsNullOrEmpty(x.DetailFile) && (x.DomainRole == "Client" || x.DomainRole == "Member Workstation")) from s in i.Software group i by s.Name into e select e).ToList();

        var body = new StringBuilder();

        AddGroupView(body, list);

        return body.ToString();

    }


    /// <summary>
    /// Overview page server by domain role
    /// </summary>
    internal string OverviewByDomainRole(StartPageItem item)
    {

        var list = _network.NetworkItems.Where(x => !string.IsNullOrEmpty(x.SummaryFile) && !string.IsNullOrEmpty(x.DetailFile) && x.HostName != x.IpAddresses[0]).OrderBy(x => x.DomainRole).GroupBy(x => x.DomainRole);

        var body = new StringBuilder();

        AddGroupView(body, list);

        return body.ToString();
    }


    /// <summary>
    /// Overview page by network speed
    /// </summary>
    internal string OverviewByNetworkSpeed(StartPageItem item)
    {

        var list = (from i in _network.NetworkItems where !string.IsNullOrEmpty(i.SummaryFile) && !string.IsNullOrEmpty(i.DetailFile) && i.HostName != i.IpAddresses[0] from s in i.NetworkAdapters where s.Speed > 0 group i by s.Speed.ToString(CultureInfo.InvariantCulture) into e select e).ToList();

        var body = new StringBuilder();

        AddGroupView(body, list);

        return body.ToString();
    }


    private static void AddGroupView(StringBuilder body, IEnumerable<IGrouping<string, NetworkItem>> list)
    {
        var data = list.ToList();
        if (!data.Any())
        {
            body.AppendLine("<p>No data found!</p>");
            return;
        }

        var overview = new StringBuilder();


        body.Append("<h2>Overview</h2>");

        var i = 0;
        var statTable = new List<StatTableRow>();

        foreach (var item in data.OrderBy(x => x.Key))
        {

            overview.AppendFormat(@"<h2 id=""key{1}"">{0}</h2>", item.Key, i);


            var list1 = item.GroupBy(x => x.IpAddresses[0]).OrderBy(x => x.Key).ToList();

            //body.Append(String.Format(@"<p>{0}</a> ({1}x)</p>", item.Key, list1.Count().ToString(CultureInfo.InvariantCulture)));

            var row = new StatTableRow
            {
                Link = $@"<a href=""#key{i}""><img src=""down.png"" alt=""Go to details"" /></a>",
                Item = item.Key,
                Count = list1.Count
            };

            statTable.Add(row);

            var vm = 0;

            var erg = new List<DetailTableRow>();

            foreach (var subitem in list1)
            {

                var subitem1 = subitem.FirstOrDefault();
                if (subitem1 == null) continue;

                var det = new DetailTableRow
                {
                    HostAddress = subitem1.HostName,
                    IpAddress = subitem1.IpAddresses[0],
                    Summary =
                        $@"<a href=""{Path.GetFileName(subitem1.SummaryFile)}""><img src=""down.png"" alt=""Go to summary""/></a>",
                    Details =
                        $@"<a href=""{Path.GetFileName(subitem1.DetailFile)}""><img src=""down.png"" alt=""Go to details""/></a>",

                };

                erg.Add(det);

                if (subitem1.VirtualMachine) vm++;
            }

            overview.Append(GetTable(erg.ToDataTable()));

            row.CountVm = vm;

            i++;
        }

        body.Append(GetTable(statTable.ToDataTable()));

        body.Append(overview);
    }


    private static string GetTable(DataTable dataTable)
    {

        var ex = new DataTableExport
        {
            CssTable = "wr_table",
            CssTableCell = "wr_cell",
            CssAlternatingRows = "_alt",
            CssTableHeader = "wr_header",
            Data = dataTable
        };

        ex.CreateTable();

        return ex.Result;

    }


    ///// <summary>
    ///// Domain overview page by software 
    ///// </summary>
    //public string DomainOverviewBySoftware(StartPageItem item)
    //{

    //    var list = (from i in _network.Domain.Computers group i by i.OperatingSystem).ToList();

    //    var html = new HtmlHandler(_htmlMaster) { Title = "Domain items by operating system" };

    //    var body = new StringBuilder();

    //    DomainAddGroupView(body, list);

    //    return body.ToString();
    //    html.SaveHtml();
    //}

    /// <summary>
    /// List of all disabled users
    /// </summary>
    public string DomainUserDisabled(StartPageItem item)
    {

        var list = (from i in _network.Domain.Users where i.Disabled orderby i.Fullname select i).ToList();
        if (!list.Any())
        {
            return "<p>No data found!</p>";
        }

        var body = new StringBuilder();

        foreach (var u in list)
        {
            body.Append($"<p>{u.Fullname}</p>");
        }

        return body.ToString();
    }

    /// <summary>
    /// List of users who cannot change their passwords
    /// </summary>
    internal string DomainPasswordCantChange(StartPageItem item)
    {

        var list = (from i in _network.Domain.Users where i.PasswordCantChange && !i.Disabled orderby i.Fullname select i).ToList();
        if (!list.Any())
        {
            return "<p>No data found!</p>";
        }

        var body = new StringBuilder();

        foreach (var u in list)
        {

            body.Append($"<p>{u.Fullname}</p>");

        }

        return body.ToString();
    }

    /// <summary>
    /// All shares
    /// </summary>
    public string AllShares(StartPageItem item)
    {

        var shares = new AllShares { Shares = _network.NetworkItems.SelectMany(x => x.Shares).ToList().Where(x => x.Type == "Disk Drive").ToList() };

        var s = Web.Basics.SerializationHelper.DataContractSerialize(shares);


        //if (Directory.Exists(@"c:\test\"))
        //{
        //    if (File.Exists(@"c:\test\shares.xml")) File.Delete(@"c:\test\shares.xml");
        //    var sw = new StreamWriter(@"c:\test\shares.xml", false, Encoding.GetEncoding("utf-8"));
        //    sw.Write(s);
        //    sw.Close();
        //}

        var byteArray = Encoding.UTF8.GetBytes(s);

        var m = new MemoryStream(byteArray);


        var target = Path.Combine(_currentSettings.HtmlTargetDir, item.FileName);


        //Task.Factory.StartNew(() =>
        //    {
        var xsl = new XsltHandler { XslFile = Path.Combine(_appPath, "prototypes", "Shares.xsl") };
        xsl.Start();
        xsl.Transform(m, target);

        return null;
    }

    /// <summary>
    /// Users with no expired passwords
    /// </summary>
    public string DontExpirePassword(StartPageItem item)
    {

        var list = (from i in _network.Domain.Users where i.DontExpirePassword && !i.Disabled orderby i.Fullname select i).ToList();
        if (!list.Any())
        {
            return "<p>No data found!</p>";
        }
        var body = new StringBuilder();

        foreach (var u in list)
        {

            body.Append($"<p>{u.Fullname}</p>");

        }

        return body.ToString();
    }

    internal string DomainPwdNotRequired(StartPageItem item)
    {

        var list = (from i in _network.Domain.Users where i.PasswordNotRequired && !i.Disabled orderby i.Fullname select i).ToList();

        if (!list.Any())
        {
            return "<p>No data found!</p>";
        }

        var body = new StringBuilder();
        foreach (var u in list)
        {
            body.Append($"<p>{u.Fullname}</p>");
        }

        return body.ToString();
    }


    private static void DomainAddGroupView(StringBuilder body, IEnumerable<IGrouping<string, DomainComputerItem>> list)
    {
        var data = list.ToList();
        if (!data.Any())
        {
            body.AppendLine("<p>No data found</p>");
            return;

        }

        var overview = new StringBuilder();


        body.Append("<h2>Overview</h2>");

        var i = 0;
        var statTable = new List<StatTableRow>();

        foreach (var item in data.OrderBy(x => x.Key))
        {

            overview.AppendFormat(@"<h2 id=""key{1}"">{0}</h2>", item.Key, i);


            var list1 = item.OrderBy(x => x.Name).ToList();

            //body.Append(String.Format(@"<p>{0}</a> ({1}x)</p>", item.Key, list1.Count().ToString(CultureInfo.InvariantCulture)));

            var row = new StatTableRow
            {
                Link = $@"<a href=""#key{i}""><img src=""down.png"" alt=""Go to details"" /></a>",
                Item = item.Key,
                Count = list1.Count
            };

            statTable.Add(row);

            //var vm = 0;

            var erg = new List<DomainComputerRow>();

            foreach (var subitem in list1)
            {

                var sitem = subitem;

                var det = new DomainComputerRow
                {
                    HostName = sitem.Name,
                    ServicePack = sitem.ServicePack,
                    //Text3 = String.Format(@"<a href=""{0}""><img src=""down.png"" alt=""Go to details""/></a>", Path.GetFileName(subitem1.DetailFile)),
                    //Text4 = String.Format(@"<a href=""{0}""><img src=""down.png"" alt=""Go to details""/></a>", Path.GetFileName(subitem1.SummaryFile)),

                };

                erg.Add(det);

                var ni = _network.NetworkItems.FirstOrDefault(x => x.HostName == sitem.Name);

                if (ni == null) continue;

                //if (ni.VirtualMachine) vm++;

                det.Details =
                    $@"<a href=""{Path.GetFileName(ni.DetailFile)}""><img src=""down.png"" alt=""Go to details""/></a>";
                det.Summary =
                    $@"<a href=""{Path.GetFileName(ni.SummaryFile)}""><img src=""down.png"" alt=""Go to details""/></a>";
            }

            overview.Append(GetTable(erg.ToDataTable()));

            i++;
        }

        body.Append(GetTable(statTable.ToDataTable()));

        body.Append(overview);
    }

    /// <summary>
    /// Overview page missing software on clients
    /// </summary>
    public string MissingSoftwareClient(StartPageItem item)
    {

        if (string.IsNullOrEmpty(_currentSettings.SoftwareAllClients))
        {                
            return "<p>No data found!</p>";
        }

        var body = new StringBuilder();

        var softwares = _currentSettings.SoftwareAllClients.Split(',');

        if (softwares.Length == 0)
        {
            body.AppendLine("<p>No data found!</p>");
        }
        else
        {
            foreach (var software in softwares)
            {

                var soft = software.ToLower();

                body.Append($"<h2>{software}</h2>");

                var list = _network.NetworkItems.Where(
                    x =>
                        (x.DomainRole == "Client" || x.DomainRole == "Member Workstation" || x.DomainRole == "Standalone Workstation") &&
                        (x.HostName != x.IpAddresses[0]) &&
                        x.SummaryFile != null && x.Software.Any() &&
                        x.Software.Count(z => z.Name != null && z.Name.ToLower().Contains(soft)) == 0).OrderBy(x => x.HostName).ToList();


                foreach (var u in list)
                {
                    body.Append($"<p>{u.HostName} ({u.IpAddresses[0]})</p>");
                }

            }
        }

        return body.ToString();
    }




    public string CreateInfrastructure(StartPageItem item)
    {

        var target = Path.Combine(_currentSettings.HtmlTargetDir, item.FileName);

        var xsl = new XsltHandler { XslFile = Path.Combine(_appPath, @"prototypes\infrastructure.xsl") };
        xsl.Start();
        xsl.Transform(_currentSettings.InfrastructureXmlFile, target);
        return null;
    }

    /// <summary>
    /// Create web page for warnings
    /// </summary>
    public string Warnings(StartPageItem item)
    {
        var body = new StringBuilder();

        var isData = false;

        foreach (var current in ((WarningSeverityLevel[])Enum.GetValues(typeof(WarningSeverityLevel)))
                 .ToList().Where(x=>x!= WarningSeverityLevel.None)
                 .OrderByDescending(x => x).ToList())
        {
            var value = current;
                

            var hosts = _network.NetworkItems.OrderBy(x => x.Ip).SelectMany(x =>
                x.Warnings.Where(y => y.WarningSeverityLevel == value)).ToList();

            if (hosts.Count == 0) continue;

            isData = true;
            body.AppendFormat("<h2>Warnings for severity level {0}</h2>", value);

            foreach (var host in hosts)
            {
                body.AppendFormat("<p>{0}</p>", host.Message);
            }

            body.AppendFormat("<p>&nbsp;</p>");
        }

        if (!isData) body.AppendFormat("<p>No warnings found!</p>");

        return body.ToString();

    }
}