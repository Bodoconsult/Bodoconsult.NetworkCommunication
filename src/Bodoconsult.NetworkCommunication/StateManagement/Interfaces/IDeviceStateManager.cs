// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

/// <summary>
/// Interface for connecting state management with a device specific business logic
/// </summary>
public interface IDeviceStateManager
{
    /// <summary>
    /// Current device
    /// </summary>
    IStateManagementDevice Device { get; }

    /// <summary>
    /// Current state factory
    /// </summary>
    IStateMachineStateFactory? StateFactory { get; }
}