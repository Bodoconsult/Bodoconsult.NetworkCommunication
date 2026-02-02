using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Bodoconsult.Inventory.Model;

[DataContract(Namespace = "http://bodoconsult/inventory")]
public class UserItem
{
    /// <summary>
    /// Full name of the user
    /// </summary>
    [DataMember]
    public string Fullname { get; set; }

    /// <summary>
    /// Surname of the user
    /// </summary>
    [DataMember]
    public string Surname { get; set; }

    /// <summary>
    /// First name of the user
    /// </summary>
    [DataMember]
    public string FirstName { get; set; }

    /// <summary>
    /// email address of the user
    /// </summary>
    [DataMember]
    public string MailAddress { get; set; }

    /// <summary>
    /// Distinguished name of the suer
    /// </summary>
    [DataMember]
    public string DistinguishedName { get; set; }

    /// <summary>
    /// Principal name of the suer
    /// </summary>
    [DataMember]
    public string PrincipalName { get; set; }

    /// <summary>
    /// Path to the user's profile
    /// </summary>
    [DataMember]
    public string ProfilePath { get; set; }
    /// <summary>
    /// Path to the user's logon script
    /// </summary>
    [DataMember]
    public string ScriptPath { get; set; }

    /// <summary>
    /// Date of the last logon
    /// </summary>
    [DataMember]
    public DateTime LastLogon { get; set; }

    /// <summary>
    /// Groups the user is member of
    /// </summary>
    [DataMember]
    public List<string> Groups { get; set; }

    /// <summary>
    /// Direct permissions of the user
    /// </summary>
    [DataMember]
    public List<string> DirectPermissions { get; set; }

    /// <summary>
    /// Is user disabled?
    /// </summary>
    [DataMember]
    public bool Disabled { get; set; }

    /// <summary>
    /// Password is not required for the user account
    /// </summary>
    [DataMember]
    public bool PasswordNotRequired { get; set; }

    /// <summary>
    /// User can't change password
    /// </summary>
    [DataMember]
    public bool PasswordCantChange { get; set; }
       
    /// <summary>
    /// User's password never expires
    /// </summary>
    [DataMember]
    public bool DontExpirePassword { get; set; }

    /// <summary>
    /// SID of the user
    /// </summary>
    [DataMember]
    public string Sid { get; set; }

    /// <summary>
    ///  default ctor
    /// </summary>
    public UserItem()
    {
        Groups = new List<string>();
        Disabled = false;
        PasswordNotRequired = false;
        PasswordCantChange = false;
        DontExpirePassword = false;
        DirectPermissions = new List<string>();
    }


}