// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.StateManagement.Builders;

/// <summary>
/// Base class for builder implementations creating <see cref="INoActionStateMachineState"/> instances
/// </summary>
public abstract class BaseNoActionStateMachineStateBuilder : BaseStateMachineStateBuilder
{
    /// <summary>
    /// Default ctor
    /// </summary>
    protected BaseNoActionStateMachineStateBuilder(int stateId, string stateName) : base(stateId, stateName, StateMachineStateTypes.NoActionState)
    { }

    /// <summary>
    /// Configure the state
    /// </summary>
    /// <param name="state">Current <see cref="IStateMachineState"/> instance</param>
    /// <param name="config">Current state configuration</param>
    public override void ConfigureState(IStateMachineState state, IStateConfiguration config)
    {
        if (state is not INoActionStateMachineState state1)
        {
            throw new ArgumentException($"Wrong state type {state.GetType().Name}");
        }

        if (config is not INoActionStateConfiguration config1)
        {
            throw new ArgumentException($"Wrong config type {config.GetType().Name}");
        }

        ConfigureNoActionState(state1, config1);
    }

    /// <summary>
    /// Configure a no action state
    /// </summary>
    /// <param name="noActionState">Current <see cref="INoActionStateMachineState"/> instance</param>
    /// <param name="config">Current state configuration</param>
    public virtual void ConfigureNoActionState(INoActionStateMachineState noActionState, INoActionStateConfiguration config)
    {
        throw new NotSupportedException("Override method in derived class!");
    }

}