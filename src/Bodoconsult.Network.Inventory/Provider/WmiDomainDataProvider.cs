using System;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.Management;
using System.Text;
using System.Xml;
using Bodoconsult.Inventory.Interfaces;

namespace Bodoconsult.Inventory.Provider;

/// <summary>
/// Provide domain data using older System.Management namespace
/// </summary>
public class WmiDomainDataProvider : WmiBaseProvider, IWmiDomainDataProvider
{

    /// <summary>
    /// default ctor
    /// </summary>
    public WmiDomainDataProvider()
    {
        WmiPath = "\\\\{0}\\root\\directory\\LDAP";
    }


    public void GetMetaData()
    {
        if (Error) return;

        Xmlc.CheckNode(
            "NetworkItemRoot/NetworkItemSection[Metadata]/NetworkItemSubSection[Current]/NetworkItemItem/NetworkItemValue[IpAddress]",
            Ip);

        Xmlc.CheckNode(
            "NetworkItemRoot/NetworkItemSection[Metadata]/NetworkItemSubSection[Current]/NetworkItemItem/NetworkItemValue[Hostname]",
            HostName);
        Xmlc.CheckNode(
            "NetworkItemRoot/NetworkItemSection[Metadata]/NetworkItemSubSection[Current]/NetworkItemItem/NetworkItemValue[Date]",
            DateTime.Now.ToString("dd.MM.yyyy"));
    }

    public void GetUsers()
    {
        if (Error) return;


        var ad = new PrincipalContext(ContextType.Domain, Domain);
        var u = new UserPrincipal(ad);
        var search = new PrincipalSearcher(u);



        const string s = "NetworkItemRoot/NetworkItemSection[ActiveDirectory]";

        Xmlc.CheckNode(s, null);

        CreateSubNodesUser(search, s, "AdUsers",  "AdUser {0}");

        //const string wmiPath = "\\\\{0}\\root\\cimv2";

        //var oConn = new ConnectionOptions
        //{
        //    Authority = string.Format("NTLMDOMAIN:{0}", Domain),
        //    Username = Username,
        //    Password = Password
        //};

        //var wmiScope = new ManagementScope(string.Format(wmiPath, Ip), oConn);
        //wmiScope.Connect();

        //ObjectQuery oQuery = new ObjectQuery("SELECT * FROM Win32_UserAccount WHERE LocalAccount=false");
        //ManagementObjectSearcher mgmtSearch = new ManagementObjectSearcher(wmiScope, oQuery);
        //ManagementObjectCollection objCollection = mgmtSearch.Get();


        //const string s = "NetworkItemRoot/NetworkItemSection[ActiveDirectory]";

        //Xmlc.CheckNode(s, null);

        //CreateSubNodes(objCollection, s, "AdUsers", "AdUser {0}", null, null, null);

    }



    private void CreateSubNodesUser(PrincipalSearcher search, string root, string mainNodeCaption, string caption)
    {

        root += $"/NetworkItemSubSection[{mainNodeCaption}]";
        Xmlc.CheckNode(root, null);

        var i = 1;



        //loop through found drives and write out info
        // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
        foreach (UserPrincipal result in search.FindAll())
        {

            var item = root + $"/NetworkItemItem[{string.Format(caption, i)}]";
            Xmlc.CheckNode(item, null);

            var desc = item + "/NetworkItemValue[BoInvDescription]";
            Xmlc.CheckNode(desc, null);

            var value = item + "/NetworkItemValue[DS_name]";
            Xmlc.CheckNode(value, CheckString(result.Name));

            value = item + "/NetworkItemValue[DS_userPrincipalName]";
            Xmlc.CheckNode(value, CheckString(result.UserPrincipalName));

            value = item + "/NetworkItemValue[DS_distinguishedName]";
            Xmlc.CheckNode(value, CheckString(result.DistinguishedName));

            value = item + "/NetworkItemValue[DS_givenName]";
            Xmlc.CheckNode(value, CheckString(result.GivenName));

            value = item + "/NetworkItemValue[DS_sn]";
            Xmlc.CheckNode(value, CheckString(result.Surname));

            value = item + "/NetworkItemValue[DS_mail]";
            Xmlc.CheckNode(value, CheckString(result.EmailAddress));

            value = item + "/NetworkItemValue[DS_scriptPath]";
            Xmlc.CheckNode(value, CheckString(result.ScriptPath));

            value = item + "/NetworkItemValue[DS_profilePath]";
            Xmlc.CheckNode(value, CheckString(result.HomeDirectory));

            value = item + "/NetworkItemValue[DS_PasswordNeverExpires]";
            Xmlc.CheckNode(value, CheckBool(result.PasswordNeverExpires));

            value = item + "/NetworkItemValue[DS_Disabled]";
            Xmlc.CheckNode(value, CheckBoolInv(result.Enabled));

            value = item + "/NetworkItemValue[DS_PasswordNotRequired]";
            Xmlc.CheckNode(value, CheckBool(result.PasswordNotRequired));

            value = item + "/NetworkItemValue[DS_PasswordCantChange]";
            Xmlc.CheckNode(value, CheckBool(result.UserCannotChangePassword));

            value = item + "/NetworkItemValue[DS_lastLogon]";
            Xmlc.CheckNode(value, result.LastLogon==null ? "":CheckDate((DateTime)result.LastLogon));

            value = item + "/NetworkItemValue[DS_Sid]";
            Xmlc.CheckNode(value, CheckString(result.Sid.Value));

            i++;
        }
    }

