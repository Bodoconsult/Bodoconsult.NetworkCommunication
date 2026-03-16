// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

namespace Bodoconsult.NetworkCommunication.OrderManagement.Devices;

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
    /// <param name="stateMachineStateFactory">Current implementation of <see cref="IStateMachineStateFactory"/></param>
    /// <param name="deviceStateCheckManager">Current device state check manager instances</param>
    public BasicStateMachineDevice(IDataMessagingConfig dataMessagingConfig,
        IOrderManagementClientNotificationManager clientNotificationManager,
        IStateMachineStateFactory stateMachineStateFactory, IDeviceStateCheckManager deviceStateCheckManager) :
        base(dataMessagingConfig, clientNotificationManager, stateMachineStateFactory, deviceStateCheckManager)
    { }


}