// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.


// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement;

namespace Bodoconsult.NetworkCommunication.BusinessLogicAdapters;

/// <summary>
/// Base class for <see cref="ISimpleDeviceBusinessLogicAdapter"/> implementations
/// </summary>
public abstract class BaseSimpleDeviceBusinessLogicAdapter : ISimpleDeviceBusinessLogicAdapter
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="device">Current device</param>
    protected BaseSimpleDeviceBusinessLogicAdapter(IIpDevice device)
    {
        IpDevice = device;
    }

    /// <summary>
    /// Current device
    /// </summary>
    public IIpDevice IpDevice { get; }

    /// <summary>
    /// Default method to handle a ComDevClose event in business logic
    /// </summary>
    public virtual void DefaultHandleComDevClose()
    {
        // Do nothing
    }

    /// <summary>
    /// Default method to handle an error message received from the device in business logic
    /// </summary>
    /// <param name="message">Received message</param>
    public virtual void DefaultHandleErrorMessage(IInboundDataMessage message)
    {
        // Do nothing
    }

    /// <summary>
    /// Default method to handle an async received message
    /// </summary>
    /// <param name="message">Received message</param>
    public virtual MessageHandlingResult DefaultHandleAsyncMessage(IInboundDataMessage? message)
    {
        // Do nothing
        return MessageHandlingResultHelper.Success();
    }

    /// <summary>
    /// Default method to handle a received message from the device in business logic
    /// </summary>
    /// <param name="message">Received message</param>
    public virtual void DefaultReceiveMessage(IInboundDataMessage message)
    {
        throw new NotSupportedException("Override in derived classes");
    }

    /// <summary>
    /// Send an outbound datamessage (overrideable)
    /// </summary>
    /// <param name="message">Outbound datamessage</param>
    /// <returns>Message sending result</returns>
    public virtual MessageSendingResult SendMessage(IOutboundDataMessage message)
    {
        ArgumentNullException.ThrowIfNull(IpDevice.CommunicationAdapter);
        return IpDevice.CommunicationAdapter.SendDataMessage(message);
    }
}