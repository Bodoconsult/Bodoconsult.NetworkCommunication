using System.Collections.Generic;

namespace Bodoconsult.Inventory.Model;

/// <summary>
/// Creates the start page for the inventory web page
/// </summary>
public class StartPage
{
    /// <summary>
    /// default ctor
    /// </summary>
    public StartPage()
    {
        FileName = "index.htm";
        PageItems = new List<StartPageItem>();
    }

    /// <summary>
    /// File name of the start page (without path)
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// Items to be placed on the start page
    /// </summary>
    public IList<StartPageItem> PageItems { get; set; }
}


public delegate string GetContentDelegate(StartPageItem item);