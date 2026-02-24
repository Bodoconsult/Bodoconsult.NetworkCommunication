// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.OrderManagement.Processors;

/// <summary>
/// Current implementation of <see cref="IOrderManager"/>
/// </summary>
public class OrderManager: IOrderManager
{
    private readonly IAppLoggerProxy _appLogger;
    private readonly string _loggerId;

    /// <summary>
    /// Default ctor
    /// </summary>
    /// <param name="messagingConfig">Current device communication adapter</param>
    /// <param name="deviceOrderProcessor">Current device order processor</param>
    /// <param name="deviceOrderReceiver">Current device order receiver</param>
    /// <param name="appLogger">Current app logger</param>
    public OrderManager(IDataMessagingConfig messagingConfig,
        IOrderProcessor deviceOrderProcessor,
        IOrderReceiver deviceOrderReceiver,
        IAppLoggerProxy appLogger)
    {
        MessagingConfig = messagingConfig;
        _loggerId = MessagingConfig.LoggerId;
        OrderProcessor = deviceOrderProcessor;
        OrderReceiver = deviceOrderReceiver;
        _appLogger = appLogger;

        // Connect order processor and receiver
        OrderReceiver.OrderReceiverCheckMessageDelegate = receivedMessage => OrderProcessor.CheckReceivedMessage(receivedMessage);

        // Bind message receiving 
        //CommunicationAdapter.NotifydeviceMessageReceived += OndeviceMessageReceived;
        if (MessagingConfig == null)
        {
            throw new NullReferenceException("Data messaging config may not be null");
        }

        MessagingConfig.RaiseAppLayerDataMessageReceivedDelegate = OndeviceMessageReceived;
        _appLogger.LogDebug($"{_loggerId}Order processing bound to receiver");
    }

    /// <summary>
    /// Current device communication adapter instance
    /// </summary>
    public IDataMessagingConfig MessagingConfig { get; }

    /// <summary>
    /// Current device order processor instance
    /// </summary>
    public IOrderProcessor OrderProcessor { get; }

    /// <summary>
    /// Current device order processor instance
    /// </summary>
    public IOrderReceiver OrderReceiver { get; }

    /// <summary>
    /// Adds a received device message to the receiver queue for further processing
    /// </summary>
    /// <param name="order">Order to add to the message queue</param>
    public void AddOrder(IOrder order)
    {
        _appLogger.LogDebug($"{_loggerId}order received: {order.LoggerId}");
        OrderProcessor.AddOrder(order);
    }

    /// <summary>
    /// Event handling method for binding to <see cref="IOrderManager.MessagingConfig"/>.NotifydeviceMessageReceivedDelegate
    /// </summary>
    /// <param name="dataMessage">Received message</param>
    public void OndeviceMessageReceived(IInboundDataMessage dataMessage)
    {
        Debug.Print($"{_loggerId}message received: {dataMessage.ToInfoString()}");
        _appLogger.LogDebug($"{_loggerId}message received: {dataMessage.ToInfoString()}");
        OrderReceiver.AddReceivedMessage(dataMessage);
    }

    /// <summary>
    /// Starts the watchdog for the order processing
    /// </summary>
    public void StartOrderProcessing()
    {
        OrderProcessor.StartOrderProcessing();
        _appLogger.LogDebug($"{_loggerId}order processing has been started");
    }

    /// <summary>
    /// Stops the watchdog for the order processing
    /// </summary>
    public void StopOrderProcessing()
    {
        OrderProcessor.StopOrderProcessing();
        _appLogger.LogDebug($"{_loggerId}order processing was stopped");
    }
}