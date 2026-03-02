// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.StateManagement;

/// <summary>
/// State managament state
/// </summary>
public interface IStateManagementState
{
    /// <summary>
    /// Current device state
    /// </summary>
    IDeviceState DeviceState { get; }

    /// <summary>
    /// Current business state
    /// </summary>
    IBusinessState BusinessState { get; }

    /// <summary>
    /// Current business substate
    /// </summary>
    IBusinessSubState BusinessSubState { get; }

    /// <summary>
    /// Current context
    /// </summary>
    IStateManagementContext CurrentContext { get; }

    /// <summary>
    /// Type name of the order
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Is cancellation of running orders required on error?
    /// </summary>
    bool IsRunningOrdersCancellationRequired { get; set; }

    /// <summary>
    /// Allowed next states
    /// </summary>
    List<string> AllowedNextStates { get; }

    /// <summary>
    /// Read only list all orders required by this state
    /// </summary>
    List<IOrder> Orders { get; }

    /// <summary>
    /// The next state to run after all orders for this state were running
    /// </summary>
    IStateManagementState NextState { get; set; }

    /// <summary>
    /// Initiate the state i.e. with creating required orders
    /// </summary>
    void InitiateState();

    /// <summary>
    /// Ser the initial states for this state
    /// </summary>
    void SetInitalStates();

    /// <summary>
    /// Run the next order for the state
    /// </summary>
    void RunNextOrder();

    /// <summary>
    /// The order was finished successfully and now do the last work on the order
    /// </summary>
    /// <param name="order">Current order</param>
    public void OrderFinishedSuccessful(IOrder order);

    /// <summary>
    /// The order was NOT finished succesfully and now do the last work on the order
    /// </summary>
    /// <param name="order">Current order</param>
    void OrderFinishedUnsuccessful(IOrder order);

    /// <summary>
    /// Handle a ComDevClose event
    /// </summary>
    void HandleComDevClose();

    /// <summary>
    /// Handle an error message sent by the device
    /// </summary>
    /// <param name="message">Error message</param>
    void HandleErrorMessage(IInboundDataMessage message);

    /// <summary>
    /// Handle an async received message sent by the device
    /// </summary>
    /// <param name="message">Async received message</param>
    MessageHandlingResult HandleAsyncMessage(IInboundDataMessage message);

    /// <summary>
    /// Create a string for logging
    /// </summary>
    /// <returns>String with state info</returns>
    string ToLogString();
}