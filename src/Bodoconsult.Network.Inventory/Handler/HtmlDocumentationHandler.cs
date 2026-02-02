using Bodoconsult.Inventory.Helper;
using Bodoconsult.Inventory.Model;

namespace Bodoconsult.Inventory.Handler;

/// <summary>
/// Create a documentation website for the network
/// </summary>
public class HtmlDocumentationHandler
{
    /// <summary>
    /// Default ctor
    /// </summary>
    public HtmlDocumentationHandler(GeneralSettings settings)
    {
        CurrentSettings = settings;
    }


    public GeneralSettings CurrentSettings { get; private set;  }

    /// <summary>
    /// Network data
    /// </summary>
    public Network Network { get; private set; }


    /// <summary>
    /// Get all needed network data
    /// </summary>
    public void GetNetworkData()
    {
        var ih = new InventoryHandler(CurrentSettings);
        ih.GetDomainData();
        ih.FindAllNetworkItems();
        ih.GetDataForNetworkItems();
        ih.SaveNetwork();
        Network = ih.Network;
    }

    /// <summary>
    /// Create the docementation website for the network
    /// </summary>
    public void CreateWebsite()
    {
        SendStatus("HtmlDocumentationHandler", "Start website creation");
        var wh = new WebsiteHandler(Network, CurrentSettings);
        wh.Status += SendStatus;
        wh.CreateHtmlContent();
    }



    protected void SendStatus(string modul, string msg)
    {
        //_IsStart = false;
        var x = CurrentSettings.StatusMessage;
        x?.Invoke(modul, msg);


    }

    protected void SendMail(string msg)
    {
        //_IsStart = false;
        var x = CurrentSettings.MailStatusMessage;
        if (x != null) x(msg);
    }

    public void GetNetworkData(string jsonFile)
    {
        Network = JsonHelper.LoadJsonFile<Network>(jsonFile);

        var xmlPath = CurrentSettings.XmlTargetDir;

        NetworkItemHelper.CheckAllNetworkItemPaths(Network, xmlPath, CurrentSettings.HtmlTargetDir);
    }
}