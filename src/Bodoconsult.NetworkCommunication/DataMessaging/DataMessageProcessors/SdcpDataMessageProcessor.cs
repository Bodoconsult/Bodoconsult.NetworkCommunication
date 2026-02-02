// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.DataMessaging.DataMessages;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessageProcessors;

/// <summary>
/// Current implementation of <see cref="IDataMessageProcessor"/> for SDCP protocol.
/// Should invoke IDataMessagingConfig.RaiseDataMessageReceivedDelegate for data messages and IDataMessagingConfig.DataMessageProcessingPackage.WaitStateManager?.OnHandshakeReceived for handshakes
/// </summary>
public class SdcpDataMessageProcessor : IDataMessageProcessor
{

    private readonly AutoResetEvent _stopped = new(false);

    private const int TimeOut = 2000;

    public readonly IDataMessagingConfig Config;

    /// <summary>
    /// Default ctor
    /// </summary>
    public SdcpDataMessageProcessor(IDataMessagingConfig config)
    {
        Config = config;
    }

    /// <summary>
    /// Process the message
    /// </summary>
    /// <param name="message">Message to process</param>
    public void ProcessMessage(IDataMessage message)
    {
        // handshake received
        if (message is HandshakeMessage handShake)
        {
            ProcessHandshakes(handShake);
            return;
        }

        // Tower data message received
        if (message is SdcpDataMessage dataMessage)
        {
            AsyncHelper.FireAndForget2(() => Config.RaiseCommLayerDataMessageReceivedDelegate?.Invoke(dataMessage)).ContinueWith(Callback);
        }

        // No valid message
    }

    // ReSharper disable once SuggestBaseTypeForParameter
    private void ProcessHandshakes(HandshakeMessage handShake)
    {
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