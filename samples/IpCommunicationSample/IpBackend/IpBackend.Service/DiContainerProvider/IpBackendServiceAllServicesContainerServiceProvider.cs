// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.DependencyInjection;
using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.CentralServices;
using Bodoconsult.App.ClientNotifications;
using Bodoconsult.App.Factories;
using Bodoconsult.NetworkCommunication.ClientNotifications;
using IpBackend.Bll.BusinessLogic;
using IpBackend.Bll.Interfaces;
using IpBackendService.App;

namespace IpBackendService.DiContainerProvider;

/// <summary>
/// Load all specific IpBackendService services to DI container. Intended mainly for production
/// </summary>
public class IpBackendServiceAllServicesContainerServiceProvider : IDiContainerServiceProvider
{
    /// <summary>
    /// Add DI container services to a DI container
    /// </summary>
    /// <param name="diContainer">Current DI container</param>
    public void AddServices(DiContainer diContainer)
    {
        // Load all other services required for the app now
        diContainer.AddSingleton<IAppDateService, AppDateService>();
        diContainer.AddSingleton<IAppEventSourceFactory, FakeAppEventSourceFactory>();
        diContainer.AddSingleton<IBackendManager, BackendManager>();

        diContainer.AddSingleton<IClientNotificationLicenseManager, FakeClientNotificationLicenseManager>();
        diContainer.AddSingleton<IClientMessagingService, BasicBtcpNetworkingClientMessagingService>();
        diContainer.AddSingleton<IClientManager, ClientManager>();
        diContainer.AddSingleton<IClientMessagingBusinessDelegate, ClientMessagingBusinessDelegate>();

        diContainer.AddSingleton<IApplicationService, IpBackendServiceService>();

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