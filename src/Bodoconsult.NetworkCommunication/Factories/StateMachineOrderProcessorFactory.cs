// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.Processors;
using IAppDateService = Bodoconsult.NetworkCommunication.App.Abstractions.IAppDateService;

namespace Bodoconsult.NetworkCommunication.Factories;

/// <summary>
/// Factory for creating <see cref="StateMachineOrderProcessor"/> instances
/// </summary>
public class StateMachineOrderProcessorFactory : IOrderProcessorFactory
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="appDateService">Current datetime service for the app</param>
    /// <param name="syncOrderManager">Current manager for running orders in a sync manner</param>
    /// <param name="clientNotificationManager">Current client notification manager</param>
    /// <param name="appBenchProxy">Current app bench logger</param>
    public StateMachineOrderProcessorFactory(IAppDateService appDateService,
        ISyncOrderManager syncOrderManager,
        IOrderManagementClientNotificationManager clientNotificationManager, IAppBenchProxy appBenchProxy)
    {
        AppBenchProxy = appBenchProxy;
        AppDateService = appDateService;
        SyncOrderManager = syncOrderManager;
        ClientNotificationManager = clientNotificationManager;
    }

    /// <summary>
    /// Current datetime service for the app
    /// </summary>
    public IAppDateService AppDateService { get; }

    /// <summary>
    /// Current client notification manager
    /// </summary>
    public IOrderManagementClientNotificationManager ClientNotificationManager { get; }

    /// <summary>
    /// Current app bench logger
    /// </summary>
    public IAppBenchProxy AppBenchProxy { get; }

    /// <summary>
    /// Current manager for running orders in a sync manner
    /// </summary>
    public ISyncOrderManager SyncOrderManager { get; }

    /// <summary>
    /// Create an instance of <see cref="IOrderProcessor"/>
    /// </summary>
    /// <param name="device">Current device</param>
    /// <param name="orderPipeline">Current order pipeline</param>
    /// <returns>New instance of <see cref="IOrderProcessor"/></returns>
    public IOrderProcessor CreateInstance(IOrderManagementDevice device, IOrderPipeline orderPipeline)
    {
        if (device is not IStateMachineDevice stateMachineDevice)
        {
            throw new ArgumentException("Device must be implement IStateMachineDevice");
        }

        return new StateMachineOrderProcessor(stateMachineDevice, AppDateService, orderPipeline, SyncOrderManager,
            ClientNotificationManager, AppBenchProxy);
    }
}