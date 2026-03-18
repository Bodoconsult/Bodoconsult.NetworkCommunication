// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

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
    /// Create a non order based state instance of the requested type
    /// </summary>
    /// <param name="currentContext">Current context</param>
    /// <param name="stateName">Name of the request state</param>
    /// <returns>State instance of the requested type</returns>
    IStateMachineState CreateInstance(IStateManagementDevice currentContext, string stateName);

    /// <summary>
    /// Create an order based state instance of the requested type
    /// </summary>
    /// <param name="currentContext">Current context</param>
    /// <param name="stateName">Name of the request state</param>
    /// <param name="parameterSets">Current parametersets to load in the orders to execute (if needed)</param>
    /// <returns>State instance of the requested type</returns>
    IStateMachineState CreateInstance(IStateManagementDevice currentContext, string stateName, List<IParameterSet> parameterSets);

    /// <summary>
    /// Register a state configuration (and add it to <see cref="StateConfigurations"/>)
    /// </summary>
    /// <param name="config">Config to register</param>
    void RegisterConfiguration(IStateConfiguration config);
}