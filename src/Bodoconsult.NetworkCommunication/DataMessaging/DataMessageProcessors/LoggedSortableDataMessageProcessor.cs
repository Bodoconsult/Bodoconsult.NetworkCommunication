// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessageProcessors;

/// <summary>
/// Current implementation of <see cref="IDataMessageProcessor"/> for sortable data messages with data logging. Delivers messages in the order of reaching it via <see cref="ProcessMessage"/>
/// Should invoke IDataMessagingConfig.RaiseDataMessageReceivedDelegate for data messages and IDataMessagingConfig.DataMessageProcessingPackage.WaitStateManager?.OnHandshakeReceived for handshakes
/// </summary>
public class LoggedSortableDataMessageProcessor : BaseDataMessageProcessor
{
    private readonly IInboundDataMessageSorter _dataMessageSorter;
    private readonly List<IInboundDataLogger> _dataLoggers;

    /// <summary>
    /// Default ctor
    /// </summary>
    public LoggedSortableDataMessageProcessor(IDataMessagingConfig config) : base(config)
    {
        ArgumentNullException.ThrowIfNull(Config.DataMessageProcessingPackage);
        ArgumentNullException.ThrowIfNull(Config.DataMessageProcessingPackage.DataMessageSorter);
        ArgumentNullException.ThrowIfNull(Config.DataMessageProcessingPackage.DataLoggers);

        _dataMessageSorter = Config.DataMessageProcessingPackage.DataMessageSorter;
        _dataLoggers = Config.DataMessageProcessingPackage.DataLoggers;
    }

    /// <summary>
    /// Process the message
    /// </summary>
    /// <param name="message">Message to process</param>
    public override void ProcessMessage(IInboundMessage message)
    {
        var s = $"{LoggerId}received {message.ToShortInfoString()}";
        Config.MonitorLogger.LogInformation(s);

        // Data message received
        if (message is ISortableInboundDataMessage dataMessage)
        {
            ProcessSortableDataMessage(dataMessage);
            return;
        }

        // Handshake received
        if (message is IInboundHandShakeMessage handShake)
        {
            ProcessHandshakes(handShake);
            return;
        }

        // No valid message
        s = $"{message.ToShortInfoString()}: not valid: {message.RawMessageDataClearText}";
        Config.MonitorLogger.LogError(s);
    }

    private void ProcessSortableDataMessage(ISortableInboundDataMessage dataMessage)
    {
        // Sort messages
        var messages = _dataMessageSorter.AddMessage(dataMessage);

        if (messages.Count == 0)
        {
            return;
        }

        //Debug.Print($"LoggedSortableDataMessageProcessor: {messages.Count}");

        // Logging activated?
        if (Config.IsDataLoggingActivated)
        {
            // Now process the messages
            foreach (var msg in messages)
            {
                // Log messages
                LogMessage(msg);

                // Now forward the message to the message receiver
                ForwardToMessageReceiver(msg);
            }

            return;
        }

        // Now process the messages
        foreach (var msg in messages)
        {
            // Now forward the message to the message receiver
            ForwardToMessageReceiver(msg);
        }
    }

    private void ForwardToMessageReceiver(ISortableInboundDataMessage msg)
    {
        Config.RaiseCommLayerDataMessageReceivedDelegate?.Invoke(msg);
    }

    private void LogMessage(ISortableInboundDataMessage msg)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        foreach (var logger in _dataLoggers)
        {
            var chunks = logger.CheckIfMessageIsToLog(msg);

            if (chunks.Count == 0)
            {
                //Debug.Print($"Logged: OID {msg.OriginalMessageId}: 0 chunks");
                continue;
            }

            logger.LogTheMessages(chunks);
            //Debug.Print($"Logged: OID {msg.OriginalMessageId}: {chunks.Count} chunks with {chunks[0].Length}B");
        }
    }
}