// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.BackgroundService.App;
using IpDeviceService.DiContainerProvider;

namespace IpDeviceService;

public class IpDeviceServiceAppBuilder : BaseBackgroundServiceAppBuilder
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="appGlobals">Global app settings</param>
    public IpDeviceServiceAppBuilder(IAppGlobals appGlobals) : base(appGlobals)
    {
        ConfigureHostBuilder(ConfigureAction);
    }

    private static void ConfigureAction(HostApplicationBuilder hostApplicationBuilder)
    {
        hostApplicationBuilder.Services.AddWindowsService(options =>
        {
            options.ServiceName = "IpDeviceService";
        });
    }

    /// <summary>
    /// Load the <see cref="IAppBuilder.DiContainerServiceProviderPackage"/>
    /// </summary>
    public override void LoadDiContainerServiceProviderPackage()
    {
        var factory = new IpDeviceServiceProductionDiContainerServiceProviderPackageFactory(AppGlobals);
        DiContainerServiceProviderPackage = factory.CreateInstance();
    }
}