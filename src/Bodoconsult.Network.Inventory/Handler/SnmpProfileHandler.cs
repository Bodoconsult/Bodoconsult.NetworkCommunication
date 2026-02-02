using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Bodoconsult.Inventory.Helper;
using Bodoconsult.Inventory.Model;
using Bodoconsult.Snmp;

namespace Bodoconsult.Inventory.Handler;

public static class SnmpProfileHandler
{
    private static IList<SnmpProfile> _profiles;


    public static event SnmpHandler.StatusMessage Status;

    /// <summary>
    /// Currently used SNMP profiles
    /// </summary>
    public static IList<SnmpProfile> Profiles
    {
        get { return _profiles ?? (_profiles = new List<SnmpProfile>()); }
        private set
        {
            _profiles = value;
        }
    }


    /// <summary>
    /// Load profiles from a directory path
    /// </summary>
    /// <param name="path">full directory path</param>
    public static void LoadProfiles(string path)
    {
        Profiles = new List<SnmpProfile>();

        foreach (var file in new DirectoryInfo(path).GetFiles("*.json"))
        {
            try
            {
                var profile = JsonHelper.LoadJsonFile<SnmpProfile>(file.FullName);

                Profiles.Add(profile);
            }
            catch
            {
                GotStatus("No SnmpProfile: " + file.FullName);
            }
        }
    }

    /// <summary>
    /// Check a value with a profile and return a warning in case of serverity level higher than none
    /// </summary>
    /// <param name="profileName">profile name</param>
    /// <param name="oid">OID</param>
    /// <param name="value">value to check</param>
    /// <returns>Warning</returns>
    public static Warning CheckValue(string profileName, string oid, int value)
    {

        var profile = Profiles.FirstOrDefault(x => x.Name == profileName);

        if (profile == null) return null;

        var item = profile.ProfileItems.FirstOrDefault(x => x.Oid == oid);

        if (item == null) return null;

        var level = item.CheckSeverityLevel(value);
        if (level == WarningSeverityLevel.None) return null;

        var w = new Warning
        {
            WarningSeverityLevel = level,
            WarningSourceType = WarningSourceType.Hardware,
            Message = item.Description + ": " + value + ": " + MibHandler.GetDescription(oid)
        };

        return w;
    }

    private static void GotStatus(string msg)
    {
        Debug.Print(msg);

        //_IsStart = false;
        var x = Status;
        if (x != null) x(msg);
    }
}