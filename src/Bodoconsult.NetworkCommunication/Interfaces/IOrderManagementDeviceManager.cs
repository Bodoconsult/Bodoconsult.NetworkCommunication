// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface enhancing <see cref="IDeviceManager"/> for IP devices supporting order management
/// </summary>
public interface IOrderManagementDeviceManager : IDeviceManager
{
    /// <summary>
    /// Current device
    /// </summary>
    public IOrderManagementDevice? Device { get; }

    /// <summary>
    /// Current <see cref="IStateMachineDeviceBusinessLogicAdapter"/> instance
    /// </summary>
    public IOrderManagementDeviceBusinessLogicAdapter? DeviceBusinessLogicAdapter { get; }
}