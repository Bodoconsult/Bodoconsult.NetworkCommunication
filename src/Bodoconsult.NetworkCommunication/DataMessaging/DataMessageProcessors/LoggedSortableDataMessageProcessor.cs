// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Helpers;
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

        Stopped.Reset();

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
        s = $"{message.ToShortInfoString()} no valid message type";
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
        string msg1;

        AsyncHelper.FireAndForget2(() =>
        {
            try
            {
                Config.RaiseCommLayerDataMessageReceivedDelegate?.Invoke(msg);
            }
            catch (Exception e)
            {
                msg1 = $" failed {msg.ToShortInfoString()}: {e}";
                Config.MonitorLogger.LogError(msg1);
                Config.AppLogger.LogError($"{LoggerId}{msg1}");
            }

        }).ContinueWith(Callback);

        var result = Stopped.WaitOne(TimeOut);
        if (result)
        {
            return;
        }

        msg1 = $"{msg.ToShortInfoString()}delivering to message receiver timed out";
        Config.AppLogger.LogError($"{Config.LoggerId}{msg1}");
        Config.MonitorLogger.LogError(msg1);
    }

    private void LogMessage(ISortableInboundDataMessage msg)
    {
        foreach (var logger in _dataLoggers)
        {
            var chunks = logger.CheckIfMessageIsToLog(msg);

            if (chunks.Count == 0)
            {
                continue;
            }

            logger.LogTheMessages(chunks);
        }
    }
}