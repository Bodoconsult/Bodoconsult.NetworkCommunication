// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;
using System.Diagnostics;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessageProcessors;

/// <summary>
/// Current implementation of <see cref="IDataMessageProcessor"/> for sortable data messages with data logging. Delivers messages in the order of reaching it via <see cref="ProcessMessage"/>
/// Should invoke IDataMessagingConfig.RaiseDataMessageReceivedDelegate for data messages and IDataMessagingConfig.DataMessageProcessingPackage.WaitStateManager?.OnHandshakeReceived for handshakes
/// </summary>
public class LoggedSortableDataMessageProcessor : BaseDataMessageProcessor
{
    private readonly IInboundDataMessageSorter _dataMessageSorter;
    private readonly List<IInboundDataLogger> _dataLoggers;
    private readonly string _loggerId;

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
        _loggerId = $"{config.LoggerId}: LoggedSortableDataMessageProcessor: ";
    }

    /// <summary>
    /// Process the message
    /// </summary>
    /// <param name="message">Message to process</param>
    public override void ProcessMessage(IInboundMessage message)
    {
        Trace.TraceInformation($"LoggedSortableDataMessageProcessor: received message {message.MessageId}: {message.RawMessageData.Length} bytes");

        // Handshake received
        if (message is IInboundHandShakeMessage handShake)
        {
            ProcessHandshakes(handShake);
            return;
        }

        // Data message received
        if (message is ISortableInboundDataMessage dataMessage)
        {
            ProcessSortableDataMessage(dataMessage);
            return;
        }

        // No valid message
        var s = $"message {message.MessageId} not valid: {message.GetType().Name}";
        Config.MonitorLogger.LogError(s);
        Trace.TraceInformation($"LoggedSortableDataMessageProcessor: {s}");
    }

    private void ProcessSortableDataMessage(ISortableInboundDataMessage dataMessage)
    {
        // Sort messages
        var messages = _dataMessageSorter.AddMessage(dataMessage);

        if (messages.Count == 0)
        {
            return;
        }

        // Log messages
        foreach (var msg in messages)
        {
            LogMessage(msg);
        }

        // Now process the messages
        foreach (var msg in messages)
        {
            AsyncHelper.FireAndForget2(() =>
            {
                try
                {
                    Config.RaiseCommLayerDataMessageReceivedDelegate?.Invoke(msg);
                }
                catch (Exception e)
                {
                    var s = $" failed {dataMessage.MessageId}: {dataMessage.RawMessageData.Length} bytes: {e}";
                    Config.MonitorLogger.LogError(s);
                    Trace.TraceError($"{_loggerId}{s}");
                }

            }).ContinueWith(Callback);

            var result = _stopped.WaitOne(TimeOut);
            if (result)
            {
                continue;
            }
            var msg1 = $"{dataMessage}delivering to message receiver timed out";
            Config.AppLogger.LogError($"{Config.LoggerId}{msg1}");
            Config.MonitorLogger.LogError(msg1);
            Trace.TraceError($"{_loggerId}{msg1}");
            return;
        }
    }

    private void LogMessage(ISortableInboundDataMessage msg)
    {
        foreach (var logger in _dataLoggers)
        {
            if (!logger.CheckIfMessageIsToLog(msg))
            {
                continue;
            }

            logger.LogTheMessage(msg);
            break;
        }
    }
}