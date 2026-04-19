// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.BackgroundService.App;
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
}