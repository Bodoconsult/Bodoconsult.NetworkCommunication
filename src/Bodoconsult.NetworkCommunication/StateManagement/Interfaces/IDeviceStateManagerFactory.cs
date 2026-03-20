// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

/// <summary>
/// Factory for creating <see cref="IStateMachineDeviceBusinessLogicAdapter"/> instances
/// </summary>
public interface IDeviceBusinessLogicAdapterFactory
{
    /// <summary>
    /// Create an instance of <see cref="IStateMachineDeviceBusinessLogicAdapter"/> for a certain device
    /// </summary>
    /// <param name="device">Current device</param>
    IStateMachineDeviceBusinessLogicAdapter CreateInstance(IStateManagementDevice device);
}