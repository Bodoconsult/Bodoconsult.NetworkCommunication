// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using System.Diagnostics;
using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessageProcessors;

/// <summary>
/// Current implementation of <see cref="IDataMessageProcessor"/> for plain data messages. Delivers messages in the order of reaching it via <see cref="ProcessMessage"/>
/// Should invoke IDataMessagingConfig.RaiseDataMessageReceivedDelegate for data messages and IDataMessagingConfig.DataMessageProcessingPackage.WaitStateManager?.OnHandshakeReceived for handshakes
/// </summary>
public class DefaultDataMessageProcessor : BaseDataMessageProcessor
{
    /// <summary>
    /// Default ctor
    /// </summary>
    public DefaultDataMessageProcessor(IDataMessagingConfig config): base(config)
    { }

    /// <summary>
    /// Process the message
    /// </summary>
    /// <param name="message">Message to process</param>
    public override void ProcessMessage(IInboundMessage message)
    {
        Trace.TraceInformation($"DefaultDataMessageProcessor: received message {message.MessageId}: {message.RawMessageData.Length} bytes");

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
        var s = $"message {message.MessageId} not valid: {message.GetType().Name}";
        Config.MonitorLogger.LogError(s);
        Trace.TraceInformation($"DefaultDataMessageProcessor: {s}");
    }

    private void ProcessDataMessage(IInboundDataMessage dataMessage)
    {
        ArgumentNullException.ThrowIfNull(Config.RaiseCommLayerDataMessageReceivedDelegate);

        //Config.RaiseCommLayerDataMessageReceivedDelegate.Invoke(dataMessage);
        //return;

        // Now process the message
        AsyncHelper.FireAndForget2(() =>
        {
            try
            {
                Config.RaiseCommLayerDataMessageReceivedDelegate.Invoke(dataMessage);
            }
            catch (Exception e)
            {
                var s = $" failed {dataMessage.MessageId}: {dataMessage.RawMessageData.Length} bytes: {e}";
                Config.MonitorLogger.LogError(s);
                Trace.TraceInformation($"DefaultDataMessageProcessor: {s}");
            }
        }).ContinueWith(Callback);
    }
}