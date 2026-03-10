// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

/// <summary>
/// Interface for building state machine states
/// </summary>
public interface IStateMachineStateBuilder
{
    /// <summary>
    /// Current state type
    /// </summary>
    StateMachineStateTypes StateType { get; }

    /// <summary>
    /// Unique ID of the state
    /// </summary>
    int StateId { get; }

    /// <summary>
    /// Name of the state to build
    /// </summary>
    string StateName { get; }

    /// <summary>
    /// Build the state
    /// </summary>
    /// <param name="config">Current configuration</param>
    /// <returns>State machine state instance</returns>
    IStateMachineState BuildState(IStateConfiguration config);

    /// <summary>
    /// Configure the state
    /// </summary>
    /// <param name="state">Current <see cref="IStateMachineState"/> instance</param>
    /// <param name="config">Current state configuration</param>
    void ConfigureState(IStateMachineState state, IStateConfiguration config);

    /// <summary>
    /// Configure an orderless action state
    /// </summary>
    /// <param name="orderlessActionState">Current <see cref="IOrderlessActionStateMachineState"/> instance</param>
    void ConfigureOrderlessActionState(IOrderlessActionStateMachineState orderlessActionState);

    /// <summary>
    /// Configure an order based action state
    /// </summary>
    /// <param name="orderBasedActionState">Current <see cref="IOrderBasedActionStateMachineState"/> instance</param>
    void ConfigureOrderBasedActionState(IOrderBasedActionStateMachineState orderBasedActionState);

    /// <summary>
    /// Configure a job based action state
    /// </summary>
    /// <param name="jobActionState">Current <see cref="IJobStateMachineState"/> instance</param>
    void ConfigureJobActionState(IJobStateMachineState jobActionState);
}