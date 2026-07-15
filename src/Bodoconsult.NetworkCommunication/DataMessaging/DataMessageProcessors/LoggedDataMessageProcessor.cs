// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessageProcessors;

/// <summary>
/// Current implementation of <see cref="IDataMessageProcessor"/> for sortable data messages with data logging. Delivers messages in the order of reaching it via <see cref="ProcessMessage"/>
/// Should invoke IDataMessagingConfig.RaiseDataMessageReceivedDelegate for data messages and IDataMessagingConfig.DataMessageProcessingPackage.WaitStateManager?.OnHandshakeReceived for handshakes
/// </summary>
public class LoggedDataMessageProcessor : BaseDataMessageProcessor
{
    private readonly List<IInboundDataLogger> _dataLoggers;

    /// <summary>
    /// Default ctor
    /// </summary>
    public LoggedDataMessageProcessor(IDataMessagingConfig config) : base(config)
    {
        ArgumentNullException.ThrowIfNull(Config.DataMessageProcessingPackage);

        _dataLoggers = Config.DataMessageProcessingPackage.DataLoggers;
    }

    /// <summary>
    /// Process the message
    /// </summary>
    /// <param name="message">Message to process</param>
    public override void ProcessMessage(IInboundMessage message)
    {
#if DEBUG
        var s = $"{LoggerId}received {message.ToShortInfoString()}";
        Config.MonitorLogger.LogInformation(s);
#endif

        // Handshake received
        if (message is IInboundHandShakeMessage handShake)
        {
            ProcessHandshakes(handShake);
            return;
        }

        // Data message received
        if (message is IInboundDataMessage dataMessage)
        {
            ProcessDataMessage(dataMessage);
            return;
        }

        // No valid message
        s = $"{message.ToShortInfoString()}: not valid: {message.GetType().Name}: {message.RawMessageDataClearText}";
        Config.MonitorLogger.LogError(s);
    }

    private void ProcessDataMessage(IInboundDataMessage dataMessage)
    {
        // Sort messages

        // Log messages
        if (Config.IsDataLoggingActivated)
        {
            LogMessage(dataMessage);
        }

        // Now process the messages
        Config.RaiseCommLayerDataMessageReceivedDelegate?.Invoke(dataMessage);
    }

    private void LogMessage(IInboundDataMessage msg)
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