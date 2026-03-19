// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

namespace IpCommunicationSample.Backend.Bll.BusinessLogic;

/// <summary>
/// Current implementation of <see cref="IDeviceStateManagerFactory"/> delivering <see cref="TncpBackendDeviceStateManager"/> instances
/// </summary>
public class TncpBackendDeviceStateManagerFactory: IDeviceStateManagerFactory
{
    /// <summary>
    /// Create an instance of <see cref="IDeviceStateManager"/> for a certain device
    /// </summary>
    /// <param name="device">Current device</param>
    public IDeviceStateManager CreateInstance(IStateManagementDevice device)
    {
        return new TncpBackendDeviceStateManager(device);
    }
}