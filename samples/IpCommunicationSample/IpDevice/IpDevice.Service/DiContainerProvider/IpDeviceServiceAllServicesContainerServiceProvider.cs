// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.DependencyInjection;
using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.CentralServices;
using IpDevice.Bll;
using IpDevice.Bll.Interfaces;
using IpDeviceService.App;

namespace IpDeviceService.DiContainerProvider;

/// <summary>
/// Load all specific IpDeviceService services to DI container. Intended mainly for production
/// </summary>
public class IpDeviceServiceAllServicesContainerServiceProvider : IDiContainerServiceProvider
{
    /// <summary>
    /// Add DI container services to a DI container
    /// </summary>
    /// <param name="diContainer">Current DI container</param>
    public void AddServices(DiContainer diContainer)
    {
        // Load all other services required for the app now
        //var factory = (IDiContainerServiceProviderPackageFactory)new IpDeviceServiceProductionDiContainerServiceProviderPackageFactory(Globals.Instance);

        //diContainer.AddSingleton(factory);

        // ToDo: move to base package
        diContainer.AddSingleton<IAppDateService, AppDateService>();

        diContainer.AddSingleton<IIpDeviceManager, IpDeviceManager>();

        diContainer.AddSingleton<IApplicationService, IpDeviceServiceService>();

        // ...
    }

    /// <summary>
    /// Late bind DI container references to avoid circular DI references
    /// </summary>
    /// <param name="diContainer"></param>
    public void LateBindObjects(DiContainer diContainer)
    {
        //// Example 1: Load the job scheduler now
        //var scheduler = diContainer.Get<IJobSchedulerManagementDelegate>();
        //scheduler.StartJobScheduler();

        //// Example 2: Load business transactions
        //var btl = diContainer.Get<IBusinessTransactionLoader>();
        //btl.LoadProviders();
    }
}