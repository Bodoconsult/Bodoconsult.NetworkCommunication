using System.IO;
using System.Text;
using Bodoconsult.Inventory.Helper;
using Bodoconsult.Inventory.Model;

namespace Bodoconsult.Inventory.Handler;

/// <summary>
/// Handles domain data export to XML, HTML, ...
/// </summary>
public class DomainDataHandler
{

    private readonly string _xmlDomain;

    private readonly string _appPath;

    /// <summary>
    /// default ctor
    /// </summary>
    /// <param name="domain">Domain data</param>
    public DomainDataHandler(DomainItem domain)
    {
        _xmlDomain = Web.Basics.SerializationHelper.DataContractSerialize(domain);
        _appPath = FileHelper.GetAppPath();
    }

    /// <summary>
    /// Export the domain data as readable HTML file
    /// </summary>
    public void ExportAsHtml(string fileName)
    {
        var byteArray = Encoding.UTF8.GetBytes(_xmlDomain);

        var m = new MemoryStream(byteArray);

        //Task.Factory.StartNew(() =>
        //    {
        var xsl = new XsltHandler { XslFile = Path.Combine(_appPath, "prototypes", "domainSummary.xsl") };
        xsl.Start();
        xsl.Transform(m, fileName);
        //});
    }


    /// <summary>
    /// Exports domain data as XML file
    /// </summary>
    /// <param name="fileName">Export file name</param>
    public void ExportAsXml(string fileName)
    {
           
        //Task.Factory.StartNew(() =>
        //    {
        if (File.Exists(fileName)) File.Delete(fileName);
        var sw = new StreamWriter(fileName, false, Encoding.GetEncoding("utf-8"));
        sw.Write(_xmlDomain);
        sw.Close();
        //});
    }




}