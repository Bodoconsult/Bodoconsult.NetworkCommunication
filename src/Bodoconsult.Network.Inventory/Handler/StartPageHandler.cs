using System;
using System.IO;
using System.Text;
using Bodoconsult.Inventory.Helper;
using Bodoconsult.Inventory.Model;

namespace Bodoconsult.Inventory.Handler;

public class StartPageHandler
{
    private readonly StartPage _startPage;
    private readonly string _htmlMaster;

    //private readonly string _appPath;

    private readonly GeneralSettings _currentSettings;


    public StartPageHandler(StartPage startPage, GeneralSettings settings)
    {
        _startPage = startPage;
        _htmlMaster = FileHelper.ReadHtmlMaster();
        _currentSettings = settings;
    }

    /// <summary>
    /// Create the start page
    /// </summary>
    public void CreateStartPage()
    {

        var body = new StringBuilder();

        foreach (var item in _startPage.PageItems)
        {
            switch (item.PageItemType)
            {
                case StartPageItemType.Header1:
                    body.AppendFormat("<h1>{0}</h1>", item.Title);
                    break;
                case StartPageItemType.Header2:
                    body.AppendFormat("<h2>{0}</h2>", item.Title);
                    break;
                case StartPageItemType.HtmlContent:
                    CreateHtml(item);
                    body.AppendFormat("<p><a href=\"{1}\" title=\"{0}\"><img src=\"down.png\"> {0}</a></p>", item.Title, item.FileName);
                    break;
                case StartPageItemType.XmlContent:
                    CreateXmlFile(item);
                    body.AppendFormat("<p><a href=\"{1}\" title=\"{0}\"><img src=\"down.png\"> {0}</a></p>", item.Title, item.FileName);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        var html = new HtmlHandler(_htmlMaster)
        {
            Title = "Overview network items",
            Target = Path.Combine(_currentSettings.HtmlTargetDir, _startPage.FileName),
            Body = body.ToString()
        };
        html.SaveHtml(false);
    }

    private void CreateHtml(StartPageItem item)
    {
        var x = item.GetContentDelegate;
        if (x == null) return;

        var body = x(item);

        var html = new HtmlHandler(_htmlMaster)
        {
            Title = item.Title,
            Target = Path.Combine(_currentSettings.HtmlTargetDir, item.FileName),
            Body = body
        };
        html.SaveHtml();
    }

    private static void CreateXmlFile(StartPageItem item)
    {
        var x = item.GetContentDelegate;
        if (x != null) x(item);
    }


    //private string ReadPrototype(string fileName)
    //{
    //    var fsIn = new FileStream(Path.Combine(_appPath, "prototypes", fileName), FileMode.Open, FileAccess.Read, FileShare.Read);
    //    var sr = new StreamReader(fsIn);
    //    var s = sr.ReadToEnd();
    //    sr.Close();
    //    sr.Dispose();
    //    fsIn.Close();
    //    fsIn.Dispose();

    //    return s;
    //}
}