// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

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
    /// Current context
    /// </summary>
    public IStateManagementDevice? CurrentContext { get; private set; }

    /// <summary>
    /// Create a state instance of the requested type
    /// </summary>
    /// <param name="stateName">Name of the request state</param>
    /// <returns>State instance of the requested type</returns>
    public IStateMachineState CreateInstance(string stateName)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Load the context
    /// </summary>
    /// <param name="context">Current context</param>
    public void LoadContext(IStateManagementDevice context)
    {
        CurrentContext = context;
    }

    /// <summary>
    /// Register a state configuration (and add it to <see cref="IStateMachineStateFactory.StateConfigurations"/>). <see cref="IStateConfiguration.CurrentContext"/> is always overriden with <see cref="IStateMachineStateFactory.CurrentContext"/>. So you do not have to set a value for <see cref="IStateConfiguration.CurrentContext"/> before calling <see cref="IStateMachineStateFactory.RegisterConfiguration"/>
    /// </summary>
    /// <param name="config">Config to register</param>
    public void RegisterConfiguration(IStateConfiguration config)
    {
        if (CurrentContext == null)
        {
            throw new ArgumentNullException(nameof(CurrentContext), "Call LoadContext() before calling RegisterConfiguration()!");
        }

        if (config.StateBuilderBuilder == null)
        {
            throw new ArgumentNullException(nameof(config.StateBuilderBuilder));
        }

        config.CurrentContext = CurrentContext;

        _stateConfigurations.Add(config.StateName, config);
    }
}