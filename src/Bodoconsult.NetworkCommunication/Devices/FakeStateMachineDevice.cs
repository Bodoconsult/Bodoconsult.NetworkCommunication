// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.Devices;

/// <summary>
/// A fake device supporting state management and order management
/// </summary>
public class FakeStateMachineDevice : BaseStateMachineDevice
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="dataMessagingConfig">Current messaging config</param>
    /// <param name="clientNotificationManager">Current client notification manager</param>
    /// <param name="deviceStateCheckManager">Current device state check manager instances</param>
    public FakeStateMachineDevice(IDataMessagingConfig dataMessagingConfig,
        IOrderManagementClientNotificationManager clientNotificationManager,
        IDeviceStateCheckManager deviceStateCheckManager) :
        base(dataMessagingConfig, clientNotificationManager, deviceStateCheckManager)
    { }

    /// <summary>
    /// Load a state as fake
    /// </summary>
    /// <param name="state">State to load as fake</param>
    public void SetFakeState(IOrderBasedActionStateMachineState state)
    {
        CurrentState = state;
    }
}