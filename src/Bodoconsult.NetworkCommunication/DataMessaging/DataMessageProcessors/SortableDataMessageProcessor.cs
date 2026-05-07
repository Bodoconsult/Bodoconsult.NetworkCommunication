// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessageProcessors;

/// <summary>
/// Current implementation of <see cref="IDataMessageProcessor"/> for sortable data messages. Delivers messages in the order of reaching it via <see cref="ProcessMessage"/>
/// Should invoke IDataMessagingConfig.RaiseDataMessageReceivedDelegate for data messages and IDataMessagingConfig.DataMessageProcessingPackage.WaitStateManager?.OnHandshakeReceived for handshakes
/// </summary>
public class SortableDataMessageProcessor : BaseDataMessageProcessor
{
    private readonly IInboundDataMessageSorter _dataMessageSorter;

    /// <summary>
    /// Default ctor
    /// </summary>
    public SortableDataMessageProcessor(IDataMessagingConfig config) : base(config)
    {
        ArgumentNullException.ThrowIfNull(Config.DataMessageProcessingPackage?.DataMessageSorter);

        _dataMessageSorter = Config.DataMessageProcessingPackage.DataMessageSorter;
    }

    /// <summary>
    /// Process the message
    /// </summary>
    /// <param name="message">Message to process</param>
    public override void ProcessMessage(IInboundMessage message)
    {
        var s = $"received message {message.MessageId}: {message.RawMessageData.Length} bytes";
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
        s = $"message {message.MessageId} not valid: {message.GetType().Name}";
        Config.MonitorLogger.LogError(s);
    }

    private void ProcessSortableDataMessage(ISortableInboundDataMessage dataMessage)
    {
        string msg1;

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
                    msg1 = $"failed {dataMessage.MessageId}: {dataMessage.RawMessageData.Length} bytes: {e}";
                    Config.AppLogger.LogError($"{Config.LoggerId}{msg1}");
                    Config.MonitorLogger.LogError(msg1);
                }
            }).ContinueWith(Callback);

            var result = Stopped.WaitOne(TimeOut);
            if (result)
            {
                continue;
            }
            msg1 = $"{dataMessage}delivering to message receiver timed out";
            Config.AppLogger.LogError($"{Config.LoggerId}{msg1}");
            Config.MonitorLogger.LogError(msg1);
            return;
        }
    }
}