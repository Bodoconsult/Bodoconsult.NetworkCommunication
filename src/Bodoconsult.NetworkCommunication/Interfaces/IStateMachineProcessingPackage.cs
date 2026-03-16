// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for state machine processing packages
/// </summary>
public interface IStateMachineProcessingPackage
{
    /// <summary>
    /// Current implementation of a device state checker or null
    /// </summary>
    IDeviceStateCheckManager? StateCheckManager { get; set; }

    /// <summary>
    /// Current state machine state factory to use or null
    /// </summary>
    IStateMachineStateFactory? StateMachineStateFactory { get; set; }
}