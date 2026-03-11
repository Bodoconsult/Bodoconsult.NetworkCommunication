// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement.States;

namespace Bodoconsult.NetworkCommunication.StateManagement.Builders;


/// <summary>
/// Base class for builder implementations creating <see cref="INoActionStateMachineState"/> instances
/// </summary>
public abstract class BaseNoActionStateMachineStateBuilder : BaseStateMachineStateBuilder
{
    /// <summary>
    /// Default ctor
    /// </summary>
    protected BaseNoActionStateMachineStateBuilder(int stateId, string stateName) : base(stateId , stateName, StateMachineStateTypes.NoActionState)
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

        ConfigureNoActionState(state1,config1 );
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

/// <summary>
/// Base class for <see cref="IStateMachineStateBuilder"/> implementations 
/// </summary>
public abstract class BaseStateMachineStateBuilder : IStateMachineStateBuilder
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="stateId">Unique ID of the state</param>
    /// <param name="stateName">State name</param>
    /// <param name="stateType">Current state type</param>
    protected BaseStateMachineStateBuilder(int stateId, string stateName, StateMachineStateTypes stateType)
    {
        StateId = stateId;
        StateName = stateName;
        StateType = stateType;
    }

    /// <summary>
    /// Current state type
    /// </summary>
    public StateMachineStateTypes StateType { get; }

    /// <summary>
    /// Unique ID of the state
    /// </summary>
    public int StateId { get; }

    /// <summary>
    /// Name of the state to build
    /// </summary>
    public string StateName { get; }


    /// <summary>
    /// Build the state
    /// </summary>
    /// <param name="config">Current configuration</param>
    /// <returns>State machine state instance</returns>
    public IStateMachineState BuildState(IStateConfiguration config)
    {
        IStateMachineState state;

        if (config.StateName != StateName)
        {
            throw new ArgumentException($"State name: mismatch between {config.StateName} and {StateName}");
        }

        switch (StateType)
        {
            case StateMachineStateTypes.OrderBasedActionState:
                state = new OrderBasedStateMachineState(config.CurrentContext, StateId, StateName);
                break;
            case StateMachineStateTypes.OrderlessActionState:
                state = new OrderlessActionStateMachineState(config.CurrentContext, StateId, StateName);
                break;
            case StateMachineStateTypes.NoActionState:
                state = new NoActionStateMachineState(config.CurrentContext, StateId, StateName);
                break;
            case StateMachineStateTypes.JobActionState:
                state = new JobStateMachineState(config.CurrentContext, StateId, StateName);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(StateType), StateType, null);
        }

        ConfigureState(state, config);
        return state;
    }

    /// <summary>
    /// Configure the state
    /// </summary>
    /// <param name="state">Current <see cref="IStateMachineState"/> instance</param>
    /// <param name="config">Current state configuration</param>
    public virtual void ConfigureState(IStateMachineState state, IStateConfiguration config)
    {
        throw new NotSupportedException("Override method in derived classes!");
    }

    /// <summary>
    /// Configure an orderless action state
    /// </summary>
    /// <param name="noActionState"></param>
    public virtual void ConfigureOrderlessActionState(IOrderlessActionStateMachineState noActionState)
    {
        throw new NotSupportedException("Override method in derived class!");
    }

    /// <summary>
    /// Configure an order based action state
    /// </summary>
    /// <param name="orderBasedActionState">Current <see cref="IOrderBasedActionStateMachineState"/> instance</param>
    public virtual void ConfigureOrderBasedActionState(IOrderBasedActionStateMachineState orderBasedActionState)
    {
        throw new NotSupportedException("Override method in derived class!");
    }

    /// <summary>
    /// Configure a job based action state
    /// </summary>
    /// <param name="jobActionState">Current <see cref="IJobStateMachineState"/> instance</param>
    public virtual void ConfigureJobActionState(IJobStateMachineState jobActionState)
    {
        throw new NotSupportedException("Override method in derived class!");
    }
}