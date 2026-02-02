using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using Bodoconsult.Inventory.Model;

namespace Bodoconsult.Inventory.Helper;

public class DirectoryAccessReader
{

    public string DirectoryPath { get; set; }


    public List<AccessControl> AccessControlList { get; set; }

    public DirectoryAccessReader()
    {
        AccessControlList = new List<AccessControl>();
    }


    public void ReadData()
    {

        var d = new DirectoryInfo(DirectoryPath);

        var acls = d.GetAccessControl();

        foreach (var x in from FileSystemAccessRule acl in acls.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount))
                 select new AccessControl
                 {
                     Identifier = acl.IdentityReference.Value,
                     AccessMask = acl.FileSystemRights
                 })
        {
            var checkUser = x.Identifier.ToLower();
            if (checkUser.StartsWith("nt authority\\")) continue;
            if (checkUser.StartsWith("builtin\\")) continue;
            if (checkUser.StartsWith("nt-authorität\\")) continue;
            if (checkUser.StartsWith("vordefiniert\\")) continue;
            if (checkUser.StartsWith("ersteller-")) continue;

            AccessControlList.Add(x);

        }
    }
}