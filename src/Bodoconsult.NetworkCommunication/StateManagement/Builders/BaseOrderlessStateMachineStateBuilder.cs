// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.StateManagement.Builders;

/// <summary>
/// Base class for builder implementations creating <see cref="IOrderlessActionStateMachineState"/> instances
/// </summary>
public abstract class BaseOrderlessStateMachineStateBuilder : BaseStateMachineStateBuilder
{
    /// <summary>
    /// Default ctor
    /// </summary>
    protected BaseOrderlessStateMachineStateBuilder(int stateId, string stateName) : base(stateId, stateName, StateMachineStateTypes.OrderlessActionState)
    { }

    /// <summary>
    /// Configure the state
    /// </summary>
    /// <param name="state">Current <see cref="IStateMachineState"/> instance</param>
    /// <param name="config">Current state configuration</param>
    public override void ConfigureState(IStateMachineState state, IStateConfiguration config)
    {
        if (state is not IOrderlessActionStateMachineState state1)
        {
            throw new ArgumentException($"Wrong state type {state.GetType().Name}");
        }

        if (config is not IOrderlessActionStateConfiguration config1)
        {
            throw new ArgumentException($"Wrong config type {config.GetType().Name}");
        }

        ConfigureOrderlessActionState(state1, config1);
    }

    /// <summary>
    /// Configure a no action state
    /// </summary>
    /// <param name="state">Current <see cref="IOrderlessActionStateMachineState"/> instance</param>
    /// <param name="config">Current state configuration</param>
    public virtual void ConfigureOrderlessActionState(IOrderlessActionStateMachineState state, IOrderlessActionStateConfiguration config)
    {
        throw new NotSupportedException("Override method in derived class!");
    }
}