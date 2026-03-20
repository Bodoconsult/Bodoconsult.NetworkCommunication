// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

/// <summary>
/// Interface for connecting a simple device comm layer without order and state management with a device specific business logic
/// </summary>
public interface ISimpleDeviceBusinessLogicAdapter : IDeviceBusinessLogicAdapter
{
    /// <summary>
    /// Default method to handle a ComDevClose event in business logic
    /// </summary>
    void DefaultHandleComDevCloseDelegate();

    /// <summary>
    /// Default method to handle an error message received from the device in business logic
    /// </summary>
    /// <param name="message">Received message</param>
    void DefaultHandleErrorMessageDelegate(IInboundDataMessage message);

    /// <summary>
    /// Default method to handle an async received message
    /// </summary>
    /// <param name="message">Received message</param>
    MessageHandlingResult DefaultHandleAsyncMessageDelegate(IInboundDataMessage? message);
}