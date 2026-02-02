namespace Bodoconsult.Inventory.Model;

/// <summary>
/// Item to be placed on the start page
/// </summary>
public class StartPageItem
{

    /// <summary>
    /// Type of the start page item
    /// </summary>
    public StartPageItemType PageItemType { get; set; }

    /// <summary>
    /// Title of the page item
    /// </summary>
    public string Title { get; set; }


    /// <summary>
    /// File name of the page item target (without path)
    /// </summary>
    public string FileName { get; set; }


    /// <summary>
    /// Delegate to get the content for a HTML page. For XML the deleagtes creates the page directly
    /// </summary>
    public GetContentDelegate GetContentDelegate { get; set; }

}