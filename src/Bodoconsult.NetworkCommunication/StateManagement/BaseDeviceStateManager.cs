// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

namespace IpCommunicationSample.Backend.Bll.BusinessLogic;

/// <summary>
/// Base class for <see cref="IDeviceStateManager"/> implementations
/// </summary>
public abstract class BaseDeviceStateManager: IDeviceStateManager
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="device">Current device</param>
    protected BaseDeviceStateManager(IStateManagementDevice device)
    {
        Device = device;
    }

    /// <summary>
    /// Current device
    /// </summary>
    public IStateManagementDevice Device { get; }

    /// <summary>
    /// Current state factory
    /// </summary>
    public IStateMachineStateFactory? StateFactory { get; private set; }

    /// <summary>
    /// Load the state factory
    /// </summary>
    /// <param name="stateFactory">Current state factory</param>
    public void LoadStateFactory(IStateMachineStateFactory stateFactory)
    {
        StateFactory = stateFactory;
    }
}