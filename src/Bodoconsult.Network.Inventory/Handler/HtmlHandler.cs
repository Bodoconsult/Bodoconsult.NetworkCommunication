using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Bodoconsult.Inventory.Handler;

internal class HtmlHandler
{
    private const string MasterCopyright = @"<p id=""Copyright"">BodoInventory: Copyright {0} Bodoconsult EDV-Dienstleistungen GmbH</p>";

    private readonly string _copyright;

    private readonly string _master;

    public string Title { get; set; }

    public string Body { get; set; }

    public string Target { get; set; }


    public HtmlHandler(string master)
    {
        _master = master;
        _copyright = string.Format(MasterCopyright, DateTime.Now.Year);
    }



    /// <summary>
    /// Als HTML-Datei speichern
    /// </summary>
    public void SaveHtml(bool runAsync = true)
    {

        try
        {
            if (Body == null) return;

            var stand = "<p>&nbsp;</p><p>Last update: " + DateTime.Now.ToString("dd.MM.yyyy hh:mm") + "</p><p>&nbsp;</p>";

            if (runAsync)
            {
                Task.Factory.StartNew(() =>
                {
                    var s = string.Format(_master, Title, stand + Body + _copyright);
                    var sw = new StreamWriter(Target, false, Encoding.GetEncoding("utf-8"));
                    sw.Write(s);
                    sw.Close();
                    sw.Dispose();
                });
            }
            else
            {
                var s = string.Format(_master, Title, stand + Body + _copyright);
                var sw = new StreamWriter(Target, false, Encoding.GetEncoding("utf-8"));
                sw.Write(s);
                sw.Close();
                sw.Dispose();
            }

        }
// ReSharper disable EmptyGeneralCatchClause
        catch
// ReSharper restore EmptyGeneralCatchClause
        {


        }
    }


    public void Clear()
    {
        Title = null;
        Body = null;
        Target = null;
    }

}