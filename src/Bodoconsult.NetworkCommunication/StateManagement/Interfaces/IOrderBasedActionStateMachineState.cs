// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

/// <summary>
/// Interface for states based on order management orders sent to the device
/// </summary>
public interface IOrderBasedActionStateMachineState : IStateMachineState
{
    /// <summary>
    /// Delegate to create one or more orders sent to device needed for an order based state machine state
    /// </summary>
    PrepareOrdersForStateMachineStateDelegate PrepareOrdersForStateMachineStateDelegate { get; set; }

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
    OrderFinishedSucessfullyDelegate OrderFinishedSucessfullyDelegate { get; set; }

    /// <summary>
    /// Delegate fired when an order was finished unsuccessfully to implement buisness logic for that event. This delegate method should set a new state to request (but not request it)
    /// </summary>
    OrderFinishedUnsucessfullyDelegate OrderFinishedUnsucessfullyDelegate { get; set; }

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
}