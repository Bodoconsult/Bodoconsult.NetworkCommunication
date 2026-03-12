// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;

namespace Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

/// <summary>
/// Interface for states performing an action not based on order management orders
/// </summary>
public interface IOrderlessActionStateMachineState : IStateMachineState
{
    /// <summary>
    /// Delegate to be executed from an orderless state machine state
    /// </summary>
    ExecuteActionForStateDelegate? ExecuteActionForStateDelegate { get; set; }

    /// <summary>
    /// Execute the action defined with <see cref="ExecuteActionForStateDelegate"/> for this state
    /// </summary>
    void Execute();
}