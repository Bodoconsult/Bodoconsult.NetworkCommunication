using System;
using System.Linq;
using System.Xml;
using Bodoconsult.Inventory.Interfaces;
using Bodoconsult.Inventory.Model;
using Bodoconsult.Inventory.Provider;

namespace Bodoconsult.Inventory.Converter;

/// <summary>
/// Fills a <see cref="Network"/> object with domain data. Converts a XML as result of the <see cref="WmiCimDomainDataProvider"/> 
/// to C# representations of the important domain data
/// </summary>
public class WmiDomainDataConverter
{
    /// <summary>
    /// WMI data provider with the data to fill in <see cref="Network"/> object
    /// </summary>
    public IWmiDomainDataProvider Data { get; set; }

    /// <summary>
    /// Domain object to fill with data
    /// </summary>
    public DomainItem Domain { get; set; }

    /// <summary>
    /// Get all domain users
    /// </summary>
    public void GetUsers()
    {
        var list = Data.GetNodes("ActiveDirectory", "AdUsers");

        for (var i = 0; i < list.Count; i++)
        {

            var item = new UserItem();



            var node = SelectValue(list[i], "DS_name");
            if (node != null) item.Fullname = node;

            if (Domain.Users.Any(x => x.Fullname == item.Fullname)) continue;

            node = SelectValue(list[i], "DS_userPrincipalName");
            if (node != null) item.PrincipalName = node;

            node = SelectValue(list[i], "DS_distinguishedName");
            if (node != null) item.DistinguishedName = node;

            node = SelectValue(list[i], "DS_givenName");
            if (node != null) item.FirstName = node;

            node = SelectValue(list[i], "DS_sn");
            if (node != null) item.Surname = node;

            node = SelectValue(list[i], "DS_mail");
            if (node != null) item.MailAddress = node;

            // DS_scriptPath
            node = SelectValue(list[i], "DS_scriptPath");
            if (node != null) item.ScriptPath = node;

            node = SelectValue(list[i], "DS_profilePath");
            if (node != null) item.ProfilePath = node;

            node = SelectValue(list[i], "DS_PasswordNeverExpires");
            if (node != null) item.DontExpirePassword = ToBool(node);

            node = SelectValue(list[i], "DS_PasswordCantChange");
            if (node != null) item.PasswordCantChange = ToBool(node);

            node = SelectValue(list[i], "DS_Disabled");
            if (node != null) item.Disabled = ToBool(node);

            node = SelectValue(list[i], "DS_PasswordNotRequired");
            if (node != null) item.PasswordNotRequired = ToBool(node);

            node = SelectValue(list[i], "DS_Sid");
            if (node != null) item.PasswordNotRequired = ToBool(node);

            //if (node != null)
            //{
            //    var value = Convert.ToInt32(node);

            //    item.Disabled = (value & (int)UserAccountControl.AccountDisable) == (int)UserAccountControl.AccountDisable;

            //    item.DontExpirePassword = (value & (int)UserAccountControl.DontExpirePasswd) == (int)UserAccountControl.DontExpirePasswd;

            //    item.PasswordCantChange = (value & (int)UserAccountControl.PasswdCantChange) == (int)UserAccountControl.PasswdCantChange;

            //    item.PasswordNotRequired = (value & (int)UserAccountControl.PasswdNotRequired) == (int)UserAccountControl.PasswdNotRequired;

            //}



            //node = SelectValue(list[i], "DS_lastLogon");
            //if (node != null)
            //{

            //    var d = DateTime.FromFileTime(Convert.ToInt64(node));


            //    item.LastLogon = d;
            //}

            Domain.Users.Add(item);
        }

    }

    private static bool ToBool(string node)
    {
        return node == "1";
    }

    /// <summary>
    /// Get all domain groups
    /// </summary>
    public void GetGroups()
    {
        var list = Data.GetNodes("ActiveDirectory", "AdGroups");

        for (var i = 0; i < list.Count; i++)
        {

            var item = new GroupItem();

            var node = SelectValue(list[i], "DS_cn");
            if (node != null) item.Name = node;

            if (Domain.Groups.Any(x => x.Name == item.Name)) continue;

            node = SelectValue(list[i], "DS_distinguishedName");
            if (node != null) item.DistinguishedName = node;

            Domain.Groups.Add(item);
        }

    }

