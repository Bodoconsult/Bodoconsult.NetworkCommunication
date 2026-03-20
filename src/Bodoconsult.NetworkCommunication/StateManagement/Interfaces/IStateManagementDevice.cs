// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

/// <summary>
/// Interface for state management devices 
/// </summary>
public interface IStateManagementDevice : IStateMachineContext, IOrderManagementDevice
{
    /// <summary>
    /// Current <see cref="IStateMachineDeviceBusinessLogicAdapter"/> instance
    /// </summary>
    IStateMachineDeviceBusinessLogicAdapter? DeviceBusinessLogicAdapter{ get; }

    /// <summary>
    /// Load the current <see cref="IStateMachineDeviceBusinessLogicAdapter"/> instance
    /// </summary>
    /// <param name="deviceStateManager">Current <see cref="IStateMachineDeviceBusinessLogicAdapter"/> instance</param>
    void LoadDeviceBusinessLogicAdapter(IStateMachineDeviceBusinessLogicAdapter deviceStateManager);
}