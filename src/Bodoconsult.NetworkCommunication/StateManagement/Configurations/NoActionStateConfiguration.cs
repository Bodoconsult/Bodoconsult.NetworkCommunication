// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

namespace Bodoconsult.NetworkCommunication.StateManagement.Configurations;

/// <summary>
/// Current implementation of <see cref="INoActionStateConfiguration"/>. State machine state configurations have to set per device
/// </summary>
public class NoActionStateConfiguration : INoActionStateConfiguration
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="stateName">Name of the state to configure</param>
    public NoActionStateConfiguration(string stateName)
    {
        StateName = stateName;
    }

    /// <summary>
    /// Current context
    /// </summary>
    public IStateManagementDevice CurrentContext { get; set; }

    /// <summary>
    /// Name of the state to configure
    /// </summary>
    public string StateName { get; }

    /// <summary>
    /// State builder to use
    /// </summary>
    public IStateMachineStateBuilder StateBuilderBuilder { get; set; }

    /// <summary>
    /// Delegate to be executed from a no action state machine state to check if job states are required to be executed
    /// </summary>
    public CheckJobstatesActionForStateDelegate CheckJobstatesActionForStateDelegate { get; set; }
}