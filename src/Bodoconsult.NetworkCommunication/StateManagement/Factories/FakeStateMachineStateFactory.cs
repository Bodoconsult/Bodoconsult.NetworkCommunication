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

    public IStateMachineState CreateInstance(IStateManagementDevice currentContext, string stateName, IParameterSet parameterSet)
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
}