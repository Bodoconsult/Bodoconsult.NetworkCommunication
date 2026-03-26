// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for states performing no action but simple stay in the state
/// </summary>
public interface INoActionStateMachineState : IStateMachineState
{
    /// <summary>
    /// Delegate to be executed from a no action state machine state to check if job states are required to be executed
    /// </summary>
    CheckJobstatesActionForStateDelegate? CheckJobstatesActionForStateDelegate { get; set; }

    /// <summary>
    /// Execute the action defined with <see cref="CheckJobstatesActionForStateDelegate"/> for this state to check if job states are required to be executed
    /// </summary>
    void CheckJobstates();

}