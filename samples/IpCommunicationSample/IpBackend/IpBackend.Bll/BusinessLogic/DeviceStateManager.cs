// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

namespace IpCommunicationSample.Backend.Bll.BusinessLogic;



public class DeviceStateManager: IDeviceStateManager
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="device">Current device</param>
    public DeviceStateManager(IStateManagementDevice device)
    {
        Device = device;
    }

    /// <summary>
    /// Current device
    /// </summary>
    public IStateManagementDevice Device { get; }
}