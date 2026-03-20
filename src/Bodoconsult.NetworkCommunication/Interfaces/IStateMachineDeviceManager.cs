// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

namespace Bodoconsult.NetworkCommunication.Interfaces;

public interface IStateMachineDeviceManager : IDeviceManager
{
    /// <summary>
    /// Current device
    /// </summary>
    public IStateManagementDevice? Device { get; }

    /// <summary>
    /// Current <see cref="IStateMachineDeviceBusinessLogicAdapter"/> instance
    /// </summary>
    public IStateMachineDeviceBusinessLogicAdapter? DeviceBusinessLogicAdapter{ get; }
}