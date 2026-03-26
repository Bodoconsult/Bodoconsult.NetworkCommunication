// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

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

public interface ISimpleDeviceManager : IDeviceManager
{
    /// <summary>
    /// Current <see cref="IStateMachineDeviceBusinessLogicAdapter"/> instance
    /// </summary>
    public ISimpleDeviceBusinessLogicAdapter? DeviceBusinessLogicAdapter { get; }
}