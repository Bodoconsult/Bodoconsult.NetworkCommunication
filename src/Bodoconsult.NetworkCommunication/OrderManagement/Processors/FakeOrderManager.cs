// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.OrderManagement.Processors;

/// <summary>
/// Fake implementation of <see cref="IOrderManager"/>
/// </summary>
public class FakeOrderManager : IOrderManager
{
    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="messagingConfig">Current device communication adapter</param>
    /// <param name="deviceOrderProcessor">Current device order processor</param>
    /// <param name="deviceOrderReceiver">Current device order receiver</param>
    /// <param name="orderFactory">Current order factory</param>
    public FakeOrderManager(IDataMessagingConfig messagingConfig,
        IOrderProcessor deviceOrderProcessor,
        IOrderReceiver deviceOrderReceiver,
        IOrderFactory orderFactory)
    {
        MessagingConfig = messagingConfig;
        OrderProcessor = deviceOrderProcessor;
        OrderReceiver = deviceOrderReceiver;
        OrderFactory = orderFactory;
    }

    /// <summary>
    /// Current messaging config
    /// </summary>
    public IDataMessagingConfig MessagingConfig { get; set; }

    /// <summary>
    /// Current order factory
    /// </summary>
    public IOrderFactory OrderFactory { get; set;  }

    /// <summary>
    /// Current order processor instance
    /// </summary>
    public IOrderProcessor OrderProcessor { get; set; }

    /// <summary>
    /// Current order processor instance
    /// </summary>
    public IOrderReceiver OrderReceiver { get; set; }

    /// <summary>
    /// Adds a received device message to the receiver queue for further processing
    /// </summary>
    /// <param name="order">Order to add to the message queue</param>
    public void AddOrder(IOrder order)
    {
        // Do nothing
    }

    /// <summary>
    /// Event handling method for binding to NotifyDeviceMessageReceived event
    /// </summary>
    /// <param name="dataMessage">Received message</param>
    public void OnDeviceMessageReceived(IInboundDataMessage dataMessage)
    {
        // Do nothing
    }

    /// <summary>
    /// Starts the watchdog for the order processing
    /// </summary>
    public void StartOrderProcessing()
    {
        // Do nothing
    }

    /// <summary>
    /// Stops the watchdog for the order processing
    /// </summary>
    public void StopOrderProcessing()
    {
        // Do nothing
    }
}