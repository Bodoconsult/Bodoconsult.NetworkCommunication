// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for state management devices 
/// </summary>
public interface IStateMachineDevice : IStateMachineContext, IOrderManagementDevice
{
    /// <summary>
    /// Current <see cref="IOrderManagementDeviceBusinessLogicAdapter"/> instance
    /// </summary>
    public IStateMachineDeviceBusinessLogicAdapter? StateMachineDeviceBusinessLogicAdapter { get; }

    /// <summary>
    /// Check the connection: if not connected call request DeviceOfflineState
    /// </summary>
    /// <returns>True if the device is connected and false if not and DeviceOfflineState is requested</returns>
    bool CheckConnection();
}