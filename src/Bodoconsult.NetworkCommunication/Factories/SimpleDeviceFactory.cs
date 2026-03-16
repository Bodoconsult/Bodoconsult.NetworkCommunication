// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.OrderManagement.Devices;

namespace Bodoconsult.NetworkCommunication.Factories;

/// <summary>
/// Factory for creating <see cref="SimpleDevice"/> instances supporting only order management and no state management
/// </summary>
public class SimpleDeviceFactory : IOrderManagementDeviceFactory
{
    private readonly IOrderManagementClientNotificationManager _clientNotificationManager;
    private readonly IOrderManagementCommunicationAdapterFactory _commAdapterFactory;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="clientNotificationManager">Current client notification manager</param>
    /// <param name="commAdapterFactory">Current communication adapter factory</param>
    public SimpleDeviceFactory(IOrderManagementClientNotificationManager clientNotificationManager, IOrderManagementCommunicationAdapterFactory commAdapterFactory)
    {
        _clientNotificationManager= clientNotificationManager;
        _commAdapterFactory= commAdapterFactory;
    }

    /// <summary>
    /// Create a device for order management only (no state management)
    /// </summary>
    /// <param name="dataMessagingConfig">Device configuration for data messaging</param>
    public IOrderManagementDevice CreateInstance(IIpDataMessagingConfig dataMessagingConfig)
    {
        var commAdapter = _commAdapterFactory.CreateInstance(dataMessagingConfig);

        var device =  new SimpleDevice(dataMessagingConfig, _clientNotificationManager);
        device.LoadCommAdapter(commAdapter);

        return device;
    }
}