// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Devices;

/// <summary>
/// A basic device supporting order management but no state management
/// </summary>
public class BasicOrderManagementDevice : BaseOrderManagementDevice, IOnlyOrderManagementDevice
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="dataMessagingConfig">Current messaging config</param>
    /// <param name="clientNotificationManager">Current client notification manager</param>
    public BasicOrderManagementDevice(IDataMessagingConfig dataMessagingConfig, IOrderManagementClientNotificationManager clientNotificationManager) : 
        base(dataMessagingConfig, clientNotificationManager)
    { }

    /// <summary>
    /// Load the current <see cref="IDeviceBusinessLogicAdapter"/> instance
    /// </summary>
    /// <param name="businessLogicAdapter">Current <see cref="IDeviceBusinessLogicAdapter"/> instance</param>
    public override void LoadDeviceBusinessLogicAdapter(IDeviceBusinessLogicAdapter businessLogicAdapter)
    {
        if (businessLogicAdapter is not IOrderManagementDeviceBusinessLogicAdapter o)
        {
            throw new ArgumentException($"businessLogicAdapter is not {nameof(IOrderManagementDeviceBusinessLogicAdapter)}");
        }

        DeviceBusinessLogicAdapter = o;
        OrderManagementDeviceBusinessLogicAdapter = o;
    }

    /// <summary>
    /// Handle an async received message without state machine
    /// </summary>
    public NoStateMachineHandleAsyncMessageDelegate? NoStateMachineHandleAsyncMessageDelegate { get; set; }

    /// <summary>
    /// Handle an error message received from the device without state machine
    /// </summary>
    public NoStateMachineHandleErrorMessageDelegate? NoStateMachineHandleErrorMessageDelegate { get; set; }
}