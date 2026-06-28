// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessageProcessors;

/// <summary>
/// Base class for <see cref="IDataMessageProcessor"/> implementations
/// </summary>
public abstract class BaseDataMessageProcessor : IDataMessageProcessor
{
    private readonly ProducerConsumerQueue<IInboundMessage> _queue = new()
    {
        ThreadPriority = ThreadPriority.AboveNormal
    };

    /// <summary>
    /// Current queue count
    /// </summary>
    protected int CurrentQueueCount => _queue.InternalQueue.Count;

    /// <summary>
    /// Logger ID
    /// </summary>
    protected readonly string LoggerId;

    /// <summary>
    /// Timeout in ms for waiting for messages to be delivered to next step
    /// </summary>
    protected const int TimeOut = 5000;

    /// <summary>
    /// Default ctor
    /// </summary>
    protected BaseDataMessageProcessor(IDataMessagingConfig config)
    {
        Config = config;
        LoggerId = $"{config.LoggerId}{(config.LoggerId.EndsWith(": ") ? string.Empty : ": ")}";

        _queue.ConsumerTaskDelegate = ProcessMessage;
        _queue.StartConsumer();
    }

    /// <summary>
    /// Current <see cref="IDataMessagingConfig"/> instance
    /// </summary>
    public readonly IDataMessagingConfig Config;

    /// <summary>
    /// Add the message to the queue for processing
    /// </summary>
    /// <param name="message">Message to process</param>
    public void AddMessageToQueue(IInboundMessage message)
    {
        _queue.Enqueue(message);
    }

    /// <summary>
    /// Process the message
    /// </summary>
    /// <param name="message">Message to process</param>
    public virtual void ProcessMessage(IInboundMessage message)
    {
        throw new NotSupportedException("Override method in derived classes");
    }

    /// <summary>
    /// Process handshakes
    /// </summary>
    /// <param name="handShake">Handshake message</param>
    // ReSharper disable once SuggestBaseTypeForParameter
    protected void ProcessHandshakes(IInboundHandShakeMessage handShake)
    {
        ArgumentNullException.ThrowIfNull(Config.DataMessageProcessingPackage);

        Config.DataMessageProcessingPackage.WaitStateManager.OnHandshakeReceived(handShake);
    }
}