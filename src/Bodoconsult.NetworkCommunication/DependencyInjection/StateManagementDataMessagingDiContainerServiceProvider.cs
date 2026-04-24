// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Abstractions.DependencyInjection;
using Bodoconsult.App.Abstractions.Interfaces;
using Bodoconsult.App.BusinessTransactions;
using Bodoconsult.App.Interfaces;
using Bodoconsult.App.Logging;
using Bodoconsult.NetworkCommunication.ClientNotifications;
using Bodoconsult.NetworkCommunication.Factories;
using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.Processors;
using Bodoconsult.NetworkCommunication.Protocols.TcpIp;

namespace Bodoconsult.NetworkCommunication.DependencyInjection;

/// <summary>
/// Provider adding default services for data messaging with order management and state management
/// </summary>
public class StateManagementDataMessagingDiContainerServiceProvider : IDiContainerServiceProvider
{
    /// <summary>
    /// Is the client notification system active?
    /// </summary>
    public bool IsClientNoticationActive { get; set; }

    /// <summary>
    /// Add DI container services to a DI container
    /// </summary>
    /// <param name="diContainer">Current DI container</param>
    public void AddServices(DiContainer diContainer)
    {
        diContainer.AddSingleton<IBusinessTransactionManager, BusinessTransactionManager>();
        diContainer.AddSingleton<ISyncOrderManager, SyncOrderManager>();
        diContainer.AddSingleton<ISocketProxyFactory, SocketProxyFactory>();
        diContainer.AddSingleton<IMonitorLoggerFactoryFactory, MonitorLoggerFactoryFactory>();
        diContainer.AddSingleton<ITcpIpListenerManager, TcpIpListenerManager>();
        diContainer.AddSingleton<ISendPacketProcessFactory, SendPacketProcessFactory>();
        diContainer.AddSingleton<ICommunicationHandlerFactory, IpCommunicationHandlerFactory>();
        diContainer.AddSingleton<ICommunicationAdapterFactory, IpCommunicationAdapterFactory>();
        diContainer.AddSingleton<IDuplexIoFactory, IpDuplexIoFactory>();

        if (IsClientNoticationActive)
        {
            diContainer.AddSingleton<ICentralClientNotificationManager, BasicClientNotificationManager>();
            diContainer.AddSingleton<IOrderManagementClientNotificationManager, BasicClientNotificationManager>();
        }
        else
        {
            var cnm = new DoNothingOrderManagementClientNotificationManager();
            diContainer.AddSingleton<ICentralClientNotificationManager>(cnm);
            diContainer.AddSingleton<IOrderManagementClientNotificationManager>(cnm);
        }

        diContainer.AddSingleton<IOrderIdGenerator, DefaultOrderIdGenerator>();
        diContainer.AddSingleton<IOrderFactory, OrderFactory>();
        diContainer.AddSingleton<IOrderReceiverFactory, OrderReceiverFactory>();
        diContainer.AddSingleton<IRequestProcessorFactoryFactory, RequestProcessorFactoryFactory>();
        diContainer.AddSingleton<IRequestStepProcessorFactoryFactory, RequestStepProcessorFactoryFactory>();
        diContainer.AddSingleton<IOrderPipelineFactory, OrderPipelineFactory>();
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