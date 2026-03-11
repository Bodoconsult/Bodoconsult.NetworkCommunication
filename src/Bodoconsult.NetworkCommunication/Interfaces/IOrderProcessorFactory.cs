// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Interfaces;
using IAppDateService = Bodoconsult.NetworkCommunication.App.Abstractions.IAppDateService;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for factories creating <see cref="IOrderProcessor"/> instances for IP devices
/// </summary>
public interface IOrderProcessorFactory
{
    /// <summary>
    /// Current datetime service for the app
    /// </summary>
    IAppDateService AppDateService { get; }

    /// <summary>
    /// Current client notification manager
    /// </summary>
    IOrderManagementClientNotificationManager ClientNotificationManager { get; }

    /// <summary>
    /// Current app bench logger
    /// </summary>
    IAppBenchProxy AppBenchProxy { get; }

    /// <summary>
    /// Current manager for running orders in a sync manner
    /// </summary>
    ISyncOrderManager SyncOrderManager { get; }

    /// <summary>
    /// Create an instance of <see cref="IOrderProcessor"/>
    /// </summary>
    /// <param name="device">Current device</param>
    /// <param name="orderPipeline">Current order pipeline</param>
    /// <returns>New instance of <see cref="IOrderProcessor"/></returns>
    IOrderProcessor CreateInstance(IOrderManagementDevice device,
        IOrderPipeline orderPipeline );
}