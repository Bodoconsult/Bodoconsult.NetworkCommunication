using System.IO;
using System.Xml;
using System.Xml.Xsl;

namespace Bodoconsult.Inventory.Handler;

public class XsltHandler
{

    public string XslFile { get; set; }

    private readonly XslCompiledTransform _xsl = new XslCompiledTransform();

    public void Start()
    {
        _xsl.Load(XslFile);
    }


    public void Transform(string xmlFile, string htmlFile)
    {
        if (!File.Exists(xmlFile)) return;
        _xsl.Transform(xmlFile, htmlFile);
    }

    public void Transform(Stream xmlFile, string htmlFile)
    {
        xmlFile.Position = 0;

        var xml = XmlReader.Create(xmlFile);

        var xmlOut = XmlWriter.Create(htmlFile);

        _xsl.Transform(xml, xmlOut);

        xmlOut.Close();
        xml.Close();
    }

}