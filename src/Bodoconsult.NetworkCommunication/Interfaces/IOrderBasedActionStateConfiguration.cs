// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Configuration for a <see cref="IOrderBasedActionStateMachineState"/>. State machine state configurations have to set per device
/// </summary>
public interface IOrderBasedActionStateConfiguration: IStateConfiguration
{
    /// <summary>
    /// Delegate fired when an order was finished successfully to implement buisness logic for that event. This delegate method should set a new state to request (but not request it)
    /// </summary>
    OrderFinishedSucessfullyDelegate? OrderFinishedSucessfullyDelegate { get; set; }

    /// <summary>
    /// Delegate fired when an order was finished unsuccessfully to implement buisness logic for that event. This delegate method should set a new state to request (but not request it)
    /// </summary>
    OrderFinishedUnsucessfullyDelegate? OrderFinishedUnsucessfullyDelegate { get; set; }

    /// <summary>
    /// Parameter sets for the orders to be created for the state. The number of parameter sets must equal the number of <see cref="OrderConfigurations"/>
    /// </summary>
    List<IParameterSet> ParameterSets { get; }

    /// <summary>
    /// All configurations for orders to be executed for the state to be configured. Sort order is important! The first configuration added is executed as first order etc.
    /// The number of <see cref="ParameterSets"/> must equal the number of <see cref="OrderConfigurations"/>
    /// </summary>
    List<string> OrderConfigurations { get; }
}