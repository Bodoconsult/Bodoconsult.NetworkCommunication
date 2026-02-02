namespace Bodoconsult.Inventory.Model;

/// <summary>
/// Represents an additonal add network item Bodoconsult.Inventory could not handle correctly
/// </summary>
public class AdditionalItem
{
        

    /// <summary>
    /// IP address of the item
    /// </summary>
    public string IpAddress { get; set; }

    /// <summary>
    /// Username for accessing the network item via WMI
    /// </summary>
    public string Username { get; set; }


    private string  _password;

    /// <summary>
    /// Password for accessing the network item via WMI
    /// </summary>
    public string Password
    {
        get { return Console.PasswordHandler.Encrypt(_password); }
        set { _password = value; }
    }


    public string GetPassword()
    {
        return _password;
    }

    /// <summary>
    /// Is a HyperV host?
    /// </summary>
    public bool HyperVHost { get; set; }

    /// <summary>
    /// Is a VMWare host?
    /// </summary>
    public bool VmwareHost { get; set; }

}