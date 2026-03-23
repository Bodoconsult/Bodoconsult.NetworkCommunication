// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Devices;

/// <summary>
    /// A basic device supporting order management but no state management
    /// </summary>
    public class BasicOrderManagementDevice: BaseOrderManagementDevice
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="dataMessagingConfig">Current messaging config</param>
    /// <param name="clientNotificationManager">Current client notification manager</param>
    public BasicOrderManagementDevice(IDataMessagingConfig dataMessagingConfig, IOrderManagementClientNotificationManager clientNotificationManager) : base(dataMessagingConfig, clientNotificationManager)
    {
    }
}