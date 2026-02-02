using System;
using System.IO;
using System.Text;
using Bodoconsult.Inventory.Helper;
using Bodoconsult.Inventory.Model;

namespace Bodoconsult.Inventory.Handler;

/// <summary>
/// Export network item data to XML, HTML, ...
/// </summary>
public class NetworkItemDataHandler
{
    private readonly XsltHandler _xsl;

    private readonly string _xmlData;

    private readonly string _appPath;

    private readonly NetworkItem _item;

    /// <summary>
    /// default ctor
    /// </summary>
    /// <param name="item">network item</param>
    /// <param name="xsl"></param>
    public NetworkItemDataHandler(NetworkItem item, XsltHandler xsl)
    {
        try
        {
            _xmlData = Web.Basics.SerializationHelper.DataContractSerialize(item);
            _appPath = FileHelper.GetAppPath();
            _item = item;
            _xsl = xsl;
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e);
        }
    }

    /// <summary>
    /// Export the summary of a network item as user readable HTML file per IP address
    /// </summary>
    public void ExportAsHtml(string fileName)
    {
        try
        {

            if (fileName == null) return;

            var byteArray = Encoding.UTF8.GetBytes(_xmlData);

            var m = new MemoryStream(byteArray);

            var xsl = new XsltHandler { XslFile = Path.Combine(_appPath, "prototypes", "summary.xsl") };
            xsl.Start();

            foreach (var ip in _item.IpAddresses)
            {
                var target = string.Format(fileName, ip.Replace(".", "_"));
                xsl.Transform(m, target);
            }


            //target = Path.Combine(HtmlTargetDir, String.Format("{0}_summary.xml", NetworkItem.Ip));

            //var sw = new StreamWriter(target, false, Encoding.UTF8);
            //m.WriteTo(sw.BaseStream);
            //sw.Close();
            //sw.Dispose();
        }
        catch (Exception ex)
        {
            System.Console.WriteLine(ex.Message);
            _item.SummaryFile = null;
        }
    }

    /// <summary>
    /// Export details of a network item as user readable HTML page
    /// </summary>
    /// <param name="fileName"></param>
    public void ExportDetailsAsHtml(string fileName)
    {

        try
        {

            if (fileName == null) return;

            var source = _item.XmlFile;
            var target = fileName;

            _xsl.Transform(source, target);
        }
        catch //(Exception ex)
        {
            //GotStatus("ConvertToHtml", ex.Message);
            _item.DetailFile = null;
            _item.SummaryFile = null;
        }


    }


    /// <summary>
    /// Exports domain data as XML file
    /// </summary>
    /// <param name="fileName">Export file name</param>
    public void ExportAsXml(string fileName)
    {
        if (File.Exists(fileName)) File.Delete(fileName);
        var sw = new StreamWriter(fileName, false, Encoding.GetEncoding("utf-8"));
        sw.Write(_xmlData);
        sw.Close();
    }

}