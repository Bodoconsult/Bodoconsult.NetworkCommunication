// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Delegates;

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
/// Interface for implementing a device with only order management but no state machine
/// </summary>
public interface IOnlyOrderManagementDevice: IOrderManagementDevice
{
    /// <summary>
    /// Handle an async received message without state machine
    /// </summary>
    NoStateMachineHandleAsyncMessageDelegate? NoStateMachineHandleAsyncMessageDelegate { get; set; }

    /// <summary>
    /// Handle an error message received from the device without state machine
    /// </summary>
    NoStateMachineHandleErrorMessageDelegate? NoStateMachineHandleErrorMessageDelegate { get; set; }

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
}