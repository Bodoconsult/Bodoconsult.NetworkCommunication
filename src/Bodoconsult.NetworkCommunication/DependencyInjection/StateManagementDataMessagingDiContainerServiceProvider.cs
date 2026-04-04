// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.DependencyInjection;
using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.Interfaces;
using Bodoconsult.App.Logging;
using Bodoconsult.NetworkCommunication.ClientNotifications;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.Processors;

namespace Bodoconsult.NetworkCommunication.DependencyInjection;

/// <summary>
/// Provider adding default services for data messaging with order management and state management
/// </summary>
public class StateManagementDataMessagingDiContainerServiceProvider : IDiContainerServiceProvider
{
    /// <summary>
    /// Add DI container services to a DI container
    /// </summary>
    /// <param name="diContainer">Current DI container</param>
    public void AddServices(DiContainer diContainer)
    {
        diContainer.AddSingleton<ISyncOrderManager, SyncOrderManager>();
        diContainer.AddSingleton<IMonitorLoggerFactoryFactory, MonitorLoggerFactoryFactory>();
        diContainer.AddSingleton<ISendPacketProcessFactory, SendPacketProcessFactory>();
        diContainer.AddSingleton<ICommunicationHandlerFactory, IpCommunicationHandlerFactory>();
        diContainer.AddSingleton<ICommunicationAdapterFactory, IpCommunicationAdapterFactory>();
        diContainer.AddSingleton<IDuplexIoFactory, IpDuplexIoFactory>();

        var cnm = new DoNothingOrderManagementClientNotificationManager();
        diContainer.AddSingleton<ICentralClientNotificationManager>(cnm);
        diContainer.AddSingleton<IOrderManagementClientNotificationManager>(cnm);
        diContainer.AddSingleton<ITcpIpListenerManager, ITcpIpListenerManager>();

        diContainer.AddSingleton<IOrderFactory, OrderFactory>();
        diContainer.AddSingleton<IOrderReceiverFactory, OrderReceiverFactory>();
        diContainer.AddSingleton<IRequestProcessorFactoryFactory, RequestProcessorFactoryFactory>();
        diContainer.AddSingleton<IRequestStepProcessorFactoryFactory, RequestStepProcessorFactoryFactory>();
        diContainer.AddSingleton<IOrderProcessorFactory, StateMachineOrderProcessorFactory>();
    }

    /// <summary>
    /// Late bind DI container references to avoid circular DI references
    /// </summary>
    /// <param name="diContainer">Current DI container</param>
    public void LateBindObjects(DiContainer diContainer)
    {
        // Do nothing
    }
}