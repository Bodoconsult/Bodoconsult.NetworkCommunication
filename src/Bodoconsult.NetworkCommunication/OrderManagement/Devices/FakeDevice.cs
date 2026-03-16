// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.OrderManagement.Devices;

/// <summary>
/// A fake device supporting order management
/// </summary>
public class FakeNoStateMachineDevice : BaseOrderManagementDevice, INoStateMachineOrderManagementDevice
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="dataMessagingConfig">Current messaging config</param>
    /// <param name="clientNotificationManager">Current client notification manager</param>
    public FakeNoStateMachineDevice(IDataMessagingConfig dataMessagingConfig, IOrderManagementClientNotificationManager clientNotificationManager) : base(dataMessagingConfig, clientNotificationManager)
    { }

    /// <summary>
    /// Handle an async received message without state machine
    /// </summary>
    public NoStateMachineHandleAsyncMessageDelegate? NoStateMachineHandleAsyncMessageDelegate { get; set; }

    /// <summary>
    /// Handle an error message received from the device without state machine
    /// </summary>
    public NoStateMachineHandleErrorMessageDelegate? NoStateMachineHandleErrorMessageDelegate { get; set; }
}