// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for states based on order management orders sent to the device
/// </summary>
public interface IOrderBasedActionStateMachineState : IStateMachineState
{
    /// <summary>
    /// Current order index. See <see cref="RunNextOrder"/>
    /// </summary>
    int CurrentOrderIndex { get; }

    /// <summary>
    /// Parametersets for the orders to be created for the state. The number of parameter sets must equal the number of <see cref="OrderConfigurations"/>
    /// </summary>
    List<IParameterSet> ParameterSets { get; }

    /// <summary>
    /// All configurations for orders to be executed for the state to be configured. Sort order is important! The first configuration added is executed as first order etc.
    /// </summary>
    List<string> OrderConfigurations { get; }

    /// <summary>
    /// Orders to be handled by the current state
    /// </summary>
    List<IOrder> Orders { get; }

    /// <summary>
    /// Run the next order for this state
    /// </summary>
    void RunNextOrder();

    /// <summary>
    /// Delegate fired when an order was finished successfully to implement buisness logic for that event. This delegate method should set a new state to request (but not request it)
    /// </summary>
    OrderFinishedSucessfullyDelegate? OrderFinishedSucessfullyDelegate { get; set; }

    /// <summary>
    /// Delegate fired when an order was finished unsuccessfully to implement buisness logic for that event. This delegate method should set a new state to request (but not request it)
    /// </summary>
    OrderFinishedUnsucessfullyDelegate? OrderFinishedUnsucessfullyDelegate { get; set; }

    /// <summary>
    /// The order has been finished successfully
    /// </summary>
    /// <param name="orderId">Current order ID</param>
    void OrderFinishedSucessfully(long orderId);

    /// <summary>
    /// The order has been finished successfully
    /// </summary>
    /// <param name="orderId">Current order ID</param>
    void OrderFinishedUnsucessfully(long orderId);

    /// <summary>
    /// Name of the next state for the default
    /// </summary>
    string? StateNameOnSuccess { get; set; }
}