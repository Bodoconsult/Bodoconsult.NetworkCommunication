// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Helpers;
using Bodoconsult.App.Interfaces;
using Bodoconsult.NetworkCommunication.Delegates;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.OrderManagement.Processors;

/// <summary>
/// Current implementation of <see cref="IOrderReceiver"/>
/// </summary>
public class OrderReceiver : IOrderReceiver
{
    private readonly ProducerConsumerQueue<IInboundDataMessage> _messageQueue = new();
    private readonly IAppLoggerProxy _appLogger;

    /// <summary>
    /// Default ctor
    /// </summary>
    public OrderReceiver(IAppLoggerProxy appLogger)
    {
        _appLogger = appLogger;

        ConsumerTaskDelegate<IInboundDataMessage> consumerTaskDelegate = RunMessage;

        _messageQueue.ConsumerTaskDelegate = consumerTaskDelegate;
        _messageQueue.StartConsumer();
    }

    /// <summary>
    /// Run a message delivered from the queue
    /// </summary>
    /// <param name="message">device message received</param>
    public void RunMessage(IInboundDataMessage message)
    {
        if (!IsReceivedMessageProcessingActivated)
        {
            return;
        }

        var result = OrderReceiverCheckMessageDelegate.Invoke(message);
        _appLogger?.LogInformation($"Received message was processed {(result ? "successfully" : "unsucessfully")}: {message.ToShortInfoString()}");
    }


    /// <summary>
    /// Delegate for handling a received device message
    /// </summary>
    public OrderReceiverCheckMessageDelegate OrderReceiverCheckMessageDelegate { get; set; }


    /// <summary>
    /// Is the received message processing activated?
    /// </summary>
    public bool IsReceivedMessageProcessingActivated {get; set; }

    /// <summary>
    /// Adds a received  message to the receiver queue for further processing
    /// </summary>
    /// <param name="receivedMessage">Received message</param>
    public void AddReceivedMessage(IInboundDataMessage receivedMessage)
    {
        _messageQueue.Enqueue(receivedMessage);
        _appLogger?.LogInformation($"Received message added to queue: {receivedMessage.ToShortInfoString()}");
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        _messageQueue.Dispose();
    }
}