    private static string CheckDate(DateTime date)
    {
        var erg = date.ToFileTimeUtc().ToString();
        return erg;
    }

    private static string CheckBool(bool? p)
    {
        if (p == null) return "";
        return (bool)p ? "1" : "0";
    }

    private static string CheckBoolInv(bool? p)
    {
        if (p == null) return "";
        return (bool)p ? "0" : "1";
    }

    public void GetGroups()
    {
        if (Error) return;

        const string s = "NetworkItemRoot/NetworkItemSection[ActiveDirectory]";

        Xmlc.CheckNode(s, null);

        CreateSubNodes(s, "AdGroups", "select * from ds_group", "AdGroup {0}");

        //GetGroupUsers();
    }


    //private void GetGroupUsers()
    //{

    //    var list = GetNode("ActiveDirectory", "AdGroups");

    //    for (var i = 0; i < list.Count; i++)
    //    {
    //        var node = SelectValue(list[i], "DS_cn");
    //        if (node != null)
    //        {


    //            GetUsers(list[i], node);
    //        }
    //    }



    //}


    public void GetUsers(XmlNode node, string groupName)
    {

        var sBuilder = new StringBuilder("GroupComponent=");
        sBuilder.Append('"');
        sBuilder.Append("Win32_Group.Domain=");
        sBuilder.Append("'");
        sBuilder.Append(Domain);
        sBuilder.Append("'");
        sBuilder.Append(",Name=");
        sBuilder.Append("'");
        sBuilder.Append(groupName);
        sBuilder.Append("'");
        sBuilder.Append('"');


        var wql = new SelectQuery("Win32_GroupUser", sBuilder.ToString());

        try
        {

            var users = Xmlc.Xml.CreateElement("NetworkItemValue");
            AddName(users, "DS_Members");
            node.AppendChild(users);

            const string wmiPath = "\\\\{0}\\root\\cimv2";

            var oConn = new ConnectionOptions
            {
                Authority = $"NTLMDOMAIN:{Domain}",
                Username = Username,
                Password = Password
            };

            var wmiScope = new ManagementScope(string.Format(wmiPath, Ip), oConn);
            wmiScope.Connect();


            //Execute the query 
            var oSearcher = new ManagementObjectSearcher(wmiScope, wql);

            var i = 1;

            foreach (var o in oSearcher.Get())
            {
                var mObject = (ManagementObject) o;
                var path = new ManagementPath(mObject["PartComponent"].ToString());

                var names = path.RelativePath.Split(',');

                var name = names[1].Replace("Name=\"", "").Replace("\"", "").Trim();

                var user = Xmlc.Xml.CreateElement("NetworkItemSubItem");
                AddName(user, $"SubItem {i}");
                user.InnerText = CheckString(name);
                users.AppendChild(user);

                i++;

            }
        }
        catch (Exception ex)
        {
            Debug.Print(ex.Message);
            // ignored
        }
    }






    public void GetComputers()
    {
        if (Error) return;

        const string s = "NetworkItemRoot/NetworkItemSection[ActiveDirectory]";

        Xmlc.CheckNode(s, null);

        CreateSubNodes(s, "AdComputers", "select * from ds_computer", "AdComputer {0}");


        CheckFileTimeUtc(GetNodes("ActiveDirectory", "AdComputers"), "DS_lastLogon");

        //DateTime.FromFileTimeUtc

    }

    private static void CheckFileTimeUtc(XmlNodeList nodes, string keyValue)
    {

        keyValue = keyValue.ToLower();


        var list = nodes;

        for (var i = 0; i < list.Count; i++)
        {

            var list1 = list[i].ChildNodes;

            for (var j = 0; j < list1.Count; j++)
            {


                var list2 = list1[j].ChildNodes;

                for (var z = 0; z < list2.Count; z++)
                {
                    var subNode = list2[z];
                    if (subNode == null || subNode.Attributes==null) continue;

                    try
                    {
                        if (subNode.Attributes["name"].InnerText.ToLower() == keyValue)
                        {
                            var x = Convert.ToInt64(subNode.InnerText);
                            subNode.InnerText = DateTime.FromFileTimeUtc(x).ToString("dd.MM.yyyy H:mm");
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                }

            }


        }



    }


    //public void GetMetaData()
    //{

    //    if (Error) return;

    //    _meta = Xml.CreateElement("NetworkItemSection");
    //    AddName(_meta, "Metadata");
    //    Root.AppendChild(_meta);

    //    var subroot = Xml.CreateElement("NetworkItemSubSection");
    //    AddName(subroot, "Meta data");
    //    _meta.AppendChild(subroot);

    //    var item = Xml.CreateElement("NetworkItemItem");
    //    AddName(subroot, "Current");
    //    subroot.AppendChild(item);

    //    var value = Xml.CreateElement("NetworkItemValue");
    //    AddName(value, "Name");
    //    value.InnerText = Domain;
    //    item.AppendChild(value);

    //    value = Xml.CreateElement("NetworkItemValue");
    //    AddName(value, "PrimaryDomainController");
    //    value.InnerText = base.Ip;
    //    item.AppendChild(value);

    //    value = Xml.CreateElement("NetworkItemValue");
    //    AddName(value, "Date");
    //    value.InnerText = DateTime.Now.ToString("dd.MM.yyyy");
    //    item.AppendChild(value);

    //}





}