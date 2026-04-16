// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface enhancing <see cref="IDeviceManager"/> for IP devices supporting state management
/// </summary>
public interface IStateMachineDeviceManager : IDeviceManager
{
    /// <summary>
    /// Current device
    /// </summary>
    public IStateMachineDevice? Device { get; }

    /// <summary>
    /// Current <see cref="IStateMachineDeviceBusinessLogicAdapter"/> instance
    /// </summary>
    public IStateMachineDeviceBusinessLogicAdapter? DeviceBusinessLogicAdapter{ get; }
}