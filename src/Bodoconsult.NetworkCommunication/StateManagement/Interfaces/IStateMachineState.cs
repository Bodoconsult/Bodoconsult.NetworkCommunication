// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

/// <summary>
/// Interface for state machine implemenations
/// </summary>
public interface IStateMachineState
{
    /// <summary>
    /// State ID
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
    /// Initial device state
    /// </summary>
    DeviceState InitialDeviceState { get; set; }

    /// <summary>
    /// Initial business substate
    /// </summary>
    BusinessSubState InitialBusinessSubState { get; set; }

    /// <summary>
    /// Is the cancellation of all running order management orders required before the state is applied
    /// </summary>
    bool IsRunningOrdersCancellationRequired { get; set; }

    /// <summary>
    /// Is turning off state request messages to the tower required
    /// </summary>
    bool IsTurningOffStateRequestsRequired { get; set; }

    /// <summary>
    /// <see cref="CancellationTokenSource"/> instance to use for the current state or null if none is used
    /// </summary>
    CancellationTokenSource CancellationTokenSource { get; set; }

    /// <summary>
    /// Delegate to cancel the state
    /// </summary>
    CancelStateDelegate CancelStateDelegate { get; set; }

   /// <summary>
   /// The next state to be requested when this state has to be left or null if the state does not change
   /// </summary>
   IStateMachineState NextState { get; set; }

    /// <summary>
    /// Set the inital states for this business state
    /// </summary>
    void SetInitalStates();

    /// <summary>
    /// Initiate this state
    /// </summary>
    void InitiateState();

    /// <summary>
    /// Cancel this state
    /// </summary>
    void CancelState();

    /// <summary>
    /// Request the next state set by the current state
    /// </summary>
    void RequestNextState();

    /// <summary>
    /// The name of the state to be called if a ComDevClose event has happend
    /// </summary>
    public string StateToRequestOnComDevClose { get; set; }

    /// <summary>
    /// Delegate to handle a ComDevClose event in business logic
    /// </summary>
    HandleComDevCloseDelegate HandleComDevCloseDelegate { get; set; }

    /// <summary>
    /// The communication to the device was broken. Handles this event by calling <see cref="HandleComDevCloseDelegate"/>
    /// </summary>
    void HandleComDevClose();

    /// <summary>
    /// The name of the state to be called if an error message was sent by the device
    /// </summary>
    public string StateToRequestOnError { get; set; }

    /// <summary>
    /// Handle an error message received from the device
    /// </summary>
    HandleErrorMessageDelegate HandleErrorMessageDelegate { get; set; }

    /// <summary>
    /// Handle a received error message from the device
    /// </summary>
    /// <param name="receivedMessage">Received error message from the device</param>
    void HandleErrorMessage(IInboundDataMessage receivedMessage);

    /// <summary>
    /// Handle an async received message
    /// </summary>
    HandleAsyncMessageDelegate HandleAsyncMessageDelegate { get; set; }

    /// <summary>
    /// Handle async sent message from device by calling <see cref="HandleAsyncMessageDelegate"/>
    /// </summary>
    /// <param name="message">Received message</param>
    /// <returns>The result of the message handling</returns>
    MessageHandlingResult HandleAsyncMessage(IInboundDataMessage message);

    /// <summary>
    /// Request a new device state
    /// </summary>
    /// <param name="newDeviceState">Requested new device state</param>
    void RequestNewDeviceState(IDeviceState newDeviceState);

    #region State check handling

    /// <summary>
    /// Delegate for preparing orders for the regular state reqeust
    /// </summary>
    PrepareRegularStateRequestDelegate PrepareRegularStateRequestDelegate { get; set; }

    /// <summary>
    /// Delegate for handling device state check request answers in business logic
    /// </summary>
    HandleRegularStateRequestAnswerDelegate HandleRegularStateRequestAnswerDelegate { get; set; }

    /// <summary>
    /// Check a received state message from device and handle it
    /// </summary>
    /// <param name="message">Received state message from device</param>
    /// <param name="doNotNotifyClient">Do notify the clients</param>
    MessageHandlingResult HandleRegularStateRequestAnswer(IInboundDataMessage message, bool doNotNotifyClient);

    /// <summary>
    /// Prepare orders for the regular state reqeust
    /// </summary>
    /// <returns>List with orders</returns>
    List<IOrder> PerpareRegularStateRequest();

    #endregion
}