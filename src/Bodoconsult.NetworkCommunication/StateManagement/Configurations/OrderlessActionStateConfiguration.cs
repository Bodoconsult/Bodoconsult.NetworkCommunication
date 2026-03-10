// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;
using Bodoconsult.NetworkCommunication.Delegates;

namespace Bodoconsult.NetworkCommunication.StateManagement.Configurations;

/// <summary>
/// Current implementation of <see cref="IOrderlessActionStateConfiguration"/>. State machine state configurations have to set per device
/// </summary>
public class OrderlessActionStateConfiguration : IOrderlessActionStateConfiguration
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="stateName">Name of the state to configure</param>
    public OrderlessActionStateConfiguration(string stateName)
    {
        StateName = stateName;
    }

    /// <summary>
    /// Current context
    /// </summary>
    public IStateManagementDevice CurrentContext { get; set; }

    /// <summary>
    /// Name of the state to configure
    /// </summary>
    public string StateName { get; }

    /// <summary>
    /// State builder to use
    /// </summary>
    public IStateMachineStateBuilder StateBuilderBuilder { get; set; }

    /// <summary>
    /// Delegate to be executed from an orderless state machine state
    /// </summary>
    public ExecuteActionForStateDelegate ExecuteActionForStateDelegate { get; set; }
}