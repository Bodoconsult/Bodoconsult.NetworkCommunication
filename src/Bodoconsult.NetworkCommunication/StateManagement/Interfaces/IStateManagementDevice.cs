// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

/// <summary>
/// Interface for state management devices 
/// </summary>
public interface IStateManagementDevice : IStateMachineContext, IOrderManagementDevice
{
    /// <summary>
    /// Current <see cref="IDeviceStateManager"/> instance
    /// </summary>
    IDeviceStateManager? DeviceStateManager { get; }

    /// <summary>
    /// Load the current <see cref="IDeviceStateManager"/> instance
    /// </summary>
    /// <param name="deviceStateManager">Current <see cref="IDeviceStateManager"/> instance</param>
    void LoadDeviceStateManager(IDeviceStateManager deviceStateManager);
}