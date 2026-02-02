using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Bodoconsult.Inventory.Helper;
using Microsoft.Management.Infrastructure;
using CimType = Microsoft.Management.Infrastructure.CimType;

namespace Bodoconsult.Inventory.Provider;

public class WmiCimBaseProvider
{

    public string LocalIp { get; set; }



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

    internal CimSession CimSession;


    public void AddName(XmlNode node, string content)
    {
        var attr = Xmlc.Xml.CreateAttribute("name");
        attr.Value = content;
        if (node.Attributes!=null) node.Attributes.Append(attr);
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

            var result = WmiCimHelper.ExecuteQuery(CimSession, WmiPath, wql);

            CreateSubNodes(result, root, mainNodeCaption, caption, subWql, fieldName, subItemName);

        }
        catch (Exception ex)
        {
            var s = ex.Message;

        }
    }

    public void CreateSubNodes(IEnumerable<CimInstance> result, string root, string mainNodeCaption, string caption, string subWql, string fieldName, string subItemName)
    {
        if (result == null) return;

        root += $"/NetworkItemSubSection[{mainNodeCaption}]";
        Xmlc.CheckNode(root, null);

        var i = 1;

        var cimInstances = result.ToList();
        foreach (var data in cimInstances)
        {

            //var item = Xml.CreateElement("NetworkItemItem");
            //AddName(item, String.Format(caption, i));
            //subroot.AppendChild(item);



            var item = root + $"/NetworkItemItem[{string.Format(caption, i)}]";
            Xmlc.CheckNode(item, null);


            foreach (var prop in data.CimInstanceProperties)
            {

                switch (prop.Name)
                {
                    case "DS_userParameters":
                    case "OEMLogoBitmap":
                        continue;
                }

                //var value = Xml.CreateElement("NetworkItemValue");
                //AddName(value, oItem.Name);

                var value = item + $"/NetworkItemValue[{prop.Name}]";
                //if (value.Contains("NetworkAdapters")) Debug.Print(value);


                switch (prop.CimType)
                {
                    case CimType.BooleanArray:
                    case CimType.UInt8Array:
                    case CimType.SInt8Array:
                    case CimType.UInt16Array:
                    case CimType.SInt16Array:
                    case CimType.UInt32Array:
                    case CimType.SInt32Array:
                    case CimType.UInt64Array:
                    case CimType.SInt64Array:
                    case CimType.Real32Array:
                    case CimType.Real64Array:
                    case CimType.Char16Array:
                    case CimType.DateTimeArray:
                    case CimType.StringArray:
                    case CimType.ReferenceArray:
                    case CimType.InstanceArray:
                        Xmlc.CheckNode(value, null);

                        var propertyArray = (Array)prop.Value;

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
                        break;
                    default:
                        if (prop.Value != null)
                        {
                            if (!string.IsNullOrEmpty(prop.Value.ToString().Trim()))
                            {
                                Xmlc.CheckNode(value, CheckString(prop.Value.ToString().Trim()));

                                //value.InnerText = CheckString(oItem.Value.ToString().Trim());
                                //item.AppendChild(value);
                            }


                        }

                        break;
                }

                var desc = item + $"/NetworkItemValue[{"BoInvDescription"}]";
                Xmlc.CheckNode(desc, null);

            }


            if (!string.IsNullOrEmpty(subWql))
            {
                var id = data.CimInstanceProperties[fieldName].Value.ToString();

                if (!string.IsNullOrEmpty(id))
                {

                    //var mainItem = Xml.CreateElement("NetworkItemValue");


                    //AddName(mainItem, String.Format(subItemName));
                    //item.AppendChild(mainItem);

                    var mainItem = item + $"/NetworkItemSubValue[{string.Format(subItemName)}]";
                    Xmlc.CheckNode(mainItem, null);

                    //Execute the query 
                    var subResults = WmiCimHelper.ExecuteQuery(CimSession, WmiPath, string.Format(subWql, id));


                    //Get the results
                    var j = 1;

                    foreach (var subResult in subResults)
                    {
                        //var subitem = Xml.CreateElement("NetworkItemSubItem");
                        //AddName(subitem, String.Format(subItemName + " {0}", j));
                        //mainItem.AppendChild(subitem);

                        var subItem = mainItem +
                                      $"/NetworkItemSubItem[{string.Format(subItemName + " {0}", j)}]";
                        //if (subItem.Contains("NetworkAdapters")) Debug.Print(subItem);

                        Xmlc.CheckNode(subItem, null);

                        foreach (var subProp in subResult.CimInstanceProperties)
                        {

                            //var subitemvalue = Xml.CreateElement("NetworkItemSubItemValue");
                            //AddName(subitemvalue, oValue.Name);


                            var s = "";

                            switch (subProp.CimType)
                            {
                                case CimType.BooleanArray:
                                case CimType.UInt8Array:
                                case CimType.SInt8Array:
                                case CimType.UInt16Array:
                                case CimType.SInt16Array:
                                case CimType.UInt32Array:
                                case CimType.SInt32Array:
                                case CimType.UInt64Array:
                                case CimType.SInt64Array:
                                case CimType.Real32Array:
                                case CimType.Real64Array:
                                case CimType.Char16Array:
                                case CimType.DateTimeArray:
                                case CimType.StringArray:
                                case CimType.ReferenceArray:
                                case CimType.InstanceArray:
                                    var propertyArray = (Array)subProp.Value;

                                    if (propertyArray != null)
                                    {
                                        foreach (object wmiArrayElement in propertyArray)
                                        {
                                            s += " " + wmiArrayElement;
                                        }

                                        //subitemvalue.InnerText = CheckString(s.Trim());
                                    }
                                    break;
                                default:
                                    if (subProp.Value != null)
                                    {
                                        s = (subProp.Value == null) ? "" : subProp.Value.ToString().Trim();

                                        if (!string.IsNullOrEmpty(s))
                                        {
                                            //subitemvalue.InnerText = CheckString(s);
                                        }
                                    }

                                    break;
                            }

                            //subitem.AppendChild(subitemvalue);

                            var subItemValue = subItem + $"/NetworkItemSubItemValue[{subProp.Name}]";
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
        Error = false;
        try
        {

            WmiPath = WmiPath.Replace("{0}", HostName);

            CimSession = WmiCimHelper.GetSession(Domain, HostName, Username, Password);

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