// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

namespace Bodoconsult.NetworkCommunication.OrderManagement.Devices;

/// <summary>
/// A fake device supporting order management
/// </summary>
public class FakeStateMachineDevice : BaseStateManagementDevice
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="dataMessagingConfig">Current messaging config</param>
    /// <param name="clientNotificationManager">Current client notification manager</param>
    /// <param name="stateMachineStateFactory">Current implementation of <see cref="IStateMachineStateFactory"/></param>
    /// <param name="deviceStateCheckManager">Current device state check manager instances</param>
    public FakeStateMachineDevice(IDataMessagingConfig dataMessagingConfig,
        IOrderManagementClientNotificationManager clientNotificationManager,
        IStateMachineStateFactory stateMachineStateFactory, IDeviceStateCheckManager deviceStateCheckManager) :
        base(dataMessagingConfig, clientNotificationManager, stateMachineStateFactory, deviceStateCheckManager)
    { }

    /// <summary>
    /// Load a comm adapter
    /// </summary>
    /// <param name="commAdapter">Current comm adapter</param>
    public void LoadCommAdapter(ICommunicationAdapter commAdapter)
    {
        CommunicationAdapter = commAdapter;
    }
}