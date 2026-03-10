// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

/// <summary>
/// Base interface for state machine state builder configurations. State machine state configurations have to set per device
/// </summary>
public interface IStateConfiguration
{
    /// <summary>
    /// Current context
    /// </summary>
    IStateManagementDevice CurrentContext { get; set; }

    /// <summary>
    /// Name of the state to configure
    /// </summary>
    string StateName { get; }

    /// <summary>
    /// State builder to use
    /// </summary>
    IStateMachineStateBuilder StateBuilderBuilder { get; set; }
}