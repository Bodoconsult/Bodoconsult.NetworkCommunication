using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Bodoconsult.Inventory.Helper;

public class XmlChecker
{
    public string XmlFileName { get; set; }

    internal XmlDocument Xml = new XmlDocument();

    internal XmlNode Root;

    public string DoNotDeleteNodeNames { get; set; }

    internal static Encoding Encoder = Encoding.GetEncoding("ISO-8859-2");

    public void Start(string baseNode)
    {

        if (File.Exists(XmlFileName))
        {
            Xml.Load(XmlFileName);
            Root = Xml.SelectSingleNode(baseNode);
            UnchangeNode(Root);
            return;
        }

        Root = Xml.CreateElement(baseNode);
        Xml.AppendChild(Root);

        var def = Xml.CreateXmlDeclaration("1.0", "utf-8", "yes");
        Xml.InsertBefore(def, Root);

        //Xml.Save(Path);

    }


    private void UnchangeNode(XmlNode node)
    {
        if (node == null) return;

        if (node.LocalName == "#text") return;

        if (node.Attributes != null)
        {
            var changed = node.Attributes["changed"];
            if (changed == null)
            {
                changed = Xml.CreateAttribute("changed");
                node.Attributes.Append(changed);
            }

            changed.Value = "0";
        }


        foreach (XmlNode subNode in node.ChildNodes)
        {
            UnchangeNode(subNode);
        }


    }


    public void CheckNode(string nodePath, string value)
    {

        // 

        var xpath = nodePath.Replace("[", "[attribute::name='").Replace("]", "']");
            

        var node = Xml.SelectSingleNode(xpath);

        var nodes = xpath.Split('/');
        var currentPath = "";
        var updated = "0";

        if (node == null)
        {
                
            var currentNode = Xml.SelectSingleNode(nodes[0]);

            var i = 1;
            var l = nodes.Count();
            foreach (var subNode in nodes)
            {
                currentPath += string.IsNullOrEmpty(currentPath) ? subNode : '/' + subNode;

                var x = Xml.SelectSingleNode(currentPath);

                if (x == null)
                {
                    var y = subNode.IndexOf("[", StringComparison.Ordinal);
                    var z = subNode.IndexOf("='", StringComparison.Ordinal);
                    var w = subNode.IndexOf("'", z + 2, StringComparison.Ordinal);

                    var name = (y > 0) ? subNode.Substring(0, y) : subNode;
                    var attrName = (z > 0) ? subNode.Substring(z + 2, w - z - 2) : "";


                    x = Xml.CreateElement(name);

                    if (!string.IsNullOrEmpty(attrName))
                    {
                        var attr = Xml.CreateAttribute("name");
                        attr.Value = attrName;
                        if (x.Attributes != null) x.Attributes.Append(attr);
                    }


                    updated = "1";

                    if (i == l && !string.IsNullOrEmpty(value))
                    {
                        x.InnerText = CheckString(value);
                    }

                    if (currentNode != null) currentNode.AppendChild(x);
                }

                currentNode = x;
                i++;
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(value))
            {
                updated = "1";
                node.InnerText = CheckString(value);
            }
        }


        currentPath = "";

        foreach (var subNode in nodes)
        {
            currentPath += string.IsNullOrEmpty(currentPath) ? subNode : '/' + subNode;
            //if (currentPath.Contains("NetworkAdapters")) Debug.Print(currentPath);

            var x = Xml.SelectSingleNode(currentPath);

            var changed = x.Attributes["changed"];
            if (changed == null)
            {
                changed = Xml.CreateAttribute("changed");
                x.Attributes.Append(changed);
            }

            if (changed.Value != "1") changed.Value = updated;
        }


    }

    public bool Save()
    {

        ClearUnchangedNodes(Root);

        try
        {
            Xml.Save(XmlFileName);
            return false;
        }
        catch (Exception ex)
        {
            Debug.Print("Not found: {0} {1}", XmlFileName, ex.Message);
            return true;
        }
    }

    private void ClearUnchangedNodes(XmlNode node)
    {
        if (node == null) return;

        if (node.LocalName == "#text") return;

        var name = node.Attributes["name"];

        if (name != null)
        {
            if (name.Value.StartsWith(DoNotDeleteNodeNames)) return;
        }

           

        if (node.Attributes["changed"].Value == "0")
        {
            node.ParentNode.RemoveChild(node);
        }


        foreach (XmlNode subNode in node.ChildNodes)
        {
            ClearUnchangedNodes(subNode);
        }

    }

    internal static string CheckString(string erg)
    {

        //if (erg.Contains("Vordefiniertes Konto"))
        //{

        //    var s = erg;

        //}
        var b = Encoder.GetBytes(erg.Replace((char)31, ' '));
        erg = Encoder.GetString(b);
        return erg;
    }

}