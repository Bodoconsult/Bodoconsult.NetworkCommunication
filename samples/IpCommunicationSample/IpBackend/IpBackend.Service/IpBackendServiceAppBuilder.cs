// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.App;
using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.BackgroundService.App;
using IpBackend.Bll.App;
using IpBackendService.DiContainerProvider;

namespace IpBackendService;

public class IpBackendServiceAppBuilder : BaseBackgroundServiceAppBuilder
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="appGlobals">Global app settings</param>
    public IpBackendServiceAppBuilder(IAppGlobals appGlobals) : base(appGlobals)
    {
        ConfigureHostBuilder(ConfigureAction);
    }

    private static void ConfigureAction(HostApplicationBuilder hostApplicationBuilder)
    {
        hostApplicationBuilder.Services.AddWindowsService(options =>
        {
            options.ServiceName = "IpBackendService";
        });
    }

    /// <summary>
    /// Load the <see cref="IAppBuilder.DiContainerServiceProviderPackage"/>
    /// </summary>
    public override void LoadDiContainerServiceProviderPackage()
    {
        var factory = new IpBackendServiceProductionDiContainerServiceProviderPackageFactory(AppGlobals);
        DiContainerServiceProviderPackage = factory.CreateInstance();
    }

    /// <summary>
    /// Process the configuration from <see cref="IAppStartParameter.ConfigFile"/>
    /// </summary>
    public override void ProcessConfiguration()
    {
        // Load basic config
        base.ProcessConfiguration();

        // Now get the root configuration element
        var root = AppStartProvider.AppConfigurationProvider.Configuration;

        if (root == null)
        {
            return;
        }

        var section = root.GetSection("AppStartParameter");

        // Get your derived IAppGlobals instance here to access added properties
        var appStartParams= (BackendAppStartParameter)AppGlobals.AppStartParameter;

        // Now get the requested config elements out of the root config element
        appStartParams.MaxDataLoggingFileSize = DefaultAppStartProvider.ReadLongProperty(section, "MaxDataLoggingFileSize", appStartParams.MaxDataLoggingFileSize);
    }
}