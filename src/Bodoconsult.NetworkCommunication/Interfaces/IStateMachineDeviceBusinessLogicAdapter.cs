// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

namespace Bodoconsult.NetworkCommunication.Interfaces;

/// <summary>
    /// Interface for connecting device state management with a device specific business logic
    /// </summary>
    public interface IStateMachineDeviceBusinessLogicAdapter : IDeviceBusinessLogicAdapter
{
    /// <summary>
    /// Current device
    /// </summary>
    IStateMachineDevice? Device { get; }

    /// <summary>
    /// Current state factory
    /// </summary>
    IStateMachineStateFactory? StateFactory { get; }

    /// <summary>
    /// Load the order factory
    /// </summary>
    void LoadOrderFactory();

    /// <summary>
    /// Load the state factory
    /// </summary>
    /// <param name="stateFactory">Current state factory</param>
    void LoadStateFactory(IStateMachineStateFactory stateFactory);

    /// <summary>
    /// Default method to handle a ComDevClose event in business logic
    /// </summary>
    /// <param name="state">Current state</param>
    void DefaultHandleComDevClose(IStateMachineState state);

    /// <summary>
    /// Default method to handle an error message received from the device in business logic
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="message">Received message</param>
    void DefaultHandleErrorMessage(IStateMachineState state, IInboundDataMessage message);

    /// <summary>
    /// Default method to handle an async received message
    /// </summary>
    /// <param name="state">Current state</param>
    /// <param name="message">Received message</param>
    MessageHandlingResult DefaultHandleAsyncMessage(IStateMachineState state, IInboundDataMessage? message);
}