// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

namespace Bodoconsult.NetworkCommunication.StateManagement.Builders;

/// <summary>
/// Base class for builder implementations creating <see cref="IJobStateMachineState"/> instances
/// </summary>
public abstract class BaseJobStateMachineStateBuilder : BaseStateMachineStateBuilder
{
    /// <summary>
    /// Default ctor
    /// </summary>
    protected BaseJobStateMachineStateBuilder(int stateId, string stateName) : base(stateId, stateName, StateMachineStateTypes.JobActionState)
    { }

    /// <summary>
    /// Configure the state
    /// </summary>
    /// <param name="state">Current <see cref="IStateMachineState"/> instance</param>
    /// <param name="config">Current state configuration</param>
    public override void ConfigureState(IStateMachineState state, IStateConfiguration config)
    {
        if (state is not IJobStateMachineState state1)
        {
            throw new ArgumentException($"Wrong state type {state.GetType().Name}");
        }

        if (config is not IJobStateConfiguration config1)
        {
            throw new ArgumentException($"Wrong config type {config.GetType().Name}");
        }

        // Set the parametersets for the orders to be created
        if (config1.ParameterSets.Count != config1.OrderConfigurations.Count)
        {
            throw new ArgumentException($"The number of parametersets {config1.ParameterSets.Count} must equal the number of order configurations {config1.OrderConfigurations.Count}!");
        }
        
        state1.ParameterSets.AddRange(config1.ParameterSets);
        state1.OrderConfigurations.AddRange(config1.OrderConfigurations);

        ConfigureOrderBasedActionState(state1, config1);
    }

    /// <summary>
    /// Configure a no action state
    /// </summary>
    /// <param name="state">Current <see cref="IJobStateMachineState"/> instance</param>
    /// <param name="config">Current state configuration</param>
    public virtual void ConfigureOrderBasedActionState(IJobStateMachineState state, IJobStateConfiguration config)
    {
        throw new NotSupportedException("Override method in derived class!");
    }
}