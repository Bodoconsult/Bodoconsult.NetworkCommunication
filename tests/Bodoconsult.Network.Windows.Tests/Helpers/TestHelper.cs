// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using System.Reflection;
using System.Security;
using Bodoconsult.Network.Windows.Tests.Model;

namespace Bodoconsult.Network.Windows.Tests.Helpers;

public static class TestHelper
{
    private static string _testDataPath;


    public const string NameAppSettingsFile = "CoreWindowsAppSettings.json";

    public static readonly string OutputPath = Path.GetTempPath();

    public const string WorkPath = @"C:\Daten\Projekte\_work\Data\";


    static TestHelper()
    {
        if (!Directory.Exists(OutputPath))
        {
            Directory.CreateDirectory(OutputPath);
        }
    }
    public static string TestDataPath
    {
        get
        {

            if (!string.IsNullOrEmpty(_testDataPath))
            {
                return _testDataPath;
            }

            var path = new DirectoryInfo(new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName).Parent.Parent.Parent.FullName;

            _testDataPath = Path.Combine(path, "TestData");

            if (!Directory.Exists(_testDataPath))
            {
                Directory.CreateDirectory(_testDataPath);
            }

            return _testDataPath;
        }
    }

    /// <summary>
    /// Start an app by file name
    /// </summary>
    /// <param name="fileName"></param>
    public static void StartFile(string fileName)
    {

        if (!Debugger.IsAttached)
        {
            return;
        }

        Assert.That(File.Exists(fileName));

        var p = new Process { StartInfo = new ProcessStartInfo { UseShellExecute = true, FileName = fileName } };

        p.Start();

    }


    public static SecureString GetSecureString(string input)
    {

        var securepassword = new SecureString();
        foreach (var c in input)
        {
            securepassword.AppendChar(c);
        }

        return securepassword;
    }


    public static AppSettings GetAppSettings()
    {
        var fileName = Path.Combine(WorkPath, NameAppSettingsFile);

        var s = JsonHelper.LoadJsonFile<AppSettings>(fileName);

        s.UserName = PasswordHandler.Decrypt(s.UserName);
        s.Password = PasswordHandler.Decrypt(s.Password);

        return s;
    }



    public static string ReadString(string fileName)
    {
        return File.ReadAllText(fileName);
    }





}
