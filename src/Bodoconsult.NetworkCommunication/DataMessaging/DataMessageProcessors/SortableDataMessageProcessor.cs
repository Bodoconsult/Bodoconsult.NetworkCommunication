// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;
using System.Diagnostics;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessageProcessors;

/// <summary>
/// Current implementation of <see cref="IDataMessageProcessor"/> for sortable data messages. Delivers messages in the order of reaching it via <see cref="ProcessMessage"/>
/// Should invoke IDataMessagingConfig.RaiseDataMessageReceivedDelegate for data messages and IDataMessagingConfig.DataMessageProcessingPackage.WaitStateManager?.OnHandshakeReceived for handshakes
/// </summary>
public class SortableDataMessageProcessor : BaseDataMessageProcessor
{
    private readonly IInboundDataMessageSorter _dataMessageSorter;
    private readonly string _loggerId;

    /// <summary>
    /// Default ctor
    /// </summary>
    public SortableDataMessageProcessor(IDataMessagingConfig config) : base(config)
    {
        ArgumentNullException.ThrowIfNull(Config.DataMessageProcessingPackage?.DataMessageSorter);

        _dataMessageSorter = Config.DataMessageProcessingPackage.DataMessageSorter;
        _loggerId = $"{config.LoggerId}: SortableDataMessageProcessor: ";
    }

    /// <summary>
    /// Process the message
    /// </summary>
    /// <param name="message">Message to process</param>
    public override void ProcessMessage(IInboundMessage message)
    {
        Trace.TraceInformation($"{_loggerId}received message {message.MessageId}: {message.RawMessageData.Length} bytes");

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
        Trace.TraceInformation($"{_loggerId}{s}");
    }

    private void ProcessSortableDataMessage(ISortableInboundDataMessage dataMessage)
    {
        // Sort messages
        var messages = _dataMessageSorter.AddMessage(dataMessage);

        if (messages.Count == 0)
        {
            return;
        }

        // Now process the message
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
}