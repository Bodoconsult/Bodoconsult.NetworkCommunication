// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Extensions;
using Bodoconsult.NetworkCommunication.Tests.App;
using Bodoconsult.NetworkCommunication.Tests.Helpers;
using System.Diagnostics;

namespace Bodoconsult.NetworkCommunication.Tests;

/// <summary>
/// Setup for the assembly for all tests
/// </summary>
[SetUpFixture]
public static class AssemblySetup
{
    /// <summary>
    /// At startup of the assembly
    /// </summary>
    [OneTimeSetUp]
    public static void AssemblyStartUp()
    {
        var globals = Globals.Instance;
        globals.LoggingConfig.AddDefaultLoggerProviderConfiguratorsForUiApp();

        // Set additional app start parameters as required
        var param = globals.AppStartParameter;
        param.AppName = "Network: Demo app";
        param.SoftwareTeam = "Robert Leisner";
        //param.LogoRessourcePath = "WinFormsConsoleApp1.Resources.logo.jpg";
        param.AppFolderName = "NetworkTests";
        param.DataPath = "NetworkTests";

        // Now start the app building process
        var builder = new MyDebugAppBuilder(globals);
#if !DEBUG
        AppDomain.CurrentDomain.UnhandledException += builder.CurrentDomainOnUnhandledException;
#endif

        // Load basic app metadata

        builder.LoadBasicSettings();

        // Process the config file
        builder.ProcessConfiguration();

        // Now load the globally needed settings
        builder.LoadGlobalSettings();

        ArgumentNullException.ThrowIfNull(Globals.Instance.Logger);
        Globals.Instance.Logger.LogInformation("Starting tests...");
        TestDataHelper.Logger = Globals.Instance.Logger;

        //var traceFile = Path.Combine(param.LogfilePath,"TraceLog.txt");

        //if (File.Exists(traceFile))
        //{
        //    try
        //    {
        //        File.Delete(traceFile);
        //    }
        //    catch (Exception e)
        //    {
        //        Debug.Print(e.ToString());
        //    }
        //}

        //Trace.Listeners.Add(new TextWriterTraceListener(traceFile));
    }
}