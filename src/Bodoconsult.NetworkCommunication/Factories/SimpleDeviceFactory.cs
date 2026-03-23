// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.App.Abstractions;
using Bodoconsult.NetworkCommunication.Devices;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Factories;

/// <summary>
/// Factory for creating <see cref="BasicSimpleDevice"/> instances supporting only order management and no state management
/// </summary>
public class SimpleDeviceFactory : IIpDeviceFactory
{
    private readonly ICentralClientNotificationManager _clientNotificationManager;
    private readonly ICommunicationAdapterFactory _commAdapterFactory;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="clientNotificationManager">Current client notification manager</param>
    /// <param name="commAdapterFactory">Current communication adapter factory</param>
    public SimpleDeviceFactory(ICentralClientNotificationManager clientNotificationManager, IpCommunicationAdapterFactory commAdapterFactory)
    {
        _clientNotificationManager = clientNotificationManager;
        _commAdapterFactory = commAdapterFactory;
    }

    /// <summary>
    /// Create a device for order management only (no state management)
    /// </summary>
    /// <param name="dataMessagingConfig">Device configuration for data messaging</param>
    public IIpDevice CreateInstance(IIpDataMessagingConfig dataMessagingConfig)
    {
        var commAdapter = _commAdapterFactory.CreateInstance(dataMessagingConfig);

        var device = new BasicSimpleDevice(dataMessagingConfig, _clientNotificationManager);
        device.LoadCommAdapter(commAdapter);

        return device;
    }
}
