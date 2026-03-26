// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement.Configurations;

namespace Bodoconsult.NetworkCommunication.StateManagement.Factories;

/// <summary>
/// Current implementation of <see cref="IStateMachineStateFactory"/>
/// </summary>
public class StateMachineStateFactory : IStateMachineStateFactory
{
    private readonly Dictionary<string, IStateConfiguration> _stateConfigurations = new();

    /// <summary>
    /// Read-only list of all registered state builders
    /// </summary>
    public List<KeyValuePair<string, IStateConfiguration>> StateConfigurations => _stateConfigurations.ToList();


    /// <summary>
    /// Create a non order based state instance of the requested type
    /// </summary>
    /// <param name="currentContext">Current context</param>
    /// <param name="stateName">Name of the request state</param>
    /// <returns>State instance of the requested type</returns>
    public IStateMachineState CreateInstance(IStateMachineDevice currentContext, string stateName)
    {
        ArgumentNullException.ThrowIfNull(currentContext);

        if (!_stateConfigurations.TryGetValue(stateName, out var config))
        {
            throw new ArgumentException($"Builder for state {stateName} is not registered");
        }

        if (config is IOrderBasedActionStateConfiguration)
        {
            throw new ArgumentException("Use other overload of the method providing an IParameterSet instance for an order");
        }

        config = (IStateConfiguration)config.Clone();
        config.CurrentContext = currentContext;

        if (config.StateBuilderBuilder == null)
        {
            throw new ArgumentNullException(nameof(config.StateBuilderBuilder));
        }

        return config.StateBuilderBuilder.BuildState(config);
    }

    /// <summary>
    /// Create an order based state instance of the requested type
    /// </summary>
    /// <param name="currentContext">Current context</param>
    /// <param name="stateName">Name of the request state</param>
    /// <param name="parameterSets">Current parametersets to load in the orders to execute (if needed)</param>
    /// <returns>State instance of the requested type</returns>
    public IStateMachineState CreateInstance(IStateMachineDevice currentContext, string stateName, List<IParameterSet> parameterSets)
    {
        ArgumentNullException.ThrowIfNull(currentContext);

        if (!_stateConfigurations.TryGetValue(stateName, out var config))
        {
            throw new ArgumentException($"Builder for state {stateName} is not registered");
        }

        if (config is not IOrderBasedActionStateConfiguration)
        {
            throw new ArgumentException("Use other overload of the method NOT providing an IParameterSet instance for an order");
        }

        var obas = (IOrderBasedActionStateConfiguration)config.Clone();

        if (obas.StateBuilderBuilder == null)
        {
            throw new ArgumentNullException(nameof(obas.StateBuilderBuilder));
        }

        obas.CurrentContext = currentContext;
        obas.ParameterSets.AddRange( parameterSets);

        return obas.StateBuilderBuilder.BuildState(obas);
    }

    /// <summary>
    /// Register a state configuration (and add it to <see cref="IStateMachineStateFactory.StateConfigurations"/>)
    /// </summary>
    /// <param name="config">Config to register</param>
    public void RegisterConfiguration(IStateConfiguration config)
    {
        ArgumentNullException.ThrowIfNull(config.StateBuilderBuilder, "State builder must be loaded!");

        if (config.CurrentContext != null)
        {
            config.CurrentContext = null;
        }

        var checkResult = config switch
        {
            IOrderBasedActionStateConfiguration obas => OrderBasedActionStateConfiguration.IsValid(obas),
            IOrderlessActionStateConfiguration olas => OrderlessActionStateConfiguration.IsValid(olas),
            INoActionStateConfiguration nas => NoActionStateConfiguration.IsValid(nas),
            _ => null
        };

        if (checkResult is { Count: > 0 })
        {
            var s = checkResult.Aggregate("", (current, item) => current + item);
            throw new ArgumentException(s);
        }

        _stateConfigurations.Add(config.StateName, config);
    }

    /// <summary>
    /// Get the configuration for a certain state
    /// </summary>
    /// <param name="stateName">Name of the requested state</param>
    /// <returns>State configuration</returns>
    public IStateConfiguration? GetConfiguration(string stateName)
    {
        return _stateConfigurations.GetValueOrDefault(stateName);
    }
}