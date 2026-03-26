// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Devices;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Factories;

/// <summary>
/// Factory for creating a <see cref="BasicStateMachineDevice"/> supporting state management and order management
/// </summary>
public class BasicStateMachineDeviceFactory : IStateMachineDeviceFactory
{
    private readonly IOrderManagementClientNotificationManager _clientNotificationManager;
    private readonly ICommunicationAdapterFactory _commAdapterFactory;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="clientNotificationManager">Current client notification manager</param>
    /// <param name="commAdapterFactory">Current communication adapter factory</param>
    public BasicStateMachineDeviceFactory(IOrderManagementClientNotificationManager clientNotificationManager, ICommunicationAdapterFactory commAdapterFactory)
    {
        _clientNotificationManager = clientNotificationManager;
        _commAdapterFactory = commAdapterFactory;
    }

    /// <summary>
    /// Create a device for order management and state management
    /// </summary>
    /// <param name="dataMessagingConfig">Device configuration for data messaging</param>
    /// <param name="deviceStateCheckManager">Curent state checker implementation</param>
    public IStateMachineDevice CreateInstance(IIpDataMessagingConfig dataMessagingConfig, IDeviceStateCheckManager deviceStateCheckManager)
    {
        var commAdapter = _commAdapterFactory.CreateInstance(dataMessagingConfig);
        
        var device = new BasicStateMachineDevice(dataMessagingConfig,
            _clientNotificationManager,
             deviceStateCheckManager);
        device.LoadCommAdapter(commAdapter);

        return device;
    }
}