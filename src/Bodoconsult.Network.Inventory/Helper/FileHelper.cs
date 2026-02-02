using System.IO;
using System.Reflection;

namespace Bodoconsult.Inventory.Helper;

/// <summary>
/// Helper class for file handling
/// </summary>
public class FileHelper
{

    /// <summary>
    /// Read an ASCII file
    /// </summary>
    /// <param name="fileName">filename to read in</param>
    /// <returns>content of the ASCII file</returns>
    public static string ReadAsciiFile(string fileName)
    {

        var fsIn = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
        var sr = new StreamReader(fsIn);
        var s = sr.ReadToEnd();
        sr.Close();
        sr.Dispose();
        fsIn.Close();
        fsIn.Dispose();

        return s;
    }

    /// <summary>
    /// Read the HTML master file
    /// </summary>
    /// <returns>HTML master to fill in content</returns>
    public static string ReadHtmlMaster()
    {
        return ReadAsciiFile(Path.Combine(GetAppPath(), "prototypes", "Master.htm"));
    }

    private static string _appPath;

    /// <summary>
    /// Get path of current application
    /// </summary>
    /// <returns>current application path</returns>
    public static string GetAppPath()
    {
        if (!string.IsNullOrEmpty(_appPath)) return _appPath;

        var s = Assembly.GetExecutingAssembly().Location;

        s = new FileInfo(s).DirectoryName;

        _appPath = s;
        return _appPath;
    }

    /// <summary>
    /// Returns the MIB path
    /// </summary>
    /// <returns></returns>
    public static string MibPath
    {
        get
        {
            if (_appPath == null) GetAppPath();
            return Path.Combine(_appPath, "Mib");
        }
    }

    /// <summary>
    /// Json path for additional data files
    /// </summary>
    public static string JsonDir
    {
        get
        {
            if (_appPath == null) GetAppPath();
            return Path.Combine(_appPath, "Json");
        }
    }


    /// <summary>
    /// Path for JSON files with SNMP profiles
    /// </summary>
    public static string ProfileDir
    {
        get
        {
            if (_appPath == null) GetAppPath();
            return Path.Combine(_appPath, "Profiles");
        }
    }

    /// <summary>
    /// The file name of the JSON file with all network data in
    /// </summary>
    public static string NetworkJsonFileName
    {
        get
        {
            if (_appPath == null) GetAppPath();
            return Path.Combine(_appPath, "Json", "network.json");
        }
    }
}