    /// <summary>
    /// Get the relation between domain users and domain groups
    /// </summary>
    public void GetUserGroups()
    {
        var list = Data.GetNodes("ActiveDirectory", "AdUsers");

        for (var i = 0; i < list.Count; i++)
        {

            var node = SelectValue(list[i], "DS_name");
            if (node == null) continue;
            var user = Domain.Users.Find(x => x.Fullname == node);
            // User ermitteln

            var y = list[i].SelectSingleNode($"NetworkItemValue[attribute::name='{"DS_memberOf"}']");

            if (y == null) continue;

            var list1 = y.SelectNodes("*");

            if (list1 == null) continue;

            for (var z = 0; z < list1.Count; z++)
            {
                var wert = list1[z].InnerText;

                var j = wert.IndexOf(",", StringComparison.Ordinal);

                var userName = wert.Substring(0, j).Replace("CN=", "");

                var group = Domain.Groups.Find(x => x.Name == userName);
                if (group == null) continue;

                if (group.Users.IndexOf(user.Fullname) < 0)
                {
                    group.Users.Add(user.Fullname);
                }

                if (user.Groups.IndexOf(group.Name) < 0)
                {
                    user.Groups.Add(group.Name);
                }

            }
        }


        list = Data.GetNodes("ActiveDirectory", "AdGroups");

        for (var i = 0; i < list.Count; i++)
        {

            var node = SelectValue(list[i], "DS_name");
            if (node == null) continue;
            var group = Domain.Groups.Find(x => x.Name == node);
            if (group == null) continue;

            // User ermitteln
            XmlNodeList list1;

            var y = list[i].SelectSingleNode($"NetworkItemValue[attribute::name='{"DS_member"}']");

            if (y != null)
            {
                list1 = y.SelectNodes("*");

                if (list1 == null) continue;

                for (var z = 0; z < list1.Count; z++)
                {
                    var wert = list1[z].InnerText;

                    var j = wert.IndexOf(",", StringComparison.Ordinal);

                    var userName = wert.Substring(0, j).Replace("CN=", "");

                    var user = Domain.Users.Find(x => x.Fullname == userName);
                    if (user == null) continue;

                    if (group.Users.IndexOf(userName) < 0)
                    {
                        group.Users.Add(userName);
                    }

                    if (user.Groups.IndexOf(group.Name) < 0)
                    {
                        user.Groups.Add(group.Name);
                    }

                }
            }

            y = list[i].SelectSingleNode($"NetworkItemValue[attribute::name='{"DS_memberOf"}']");


            if (y == null) continue;

            list1 = y.SelectNodes("*");

            if (list1 == null) continue;

            for (var z = 0; z < list1.Count; z++)
            {
                var wert = list1[z].InnerText;

                var j = wert.IndexOf(",", StringComparison.Ordinal);

                var userName = wert.Substring(0, j).Replace("CN=", "");

                var user = Domain.Groups.Find(x => x.Name == userName);
                if (user == null) continue;

                if (group.MemberOfGroups.IndexOf(user.Name) < 0)
                {
                    group.MemberOfGroups.Add(user.Name);
                }

                if (user.GroupMembers.IndexOf(group.Name) < 0)
                {
                    user.GroupMembers.Add(group.Name);
                }
            }
        }

    }


    /// <summary>
    /// Get all registered computer accounts from domain server
    /// </summary>
    public void GetComputers()
    {
        var list = Data.GetNodes("ActiveDirectory", "AdComputers");

        for (var i = 0; i < list.Count; i++)
        {

            // Domain item
            var item = new DomainComputerItem();
                
            var node = SelectValue(list[i], "DS_cn");
            if (node != null) item.Name = node;

            node = SelectValue(list[i], "ADSIPath");
            if (node != null) item.AdsPath = node;

            node = SelectValue(list[i], "DS_lastLogon");
            if (node != null)
            {
                var d = DateTime.FromFileTime(Convert.ToInt64(node));
                item.LastLogon = d;
            }

            node = SelectValue(list[i], "DS_operatingSystem");
            if (node != null)
            {
                item.OperatingSystem = node;
                //if (node.ToLower().Contains("windows"))
            }

            node = SelectValue(list[i], "DS_operatingSystemServicePack");
            if (node != null) item.ServicePack = node;


            if (Domain.Computers.All(x => x.Name != item.Name))
            {
                Domain.Computers.Add(item);
            }
        }

    }



    private static string SelectValue(XmlNode node, string value)
    {
        var n = node.SelectSingleNode($"NetworkItemValue[attribute::name='{value}']");
        return n == null ? null : n.InnerText;
    }

    //public void SaveAsHtml()
    //{


    //    var s = Web.Basics.SerializationHelper.DataContractSerialize(Domain);


    //    if (Directory.Exists(@"c:\test\"))
    //    {
    //        if (File.Exists(@"c:\test\domain.xml")) File.Delete(@"c:\test\domain.xml");
    //        var sw = new StreamWriter(@"c:\test\domain.xml", false, Encoding.GetEncoding("utf-8"));
    //        sw.Write(s);
    //        sw.Close();
    //    }



    //    var byteArray = Encoding.UTF8.GetBytes(s);

    //    var m = new MemoryStream(byteArray);


    //    var target = Path.Combine(HtmlTargetDir, "Domain_summary.htm");


    //    //Task.Factory.StartNew(() =>
    //    //    {
    //    var xsl = new XsltHandler { XslFile = Path.Combine(AppPath, "prototypes", "domainSummary.xsl") };
    //    xsl.Start();
    //    xsl.Transform(m, target);
    //    //});
    //    //target = Path.Combine(HtmlTargetDir, String.Format("{0}_summary.xml", NetworkItem.Ip));

    //    //var sw = new StreamWriter(target, false, Encoding.UTF8);
    //    //m.WriteTo(sw.BaseStream);
    //    //sw.Close();
    //    //sw.Dispose();


    //}


}