// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.StateManagement;

/// <summary>
/// Interface for state machine implemenations
/// </summary>
public interface IStateMachineState
{
    /// <summary>
    /// State ID. See <see cref=""/>
    /// </summary>
    int Id { get; }

    /// <summary>
    /// Name of the state
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The states allowed to follow the current state
    /// </summary>
    List<string> AllowedNextStates { get; }

    /// <summary>
    /// Current context
    /// </summary>
    IStateManagementDevice CurrentContext { get; }

    /// <summary>
    /// Is the cancellation of all running orders required before the state is applied
    /// </summary>
    bool IsRunningOrdersCancellationRequired { get; set; }

    /// <summary>
    /// Is turning offstate request messages to the tower required
    /// </summary>
    bool IsTurningOffStateRequestsRequired { get; set; }


    /// <summary>
    /// Orders to be handled by the current state
    /// </summary>
    List<IOrder> Orders { get; }

    /// <summary>
    /// The next state to be requested when this state has to be left or null if the state does not change
    /// </summary>
    IStateMachineState NextState { get; }

    /// <summary>
    /// Set the inital states for this business state
    /// </summary>
    void SetInitalStates();


    /// <summary>
    /// Initiate this state
    /// </summary>
    void InitiateState();

    /// <summary>
    /// Run the next order for this state
    /// </summary>
    void RunNextOrder();

    /// <summary>
    /// Cancel this state
    /// </summary>
    void CancelState();

    /// <summary>
    /// Request the next state set by the current state
    /// </summary>
    void RequestNextState();


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
    /// The communication to the device was broken. Handle this event
    /// </summary>
    void HandleComDevClose();

    /// <summary>
    /// Handle a received error message from the device
    /// </summary>
    /// <param name="receivedMessage">Received error message from the device</param>
    void HandleErrorMessage(IInboundDataMessage receivedMessage);

    /// <summary>
    /// Handle async sent message from device
    /// </summary>
    /// <param name="message">Received message</param>
    /// <returns>The result of the message handling</returns>
    MessageHandlingResult HandleAsyncMessage(IInboundDataMessage message);

    /// <summary>
    /// Check a received state message from device if a state change is necessary
    /// </summary>
    /// <param name="message">Received state message from device</param>
    /// <param name="doNotNotifyClient">Do notify the clients</param>
    MessageHandlingResult CheckReceivedStateMessage(IInboundDataMessage message, bool doNotNotifyClient);

    /// <summary>
    /// Request a new device state
    /// </summary>
    /// <param name="newDeviceState">Requested new device state</param>
    void RequestNewDeviceState(IDeviceState newDeviceState);

    /// <summary>
    /// Prepare orders for the regular state reqeust
    /// </summary>
    /// <returns>List with orders</returns>
    IList<IOrder> PerpareRegularStateRequest();
}