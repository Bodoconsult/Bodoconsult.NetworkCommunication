using System.Runtime.Serialization;
using System.Security.AccessControl;

namespace Bodoconsult.Inventory.Model;

[DataContract(Namespace = "http://bodoconsult/inventory")]
public class AccessControl
{
    [DataMember]
    public string Identifier { get; set; }

    public FileSystemRights AccessMask { get; set; }

    [DataMember]
    public string Rights
    {
        get => AccessMask.ToString();
        set { var x = value; }
    }
}