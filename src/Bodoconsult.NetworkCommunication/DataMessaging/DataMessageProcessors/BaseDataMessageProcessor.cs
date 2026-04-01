// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

using Bodoconsult.App.Helpers;
using Bodoconsult.NetworkCommunication.Interfaces;

namespace Bodoconsult.NetworkCommunication.DataMessaging.DataMessageProcessors;

/// <summary>
/// Base class for <see cref="IDataMessageProcessor"/> implementations
/// </summary>
public abstract class BaseDataMessageProcessor : IDataMessageProcessor
{
    private readonly AutoResetEvent _stopped = new(false);

    private const int TimeOut = 2000;

    /// <summary>
    /// Default ctor
    /// </summary>
    protected BaseDataMessageProcessor(IDataMessagingConfig config)
    {
        Config = config;
    }

    /// <summary>
    /// Current <see cref="IDataMessagingConfig"/> instance
    /// </summary>
    public readonly IDataMessagingConfig Config;

    /// <summary>
    /// Process the message
    /// </summary>
    /// <param name="message">Message to process</param>
    public virtual void ProcessMessage(IInboundMessage message)
    {
        throw new NotSupportedException("Override method in derived classes");
    }

    /// <summary>
    /// Process handshakes
    /// </summary>
    /// <param name="handShake">Handshake message</param>
    // ReSharper disable once SuggestBaseTypeForParameter
    protected void ProcessHandshakes(IInboundHandShakeMessage handShake)
    {
        ArgumentNullException.ThrowIfNull(Config.DataMessageProcessingPackage);

        // fire and forget but let CallBack() be run at the end
        AsyncHelper.FireAndForget2(() => Config.DataMessageProcessingPackage.WaitStateManager.OnHandshakeReceived(handShake))
            .ContinueWith(Callback);
        _stopped.WaitOne(TimeOut);
        //Config.MonitorLogger?.LogInformation($"received handshake message [{hs.HandshakeMessageType:X2}]");

    }

    protected void Callback(IAsyncResult ar)
    {
        _stopped.Set();
    }
}