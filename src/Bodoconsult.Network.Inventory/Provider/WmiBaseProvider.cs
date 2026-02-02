using System;
using System.Management;
using System.Text;
using System.Xml;
using Bodoconsult.Inventory.Helper;

namespace Bodoconsult.Inventory.Provider;

public class WmiBaseProvider
{

    public string LocalIp { get; set; }


    internal ConnectionOptions ConnectionOptions;

    public string HostName { get; set; }


    public bool Error { get; set; }

    internal XmlChecker Xmlc = new XmlChecker { DoNotDeleteNodeNames = "BoInv" };



    public string WmiPath { get; set; }

    internal static Encoding Encoder = Encoding.GetEncoding("ISO-8859-2");

    /// <summary>
    /// IP address of the item
    /// </summary>
    public string Ip { get; set; }

    /// <summary>
    /// Domain of the user to access WMI with
    /// </summary>
    public string Domain { get; set; }

    /// <summary>
    /// Username of the user to access WMI with
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// Password of the user to access WMI with
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// XML file name to save XML in
    /// </summary>
    public string XmlFileName { get; set; }



        

    public bool VirtualMachine { get; set; }

    public string VirtualMachineHost { get; set; }

    internal ManagementScope WmiScope;


    public void AddName(XmlNode node, string content)
    {
        var attr = Xmlc.Xml.CreateAttribute("name");
        attr.Value = content;
        node.Attributes.Append(attr);
    }


    public void CreateSubNodes(string root, string mainNodeCaption, string wql, string caption)
    {

        // ReSharper disable IntroduceOptionalParameters.Local
        CreateSubNodes(root, mainNodeCaption, wql, caption, null, null, null);
        // ReSharper restore IntroduceOptionalParameters.Local

    }

    public void CreateSubNodes(string root, string mainNodeCaption, string wql, string caption, string subWql, string fieldName, string subItemName)
    {
        try
        {



            //var subroot = Xml.CreateElement("");
            //AddName(subroot, mainNodeCaption);
            //node.AppendChild(subroot);


            //get Fixed disk stats


            var oQuery = new ObjectQuery(wql);

            //Execute the query 
            var oSearcher = new ManagementObjectSearcher(WmiScope, oQuery);

            //Get the results
            var result = oSearcher.Get();

            CreateSubNodes(result, root, mainNodeCaption, caption, subWql, fieldName, subItemName);

        }
        catch (Exception ex)
        {
            var s = ex.Message;

        }
    }

