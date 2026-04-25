// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Devices;

/// <summary>
/// A fake device supporting only order management and no state management
/// </summary>
public class FakeOrderManagementDevice : BaseOrderManagementDevice, IOnlyOrderManagementDevice
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="dataMessagingConfig">Current messaging config</param>
    /// <param name="clientNotificationManager">Current client notification manager</param>
    public FakeOrderManagementDevice(IDataMessagingConfig dataMessagingConfig, IOrderManagementClientNotificationManager clientNotificationManager) : base(dataMessagingConfig, clientNotificationManager)
    { }

    /// <summary>
    /// Handle an async received message without state machine
    /// </summary>
    public AppLayerHandleAsyncMessageDelegate? NoStateMachineHandleAsyncMessageDelegate { get; set; }

    /// <summary>
    /// Handle an error message received from the device without state machine
    /// </summary>
    public NoStateMachineHandleErrorMessageDelegate? NoStateMachineHandleErrorMessageDelegate { get; set; }
}