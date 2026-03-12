// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessageProcessors;

/// <summary>
/// Current implementation of <see cref="IDataMessageProcessor"/>. Delivers messages in the order of reaching it via <see cref="ProcessMessage"/>
/// Should invoke IDataMessagingConfig.RaiseDataMessageReceivedDelegate for data messages and IDataMessagingConfig.DataMessageProcessingPackage.WaitStateManager?.OnHandshakeReceived for handshakes
/// </summary>
public class DefaultDataMessageProcessor : IDataMessageProcessor
{

    private readonly AutoResetEvent _stopped = new(false);

    private const int TimeOut = 2000;

    /// <summary>
    /// Current <see cref="IDataMessagingConfig"/> instance
    /// </summary>
    public readonly IDataMessagingConfig Config;

    /// <summary>
    /// Default ctor
    /// </summary>
    public DefaultDataMessageProcessor(IDataMessagingConfig config)
    {
        Config = config;
    }

    /// <summary>
    /// Process the message
    /// </summary>
    /// <param name="message">Message to process</param>
    public void ProcessMessage(IInboundMessage message)
    {
        // Handshake received
        if (message is IInboundHandShakeMessage handShake)
        {
            ProcessHandshakes(handShake);
            return;
        }

        // Data message received
        if (message is IInboundDataMessage dataMessage)
        {
            AsyncHelper.FireAndForget2(() => Config.RaiseCommLayerDataMessageReceivedDelegate?.Invoke(dataMessage)).ContinueWith(Callback);
        }

        // No valid message
    }

    // ReSharper disable once SuggestBaseTypeForParameter
    private void ProcessHandshakes(IInboundHandShakeMessage handShake)
    {
        ArgumentNullException.ThrowIfNull(Config.DataMessageProcessingPackage);

        // fire and forget but let CallBack() be run at the end
        AsyncHelper.FireAndForget2(() =>
                Config.DataMessageProcessingPackage.WaitStateManager?.OnHandshakeReceived(handShake))
            .ContinueWith(Callback);
        _stopped.WaitOne(TimeOut);
        //Config.MonitorLogger?.LogInformation($"received handshake message [{hs.HandshakeMessageType:X2}]");

    }

    private void Callback(IAsyncResult ar)
    {
        _stopped.Set();
    }
}