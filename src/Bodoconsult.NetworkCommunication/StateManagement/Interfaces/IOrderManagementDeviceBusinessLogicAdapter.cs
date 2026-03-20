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
    IOrderManagementDevice Device { get; }

    /// <summary>
    /// Default method to handle a ComDevClose event in business logic
    /// </summary>
    void DefaultHandleComDevClose();

    /// <summary>
    /// Default method to handle an error message received from the device in business logic
    /// </summary>
    /// <param name="message">Received message</param>
    void DefaultHandleErrorMessage(IInboundDataMessage message);

    /// <summary>
    /// Default method to handle an async received message
    /// </summary>
    /// <param name="message">Received message</param>
    MessageHandlingResult DefaultHandleAsyncMessage(IInboundDataMessage? message);
}