// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.Devices;
using Bodoconsult.NetworkCommunication.StateManagement.StateCheckManagers;

namespace Bodoconsult.NetworkCommunication.Factories;

/// <summary>
/// Factory for creating a <see cref="BasicStateMachineDevice"/> supporting state management and order management
/// </summary>
public class BasicManagementDeviceFactory : IOrderManagementDeviceFactory
{
    private readonly IOrderManagementClientNotificationManager _clientNotificationManager;
    private readonly IOrderManagementCommunicationAdapterFactory _commAdapterFactory;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="clientNotificationManager">Current client notification manager</param>
    /// <param name="commAdapterFactory">Current communication adapter factory</param>
    public BasicManagementDeviceFactory(IOrderManagementClientNotificationManager clientNotificationManager, IOrderManagementCommunicationAdapterFactory commAdapterFactory)
    {
        _clientNotificationManager = clientNotificationManager;
        _commAdapterFactory = commAdapterFactory;
    }

    /// <summary>
    /// Create a device for order management only (no state management)
    /// </summary>
    /// <param name="dataMessagingConfig">Device configuration for data messaging</param>
    public IOrderManagementDevice CreateInstance(IIpDataMessagingConfig dataMessagingConfig)
    {
        ArgumentNullException.ThrowIfNull(dataMessagingConfig.StateMachineProcessingPackage);
        ArgumentNullException.ThrowIfNull(dataMessagingConfig.StateMachineProcessingPackage.StateMachineStateFactory);

        var commAdapter = _commAdapterFactory.CreateInstance(dataMessagingConfig);

        var deviceStateCheckManager = dataMessagingConfig.StateMachineProcessingPackage.StateCheckManager ??
                                      new DoNothingStateCheckManager();
        
        var device = new BasicStateMachineDevice(dataMessagingConfig,
            _clientNotificationManager,
            dataMessagingConfig.StateMachineProcessingPackage.StateMachineStateFactory, deviceStateCheckManager);
        device.LoadCommAdapter(commAdapter);

        return device;
    }
}