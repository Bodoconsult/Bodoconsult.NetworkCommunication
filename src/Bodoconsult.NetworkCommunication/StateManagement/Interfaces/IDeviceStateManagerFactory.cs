// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

/// <summary>
/// Factory for creating <see cref="IDeviceStateManager"/> instances
/// </summary>
public interface IDeviceStateManagerFactory
{
    /// <summary>
    /// Create an instance of <see cref="IDeviceStateManager"/> for a certain device
    /// </summary>
    /// <param name="device">Current device</param>
    IDeviceStateManager CreateInstance(IStateManagementDevice device);
}