// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.App.Abstractions;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.OrderManagement.Devices;

/// <summary>
/// A simple device supporting only basic comm features like sending and receiving messages but no order management and no state management
/// </summary>
public class SimpleDevice : BaseIpDevice
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="dataMessagingConfig">Current messaging config</param>
    /// <param name="clientNotificationManager">Current client notification manager</param>
    public SimpleDevice(IDataMessagingConfig dataMessagingConfig, ICentralClientNotificationManager clientNotificationManager) : 
        base(dataMessagingConfig, clientNotificationManager)
    { }
}