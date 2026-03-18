// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

namespace Bodoconsult.NetworkCommunication.StateManagement.Factories;

/// <summary>
/// Fake implementation of <see cref="IStateMachineStateFactory"/>
/// </summary>
public class FakeStateMachineStateFactory : IStateMachineStateFactory
{
    private readonly Dictionary<string, IStateConfiguration> _stateConfigurations = new();

    /// <summary>
    /// Read-only list of all registered state builders
    /// </summary>
    public List<KeyValuePair<string, IStateConfiguration>> StateConfigurations => _stateConfigurations.ToList();


    /// <summary>
    /// Create a state instance of the requested type
    /// </summary>
    /// <param name="currentContext">Current context</param>
    /// <param name="stateName">Name of the request state</param>
    /// <returns>State instance of the requested type</returns>
    public IStateMachineState CreateInstance(IStateManagementDevice currentContext, string stateName)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Create an order based state instance of the requested type
    /// </summary>
    /// <param name="currentContext">Current context</param>
    /// <param name="stateName">Name of the request state</param>
    /// <param name="parameterSets">Current parametersets to load in the orders to execute (if needed)</param>
    /// <returns>State instance of the requested type</returns>
    public IStateMachineState CreateInstance(IStateManagementDevice currentContext, string stateName, List<IParameterSet> parameterSets)
    {
        throw new NotImplementedException();
    }


    /// <summary>
    /// Register a state configuration (and add it to <see cref="IStateMachineStateFactory.StateConfigurations"/>). <see cref="IStateConfiguration.CurrentContext"/> is always overriden with <see cref="IStateMachineStateFactory.CurrentContext"/>. So you do not have to set a value for <see cref="IStateConfiguration.CurrentContext"/> before calling <see cref="IStateMachineStateFactory.RegisterConfiguration"/>
    /// </summary>
    /// <param name="config">Config to register</param>
    public void RegisterConfiguration(IStateConfiguration config)
    {
        if (config.StateBuilderBuilder == null)
        {
            throw new ArgumentNullException(nameof(config.StateBuilderBuilder));
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
        throw new NotImplementedException();
    }
}