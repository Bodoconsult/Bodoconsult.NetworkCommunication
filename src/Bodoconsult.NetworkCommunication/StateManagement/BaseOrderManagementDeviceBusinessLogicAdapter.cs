// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;
using Bodoconsult.NetworkCommunication.StateManagement.Interfaces;

namespace Bodoconsult.NetworkCommunication.StateManagement;

/// <summary>
/// Base class for <see cref="IOrderManagementDeviceBusinessLogicAdapter"/> implementations
/// </summary>
public abstract class BaseOrderManagementDeviceBusinessLogicAdapter : IOrderManagementDeviceBusinessLogicAdapter
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="device">Current device</param>
    protected BaseOrderManagementDeviceBusinessLogicAdapter(IOrderManagementDevice device)
    {
        ArgumentNullException.ThrowIfNull(device.OrderManager);
        IpDevice = IpDevice = device;
        Device = device;
        OrderFactory = device.OrderManager.OrderFactory;
    }

    /// <summary>
    /// Current device
    /// </summary>
    public IIpDevice IpDevice { get; }

    /// <summary>
    /// Current device
    /// </summary>
    public IOrderManagementDevice Device { get; }

    /// <summary>
    /// Current order factory
    /// </summary>
    protected IOrderFactory? OrderFactory;

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
}