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
        }

        // No valid message
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
            AsyncHelper.FireAndForget2(() => Config.RaiseCommLayerDataMessageReceivedDelegate?.Invoke(msg)).ContinueWith(Callback);
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