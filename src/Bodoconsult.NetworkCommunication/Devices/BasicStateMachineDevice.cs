// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

namespace Bodoconsult.NetworkCommunication.Devices;

/// <summary>
/// A basic device supporting state management and order management
/// </summary>
public class BasicStateMachineDevice : BaseStateManagementDevice
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="dataMessagingConfig">Current messaging config</param>
    /// <param name="clientNotificationManager">Current client notification manager</param>
    /// <param name="deviceStateCheckManager">Current device state check manager instances</param>
    public BasicStateMachineDevice(IDataMessagingConfig dataMessagingConfig,
        IOrderManagementClientNotificationManager clientNotificationManager,
        IDeviceStateCheckManager deviceStateCheckManager) :
        base(dataMessagingConfig, clientNotificationManager, deviceStateCheckManager)
    { }
}