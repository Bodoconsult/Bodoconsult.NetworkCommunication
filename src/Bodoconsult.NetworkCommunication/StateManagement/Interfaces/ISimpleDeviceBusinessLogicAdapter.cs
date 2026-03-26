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
    public void DefaultHandleComDevClose();

    /// <summary>
    /// Default method to handle an error message received from the device in business logic
    /// </summary>
    /// <param name="message">Received message</param>
    public void DefaultHandleErrorMessage(IInboundDataMessage message);

    /// <summary>
    /// Default method to handle an async received message
    /// </summary>
    /// <param name="message">Received message</param>
    public MessageHandlingResult DefaultHandleAsyncMessage(IInboundDataMessage? message);

    /// <summary>
    /// Default method to handle a received message from the device in business logic
    /// </summary>
    /// <param name="message">Received message</param>
    public void DefaultReceiveMessage(IInboundDataMessage message);
}