    public void CreateSubNodes(ManagementObjectCollection result, string root, string mainNodeCaption, string caption, string subWql, string fieldName, string subItemName)
    {

        root += $"/NetworkItemSubSection[{mainNodeCaption}]";
        Xmlc.CheckNode(root, null);

        var i = 1;

        var wmienumerator = result.GetEnumerator();

        //loop through found drives and write out info
        while (wmienumerator.MoveNext())
        {
            var oReturn = wmienumerator.Current;


            //var item = Xml.CreateElement("NetworkItemItem");
            //AddName(item, String.Format(caption, i));
            //subroot.AppendChild(item);



            var item = root + $"/NetworkItemItem[{string.Format(caption, i)}]";
            Xmlc.CheckNode(item, null);


            var enumerator = oReturn.Properties.GetEnumerator();

            while (enumerator.MoveNext())
            {
                var oItem = enumerator.Current;

                switch (oItem.Name)
                {

                    case "DS_userParameters":
                    case "OEMLogoBitmap":

                        continue;

                }

                //var value = Xml.CreateElement("NetworkItemValue");
                //AddName(value, oItem.Name);

                var value = item + $"/NetworkItemValue[{oItem.Name}]";
                //if (value.Contains("NetworkAdapters")) Debug.Print(value);

                if (oItem.IsArray)
                {

                    Xmlc.CheckNode(value, null);

                    var propertyArray = (Array)oItem.Value;

                    if (propertyArray != null)
                    {
                        var z = 1;
                        foreach (object wmiArrayElement in propertyArray)
                        {
                            //var subitem = Xml.CreateElement("NetworkItemSubItem");
                            //AddName(subitem, String.Format("SubItem {0}", z));
                            //subitem.InnerText = CheckString(wmiArrayElement.ToString().Trim());
                            //value.AppendChild(subitem);


                            var subitem = value +
                                          $"/NetworkItemSubItem[{$"SubItem {z}"}]";

                                

                            Xmlc.CheckNode(subitem, CheckString(wmiArrayElement.ToString().Trim()));

                            z++;
                        }
                        //item.AppendChild(value);
                    }



                }
                else
                {



                    if (oItem.Value != null)
                    {
                        if (!string.IsNullOrEmpty(oItem.Value.ToString().Trim()))
                        {
                            Xmlc.CheckNode(value, CheckString(oItem.Value.ToString().Trim()));

                            //value.InnerText = CheckString(oItem.Value.ToString().Trim());
                            //item.AppendChild(value);
                        }


                    }
                }

                var desc = item + $"/NetworkItemValue[{"BoInvDescription"}]";
                Xmlc.CheckNode(desc, null);

            }


            if (!string.IsNullOrEmpty(subWql))
            {
                var id = oReturn.GetPropertyValue(fieldName).ToString();

                if (!string.IsNullOrEmpty(id))
                {

                    //var mainItem = Xml.CreateElement("NetworkItemValue");


                    //AddName(mainItem, String.Format(subItemName));
                    //item.AppendChild(mainItem);

                    var mainItem = item + $"/NetworkItemSubValue[{string.Format(subItemName)}]";
                    Xmlc.CheckNode(mainItem, null);

                    var oSubQuery = new ObjectQuery(string.Format(subWql, id));

                    //Execute the query 
                    var oSubSearcher = new ManagementObjectSearcher(WmiScope, oSubQuery);

                    //Get the results
                    var oSubReturnCollection = oSubSearcher.Get();

                    var j = 1;

                    var wmiSubEnumerator = oSubReturnCollection.GetEnumerator();

                    //loop through found drives and write out info
                    while (wmiSubEnumerator.MoveNext())
                    {
                        var oSubItem = wmiSubEnumerator.Current;

                        var subItemEumerator = oSubItem.Properties.GetEnumerator();


                        //var subitem = Xml.CreateElement("NetworkItemSubItem");
                        //AddName(subitem, String.Format(subItemName + " {0}", j));
                        //mainItem.AppendChild(subitem);

                        var subItem = mainItem +
                                      $"/NetworkItemSubItem[{string.Format(subItemName + " {0}", j)}]";
                        //if (subItem.Contains("NetworkAdapters")) Debug.Print(subItem);

                        Xmlc.CheckNode(subItem, null);

                        while (subItemEumerator.MoveNext())
                        {

                            var oValue = subItemEumerator.Current;

                            //var subitemvalue = Xml.CreateElement("NetworkItemSubItemValue");
                            //AddName(subitemvalue, oValue.Name);


                            var s = "";

                            if (oValue.IsArray)
                            {

                                var propertyArray = (Array)oValue.Value;

                                if (propertyArray != null)
                                {
                                    foreach (object wmiArrayElement in propertyArray)
                                    {
                                        s += " " + wmiArrayElement;
                                    }

                                    //subitemvalue.InnerText = CheckString(s.Trim());
                                }
                            }
                            else
                            {
                                s = (oValue.Value == null) ? "" : oValue.Value.ToString().Trim();

                                if (!string.IsNullOrEmpty(s))
                                {
                                    //subitemvalue.InnerText = CheckString(s);
                                }
                            }

                            //subitem.AppendChild(subitemvalue);

                            var subItemValue = subItem + $"/NetworkItemSubItemValue[{oValue.Name}]";
                            //if (subItemValue.Contains("NetworkAdapters")) Debug.Print(subItemValue);

                            Xmlc.CheckNode(subItemValue, s);

                        }

                        j++;
                    }
                }
            }

            i++;
        }
    }


    internal static string CheckString(string erg)
    {
        if (erg == null) return null;


        //if (erg.Contains("Vordefiniertes Konto"))
        //{

        //    var s = erg;

        //}





        var b = Encoder.GetBytes(erg.Replace((char)31, ' '));
        erg = Encoder.GetString(b);
        return erg;
    }


    public void Start(string rootElement)
    {
        var hostName = Ip== LocalIp ? "localhost": Ip;

        Error = false;
        try
        {
            var options = WmiHelper.GetWmiConnectionOptions(Ip, LocalIp, Domain, Username, Password);

            WmiScope = new ManagementScope(string.Format(WmiPath, hostName), options);
            WmiScope.Connect();

            Xmlc.XmlFileName = XmlFileName;
            Xmlc.Start(rootElement);

        }
        catch (Exception ex)
        {
            Error = true;

            var msg = $"{LocalIp} {Ip} {HostName} {ex.Message}";
            throw new Exception(msg);

        }
    }

        

    /// <summary>
    /// Save XML as file
    /// </summary>
    public void Save()
    {
        if (Error) return;
        //Xml.Save(Path);

        Error = Xmlc.Save();
    }

    public XmlNodeList GetNodes(string section, string subsection)
    {
        return Xmlc.Xml.SelectNodes(
            $"NetworkItemRoot/NetworkItemSection[attribute::name='{section}']/NetworkItemSubSection[attribute::name='{subsection}']/NetworkItemItem");
    }

    public XmlNode GetFirstNode(string section, string subsection)
    {
        return Xmlc.Xml.SelectSingleNode(
            $"NetworkItemRoot/NetworkItemSection[attribute::name='{section}']/NetworkItemSubSection[attribute::name='{subsection}']/NetworkItemItem");
    }


    public static string SelectValue(XmlNode node, string value)
    {
        var n = node.SelectSingleNode($"NetworkItemValue[attribute::name='{value}']");
        return n == null ? null : n.InnerText;
    }
}