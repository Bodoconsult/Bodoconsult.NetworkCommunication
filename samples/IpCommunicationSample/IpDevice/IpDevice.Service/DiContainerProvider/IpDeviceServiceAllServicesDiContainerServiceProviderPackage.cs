// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.DependencyInjection;
using Bodoconsult.NetworkCommunication.DependencyInjection;

namespace IpDeviceService.DiContainerProvider;

/// <summary>
/// Load all the complete package of IpDeviceService services based on GRPC to DI container. Intended mainly for production
/// </summary>
public class IpDeviceServiceAllServicesDiContainerServiceProviderPackage : BaseDiContainerServiceProviderPackage
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="appGlobals">Current app globals</param>
    public IpDeviceServiceAllServicesDiContainerServiceProviderPackage(IAppGlobals appGlobals) : base(appGlobals)
    {
        DoNotBuildDiContainer = true;

        // Basic app services
        IDiContainerServiceProvider provider = new BasicAppServicesConfig1ContainerServiceProvider(appGlobals);
        ServiceProviders.Add(provider);

        // Performance measurement
        provider = new ApmDiContainerServiceProvider(appGlobals.AppStartParameter, appGlobals.StatusMessageDelegate);
        ServiceProviders.Add(provider);

        // App default logging
        provider = new DefaultAppLoggerDiContainerServiceProvider(appGlobals.LoggingConfig, appGlobals.Logger);
        ServiceProviders.Add(provider);

        // Add networking features
        provider = new SimpleDeviceDataMessagingDiContainerServiceProvider();
        ServiceProviders.Add(provider);

        // IpDeviceService specific services
        provider = new IpDeviceServiceAllServicesContainerServiceProvider();
        ServiceProviders.Add(provider);
    }
}