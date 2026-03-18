// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.OrderManagement.Devices;

/// <summary>
/// A simple device supporting order management
/// </summary>
public class OrderManagementDevice: BaseOrderManagementDevice
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="dataMessagingConfig">Current messaging config</param>
    /// <param name="clientNotificationManager">Current client notification manager</param>
    public OrderManagementDevice(IDataMessagingConfig dataMessagingConfig, IOrderManagementClientNotificationManager clientNotificationManager) : base(dataMessagingConfig, clientNotificationManager)
    {
    }
}