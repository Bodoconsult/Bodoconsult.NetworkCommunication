// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.DependencyInjection;
using Bodoconsult.NetworkCommunication.DependencyInjection;

namespace IpBackendService.DiContainerProvider;

/// <summary>
/// Load all the complete package of IpBackendService services based on GRPC to DI container. Intended mainly for production
/// </summary>
public class IpBackendServiceAllServicesDiContainerServiceProviderPackage : BaseDiContainerServiceProviderPackage
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="appGlobals">Current app globals</param>
    public IpBackendServiceAllServicesDiContainerServiceProviderPackage(IAppGlobals appGlobals) : base(appGlobals)
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
        var provider1 = new StateManagementDataMessagingDiContainerServiceProvider
        {
            IsClientNoticationActive = true
        };
        ServiceProviders.Add(provider1);

        // SIpBackendService specific services
        provider = new IpBackendServiceAllServicesContainerServiceProvider();
        ServiceProviders.Add(provider);
    }
}