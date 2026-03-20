// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

/// <summary>
/// Interface for connecting device order management with a device specific business logic
/// </summary>
public interface IOrderManagementDeviceBusinessLogicAdapter : IDeviceBusinessLogicAdapter
{
    /// <summary>
    /// Current device
    /// </summary>
    IOrderManagementDevice? Device { get; }

}


/// <summary>
    /// Interface for connecting device state management with a device specific business logic
    /// </summary>
    public interface IStateMachineDeviceBusinessLogicAdapter : IDeviceBusinessLogicAdapter
{
    /// <summary>
    /// Current device
    /// </summary>
    IStateManagementDevice? Device { get; }

    /// <summary>
    /// Current state factory
    /// </summary>
    IStateMachineStateFactory? StateFactory { get; }

    /// <summary>
    /// Load the state factory
    /// </summary>
    /// <param name="stateFactory">Current state factory</param>
    void LoadStateFactory(IStateMachineStateFactory stateFactory);

    /// <summary>
    /// Default method to handle a ComDevClose event in business logic
    /// </summary>
    /// <param name="state">Current state</param>
    void DefaultHandleComDevCloseDelegate(IStateMachineState state);

    /// <summary>
    /// Default method to handle an error message received from the device in business logic
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="message">Received message</param>
    void DefaultHandleErrorMessageDelegate(IStateMachineState state, IInboundDataMessage message);

    /// <summary>
    /// Default method to handle an async received message
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="message">Received message</param>
    MessageHandlingResult DefaultHandleAsyncMessageDelegate(IStateMachineState state, IInboundDataMessage? message);
}