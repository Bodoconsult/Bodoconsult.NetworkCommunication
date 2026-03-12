// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

/// <summary>
/// State machine state factory
/// </summary>
public interface IStateMachineStateFactory
{
    /// <summary>
    /// Read-only list of all registered state builders
    /// </summary>
    List<KeyValuePair<string, IStateConfiguration>> StateConfigurations { get; }

    /// <summary>
    /// Current context
    /// </summary>
    IStateManagementDevice? CurrentContext { get; }

    /// <summary>
    /// Create a state instance of the requested type
    /// </summary>
    /// <param name="stateName">Name of the request state</param>
    /// <returns>State instance of the requested type</returns>
    IStateMachineState CreateInstance(string stateName);

    /// <summary>
    /// Load the context
    /// </summary>
    /// <param name="context">Current context</param>
    void LoadContext(IStateManagementDevice context);

    /// <summary>
    /// Register a state configuration (and add it to <see cref="StateConfigurations"/>). <see cref="IStateConfiguration.CurrentContext"/> is always overriden with <see cref="CurrentContext"/>. So you do not have to set a value for <see cref="IStateConfiguration.CurrentContext"/> before calling <see cref="RegisterConfiguration"/>
    /// </summary>
    /// <param name="config">Config to register</param>
    void RegisterConfiguration(IStateConfiguration config);
}